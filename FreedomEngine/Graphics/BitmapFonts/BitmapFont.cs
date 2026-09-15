using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Graphics.BitmapFonts
{
    /// <summary>
    /// Represents a bitmap font and provides fast glyph lookup, measurement, and glyph enumeration.
    /// </summary>
    /// <remarks>
    /// Glyphs in the first 256 code points are stored in a flat array for constant-time lookup; all
    /// others fall back to a dictionary. Measurements are cached and invalidated automatically when a
    /// layout-affecting setting changes.
    /// </remarks>
    public sealed class BitmapFont
    {
        #region Constants & Fields

        /// <summary>
        /// The number of leading code points held in the flat lookup array. Covers Latin-1, not just ASCII.
        /// </summary>
        private const int DirectLookupRange = 256;

        /// <summary>
        /// The maximum number of measured strings retained by <see cref="_measureCache"/>.
        /// </summary>
        /// <remarks>
        /// Bounds the cache so that text which changes every frame — scores, timers, countdowns —
        /// cannot grow it without limit for the lifetime of the font.
        /// </remarks>
        private const int MeasureCacheCapacity = 512;

        /// <summary>
        /// Glyphs whose code point falls below <see cref="DirectLookupRange"/>, indexed by code point.
        /// </summary>
        private readonly BitmapFontCharacter[] _directCharacters;

        /// <summary>
        /// Glyphs whose code point falls outside <see cref="DirectLookupRange"/>, keyed by code point.
        /// </summary>
        private readonly Dictionary<int, BitmapFontCharacter> _characters;

        /// <summary>
        /// Cached measurements, keyed by the measured string.
        /// </summary>
        private readonly Dictionary<string, SizeF> _measureCache;

        /// <summary>
        /// The extra horizontal spacing applied between characters.
        /// </summary>
        private int _letterSpacing;

        /// <summary>
        /// The extra vertical spacing applied between lines.
        /// </summary>
        private int _lineSpacing;

        /// <summary>
        /// Indicates whether kerning pairs are applied during measurement and layout.
        /// </summary>
        private bool _useKernings;

        /// <summary>
        /// The current layout revision number.
        /// </summary>
        private int _revision;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the typeface this font was generated from.
        /// </summary>
        public string Face { get; }

        /// <summary>
        /// Gets the point size this font was generated at.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets the distance, in pixels, between the tops of two consecutive lines, before
        /// <see cref="LineSpacing"/> is applied.
        /// </summary>
        public int LineHeight { get; }

        /// <summary>
        /// Gets the distance, in pixels, from the top of a line down to the baseline that glyphs sit on.
        /// </summary>
        public int Baseline { get; }

        /// <summary>
        /// Gets a monotonically increasing revision number.
        /// The revision changes whenever font layout-related settings change.
        /// </summary>
        /// <remarks>
        /// Consumers that cache their own layout, such as <c>Text</c>, compare this value against the
        /// one they last saw in order to detect that the font changed underneath them.
        /// </remarks>
        public int Revision => _revision;

        /// <summary>
        /// Gets or sets the extra horizontal spacing, in pixels, added after each character.
        /// </summary>
        /// <remarks>Changing this bumps <see cref="Revision"/> and clears the measurement cache.</remarks>
        public int LetterSpacing
        {
            get => _letterSpacing;
            set
            {
                if (_letterSpacing == value) return;
                _letterSpacing = value;
                InvalidateLayoutCache();
            }
        }

        /// <summary>
        /// Gets or sets the extra vertical spacing, in pixels, added between lines on top of
        /// <see cref="LineHeight"/>.
        /// </summary>
        /// <remarks>Changing this bumps <see cref="Revision"/> and clears the measurement cache.</remarks>
        public int LineSpacing
        {
            get => _lineSpacing;
            set
            {
                if (_lineSpacing == value) return;
                _lineSpacing = value;
                InvalidateLayoutCache();
            }
        }

        /// <summary>
        /// Gets or sets whether kerning pairs are applied when measuring and laying out text.
        /// </summary>
        /// <remarks>Changing this bumps <see cref="Revision"/> and clears the measurement cache.</remarks>
        public bool UseKernings
        {
            get => _useKernings;
            set
            {
                if (_useKernings == value) return;
                _useKernings = value;
                InvalidateLayoutCache();
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapFont"/> class with no additional letter
        /// or line spacing.
        /// </summary>
        /// <param name="face">The name of the typeface this font was generated from.</param>
        /// <param name="size">The point size this font was generated at.</param>
        /// <param name="lineHeight">The distance, in pixels, between the tops of two consecutive lines.</param>
        /// <param name="characters">
        /// The glyphs making up this font. May be <see langword="null"/> or contain <see langword="null"/>
        /// entries, which are skipped.
        /// </param>
        /// <param name="baseline">The distance, in pixels, from the top of a line down to the baseline.</param>
        public BitmapFont(string face, int size, int lineHeight, IEnumerable<BitmapFontCharacter> characters, int baseline)
            : this(face, size, lineHeight, 0, 0, characters, baseline)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapFont"/> class.
        /// </summary>
        /// <param name="face">The name of the typeface this font was generated from. Treated as empty when <see langword="null"/>.</param>
        /// <param name="size">The point size this font was generated at.</param>
        /// <param name="lineHeight">The distance, in pixels, between the tops of two consecutive lines.</param>
        /// <param name="letterSpacing">The extra horizontal spacing, in pixels, added after each character.</param>
        /// <param name="lineSpacing">The extra vertical spacing, in pixels, added between lines.</param>
        /// <param name="characters">
        /// The glyphs making up this font. May be <see langword="null"/> or contain <see langword="null"/>
        /// entries, which are skipped. When two glyphs share a code point, the last one wins.
        /// </param>
        /// <param name="baseline">The distance, in pixels, from the top of a line down to the baseline.</param>
        public BitmapFont(string face, int size, int lineHeight, int letterSpacing, int lineSpacing, IEnumerable<BitmapFontCharacter> characters, int baseline)
        {
            Face = face ?? string.Empty;
            Size = size;
            LineHeight = lineHeight;
            Baseline = baseline;

            _letterSpacing = letterSpacing;
            _lineSpacing = lineSpacing;
            _useKernings = true;

            _directCharacters = new BitmapFontCharacter[DirectLookupRange];
            _characters = [];
            _measureCache = new Dictionary<string, SizeF>(128, StringComparer.Ordinal);

            if (characters == null) return;

            foreach (var character in characters)
            {
                if (character == null) continue;

                if (character.Character >= 0 && character.Character < DirectLookupRange)
                    _directCharacters[character.Character] = character;
                else
                    _characters[character.Character] = character;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the cached string measurements.
        /// </summary>
        public void ClearCache() => _measureCache.Clear();

        /// <summary>
        /// Gets the glyph for the given code point.
        /// </summary>
        /// <param name="character">The Unicode code point to look up.</param>
        /// <returns>The matching glyph, or <see langword="null"/> if this font has none.</returns>
        public BitmapFontCharacter GetCharacter(int character)
        {
            if (character >= 0 && character < DirectLookupRange)
                return _directCharacters[character];

            _characters.TryGetValue(character, out BitmapFontCharacter result);
            return result;
        }

        /// <summary>
        /// Attempts to get the glyph for the given code point.
        /// </summary>
        /// <param name="character">The Unicode code point to look up.</param>
        /// <param name="value">
        /// When this method returns, contains the matching glyph, or <see langword="null"/> if this
        /// font has none.
        /// </param>
        /// <returns><see langword="true"/> if a glyph was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetCharacter(int character, out BitmapFontCharacter value)
        {
            if (character >= 0 && character < DirectLookupRange)
            {
                value = _directCharacters[character];
                return value != null;
            }

            return _characters.TryGetValue(character, out value);
        }

        /// <summary>
        /// Measures the size of the given text as laid out by this font.
        /// </summary>
        /// <param name="text">The text to measure. Newlines are honoured; markup is not interpreted.</param>
        /// <returns>
        /// The width and height of the text's bounding box, or <see cref="SizeF.Empty"/> when the text
        /// is null, empty, or contains no renderable glyphs.
        /// </returns>
        /// <remarks>
        /// Results are cached by string. The cache is bounded by <see cref="MeasureCacheCapacity"/>
        /// and is cleared wholesale when full, so measuring a large number of distinct strings costs
        /// occasional re-measurement rather than unbounded memory.
        /// </remarks>
        public SizeF MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text)) return SizeF.Empty;

            if (_measureCache.TryGetValue(text, out SizeF cached))
                return cached;

            var bounds = GetStringRectangle(text, Vector2.Zero);
            var size = new SizeF(bounds.Width, bounds.Height);

            if (_measureCache.Count >= MeasureCacheCapacity)
                _measureCache.Clear();

            _measureCache[text] = size;
            return size;
        }

        /// <summary>
        /// Measures the size of the given text as laid out by this font.
        /// </summary>
        /// <param name="text">The text to measure.</param>
        /// <returns>
        /// The width and height of the text's bounding box, or <see cref="SizeF.Empty"/> when the text
        /// is null, empty, or contains no renderable glyphs.
        /// </returns>
        /// <remarks>
        /// This overload allocates a string on every call in order to build the cache key, so prefer
        /// the <see cref="MeasureString(string)"/> overload on a hot path.
        /// </remarks>
        public SizeF MeasureString(StringBuilder text)
        {
            // Note: ToString() creates a new string for the cache key.
            return text == null || text.Length == 0 ? SizeF.Empty : MeasureString(text.ToString());
        }

        /// <summary>
        /// Computes the bounding rectangle of the given text laid out from the given position.
        /// </summary>
        /// <param name="text">The text to lay out. Newlines are honoured; markup is not interpreted.</param>
        /// <param name="position">The position the first line starts at.</param>
        /// <returns>
        /// The bounding box of every rendered glyph, or <see cref="RectangleF.Empty"/> when the text
        /// is null, empty, or contains no glyph this font can render.
        /// </returns>
        /// <remarks>
        /// Code points with no matching glyph are skipped without advancing the pen. Surrogate pairs
        /// are decoded to a single code point.
        /// </remarks>
        public RectangleF GetStringRectangle(string text, Vector2 position = default)
        {
            if (string.IsNullOrEmpty(text)) return RectangleF.Empty;

            float x = position.X;
            float y = position.Y;

            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            bool hasGlyph = false;
            BitmapFontCharacter previous = null;

            for (int i = 0; i < text.Length; i++)
            {
                int codePoint = text[i];

                if (char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                {
                    codePoint = char.ConvertToUtf32(text[i], text[i + 1]);
                    i++;
                }

                if (codePoint == '\r') continue;
                if (codePoint == '\n')
                {
                    x = position.X;
                    y += LineHeight + LineSpacing;
                    previous = null;
                    maxY = Math.Max(maxY, y + LineHeight);
                    continue;
                }

                if (!TryGetCharacter(codePoint, out var character)) continue;

                if (UseKernings && previous != null && previous.Kernings.TryGetValue(codePoint, out int kern))
                    x += kern;

                float left = x + character.XOffset;
                float top = y + character.YOffset;
                float right = left + character.Rectangle.Width;
                float bottom = top + character.Rectangle.Height;

                minX = Math.Min(minX, left);
                minY = Math.Min(minY, top);
                maxX = Math.Max(maxX, right);
                maxY = Math.Max(maxY, bottom);
                hasGlyph = true;

                x += character.XAdvance + LetterSpacing;
                previous = character;
            }

            return hasGlyph ? new RectangleF(minX, minY, maxX - minX, maxY - minY) : RectangleF.Empty;
        }

        /// <summary>
        /// Returns the name of the typeface this font was generated from.
        /// </summary>
        /// <returns>The value of <see cref="Face"/>.</returns>
        public override string ToString() => Face;

        #endregion

        #region Private Methods

        /// <summary>
        /// Bumps the layout revision and drops every cached measurement.
        /// </summary>
        private void InvalidateLayoutCache()
        {
            _revision++;
            _measureCache.Clear();
        }

        #endregion
    }
}
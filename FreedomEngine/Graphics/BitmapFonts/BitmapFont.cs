using Microsoft.Xna.Framework;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace FreedomEngine.Graphics.BitmapFonts
{
    /// <summary>
    /// Represents a bitmap font and provides fast glyph lookup, measurement, and glyph enumeration.
    /// </summary>
    public sealed class BitmapFont
    {
        #region Constants & Fields

        private const int AsciiRange = 256;

        private readonly BitmapFontCharacter[] _asciiCharacters;
        private readonly Dictionary<int, BitmapFontCharacter> _characters;
        private readonly Dictionary<string, SizeF> _measureCache;

        private int _letterSpacing;
        private int _lineSpacing;
        private bool _useKernings;
        private int _revision;

        #endregion

        #region Constructors

        public BitmapFont(string face, int size, int lineHeight, IEnumerable<BitmapFontCharacter> characters, int baseline)
            : this(face, size, lineHeight, 0, 0, characters, baseline)
        {
        }

        public BitmapFont(string face, int size, int lineHeight, int letterSpacing, int lineSpacing, IEnumerable<BitmapFontCharacter> characters, int baseline)
        {
            Face = face ?? string.Empty;
            Size = size;
            LineHeight = lineHeight;
            Baseline = baseline;

            _letterSpacing = letterSpacing;
            _lineSpacing = lineSpacing;
            _useKernings = true;

            _asciiCharacters = new BitmapFontCharacter[AsciiRange];
            _characters = [];
            _measureCache = new Dictionary<string, SizeF>(128, StringComparer.Ordinal);

            if (characters == null) return;

            foreach (var character in characters)
            {
                if (character == null) continue;

                if (character.Character >= 0 && character.Character < AsciiRange)
                    _asciiCharacters[character.Character] = character;
                else
                    _characters[character.Character] = character;
            }
        }

        #endregion

        #region Properties

        public string Face { get; }
        public int Size { get; }
        public int LineHeight { get; }
        public int Baseline { get; }

        /// <summary>
        /// Gets a monotonically increasing revision number.
        /// The revision changes whenever font layout-related settings change.
        /// </summary>
        public int Revision => _revision;

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

        #region Public Methods

        public void ClearCache() => _measureCache.Clear();

        public BitmapFontCharacter GetCharacter(int character)
        {
            if (character >= 0 && character < AsciiRange)
                return _asciiCharacters[character];

            _characters.TryGetValue(character, out BitmapFontCharacter result);
            return result;
        }

        public bool TryGetCharacter(int character, out BitmapFontCharacter value)
        {
            if (character >= 0 && character < AsciiRange)
            {
                value = _asciiCharacters[character];
                return value != null;
            }

            return _characters.TryGetValue(character, out value);
        }

        public SizeF MeasureString(string text)
        {
            if (string.IsNullOrEmpty(text)) return SizeF.Empty;

            if (_measureCache.TryGetValue(text, out SizeF cached))
                return cached;

            var bounds = GetStringRectangle(text, Vector2.Zero);
            var size = new SizeF(bounds.Width, bounds.Height);

            _measureCache[text] = size;
            return size;
        }

        public SizeF MeasureString(StringBuilder text)
        {
            // Note: ToString() creates a new string for the cache key.
            return text == null || text.Length == 0 ? SizeF.Empty : MeasureString(text.ToString());
        }

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
                float right = left + character.TextureRegion.Width;
                float bottom = top + character.TextureRegion.Height;

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

        public override string ToString() => Face;

        #endregion

        #region Private Methods

        private void InvalidateLayoutCache()
        {
            _revision++;
            _measureCache.Clear();
        }

        #endregion
    }
}
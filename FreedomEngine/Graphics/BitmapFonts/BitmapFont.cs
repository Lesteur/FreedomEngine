using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Graphics.BitmapFonts
{
    /// <summary>
    /// Represents a bitmap font and provides fast glyph lookup, measurement, and glyph enumeration.
    /// </summary>
    public sealed class BitmapFont
    {
        private const int AsciiRange = 256;

        private readonly BitmapFontCharacter[] _asciiCharacters;
        private readonly Dictionary<int, BitmapFontCharacter> _characters;
        private readonly Dictionary<string, SizeF> _measureCache;

        private int _letterSpacing;
        private int _lineSpacing;
        private bool _useKernings;
        private int _revision;

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
                if (_letterSpacing == value)
                    return;

                _letterSpacing = value;
                InvalidateLayoutCache();
            }
        }

        public int LineSpacing
        {
            get => _lineSpacing;
            set
            {
                if (_lineSpacing == value)
                    return;

                _lineSpacing = value;
                InvalidateLayoutCache();
            }
        }

        public bool UseKernings
        {
            get => _useKernings;
            set
            {
                if (_useKernings == value)
                    return;

                _useKernings = value;
                InvalidateLayoutCache();
            }
        }

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
            _characters = new Dictionary<int, BitmapFontCharacter>();
            _measureCache = new Dictionary<string, SizeF>(128, StringComparer.Ordinal);

            if (characters == null)
                return;

            foreach (BitmapFontCharacter character in characters)
            {
                if (character == null)
                    continue;

                if (character.Character >= 0 && character.Character < AsciiRange)
                    _asciiCharacters[character.Character] = character;
                else
                    _characters[character.Character] = character;
            }
        }

        /// <summary>
        /// Clears all measurement caches.
        /// </summary>
        public void ClearCache()
        {
            _measureCache.Clear();
        }

        private void InvalidateLayoutCache()
        {
            _revision++;
            _measureCache.Clear();
        }

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
            if (string.IsNullOrEmpty(text))
                return SizeF.Empty;

            if (_measureCache.TryGetValue(text, out SizeF cached))
                return cached;

            RectangleF bounds = GetStringRectangle(text, Vector2.Zero);
            SizeF size = new SizeF(bounds.Width, bounds.Height);

            _measureCache[text] = size;
            return size;
        }

        public SizeF MeasureString(StringBuilder text)
        {
            if (text == null || text.Length == 0)
                return SizeF.Empty;

            return MeasureString(text.ToString());
        }

        public RectangleF GetStringRectangle(string text)
        {
            return GetStringRectangle(text, Vector2.Zero);
        }

        public RectangleF GetStringRectangle(string text, Vector2 position)
        {
            if (string.IsNullOrEmpty(text))
                return RectangleF.Empty;

            float x = position.X;
            float y = position.Y;
            float lineStartX = position.X;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

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

                if (codePoint == '\r')
                    continue;

                if (codePoint == '\n')
                {
                    x = lineStartX;
                    y += LineHeight + LineSpacing;
                    previous = null;
                    maxY = Math.Max(maxY, y + LineHeight);
                    continue;
                }

                if (!TryGetCharacter(codePoint, out BitmapFontCharacter character) || character == null)
                    continue;

                if (UseKernings && previous != null && previous.Kernings.TryGetValue(codePoint, out int kern))
                    x += kern;

                float left = x + character.XOffset;
                float top = y + character.YOffset;
                float right = left + character.TextureRegion.Width;
                float bottom = top + character.TextureRegion.Height;

                if (!hasGlyph)
                {
                    minX = left;
                    minY = top;
                    maxX = right;
                    maxY = bottom;
                    hasGlyph = true;
                }
                else
                {
                    if (left < minX) minX = left;
                    if (top < minY) minY = top;
                    if (right > maxX) maxX = right;
                    if (bottom > maxY) maxY = bottom;
                }

                x += character.XAdvance + LetterSpacing;
                previous = character;
            }

            if (!hasGlyph)
                return RectangleF.Empty;

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public RectangleF GetStringRectangle(StringBuilder text, Vector2? position = null)
        {
            if (text == null || text.Length == 0)
                return RectangleF.Empty;

            return GetStringRectangle(text.ToString(), position ?? Vector2.Zero);
        }

        public struct BitmapFontGlyph
        {
            public int CharacterID;
            public Vector2 Position;
            public BitmapFontCharacter Character;
        }

        public StringGlyphEnumerable GetGlyphs(string text, Vector2? position = null)
        {
            return new StringGlyphEnumerable(this, text ?? string.Empty, position);
        }

        public StringGlyphEnumerable GetGlyphs(StringBuilder text, Vector2? position = null)
        {
            return new StringGlyphEnumerable(this, text?.ToString() ?? string.Empty, position);
        }

        public readonly struct StringGlyphEnumerable : IEnumerable<BitmapFontGlyph>
        {
            private readonly StringGlyphEnumerator _enumerator;

            public StringGlyphEnumerable(BitmapFont font, string text, Vector2? position)
            {
                _enumerator = new StringGlyphEnumerator(font, text, position);
            }

            public StringGlyphEnumerator GetEnumerator() => _enumerator;

            IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator() => _enumerator;
            IEnumerator IEnumerable.GetEnumerator() => _enumerator;
        }

        public struct StringGlyphEnumerator : IEnumerator<BitmapFontGlyph>
        {
            private readonly BitmapFont _font;
            private readonly string _text;
            private readonly Vector2 _start;

            private int _index;
            private Vector2 _delta;
            private BitmapFontGlyph _current;
            private BitmapFontCharacter _previous;

            public BitmapFontGlyph Current => _current;
            object IEnumerator.Current => throw new InvalidOperationException();

            public StringGlyphEnumerator(BitmapFont font, string text, Vector2? position)
            {
                _font = font;
                _text = text ?? string.Empty;
                _start = position ?? Vector2.Zero;
                _index = -1;
                _delta = Vector2.Zero;
                _current = default;
                _previous = null;
            }

            public bool MoveNext()
            {
                if (++_index >= _text.Length)
                    return false;

                int codePoint = _text[_index];

                if (char.IsHighSurrogate(_text[_index]) && _index + 1 < _text.Length && char.IsLowSurrogate(_text[_index + 1]))
                {
                    codePoint = char.ConvertToUtf32(_text[_index], _text[_index + 1]);
                    _index++;
                }

                if (codePoint == '\r')
                    return MoveNext();

                if (codePoint == '\n')
                {
                    _current = new BitmapFontGlyph
                    {
                        CharacterID = codePoint,
                        Character = null,
                        Position = _start + _delta
                    };

                    _delta.Y += _font.LineHeight + _font.LineSpacing;
                    _delta.X = 0;
                    _previous = null;
                    return true;
                }

                var character = _font.GetCharacter(codePoint);
                var position = _start + _delta;

                if (character != null)
                {
                    if (_font.UseKernings && _previous != null && _previous.Kernings.TryGetValue(codePoint, out int kern))
                    {
                        _delta.X += kern;
                        position.X += kern;
                    }

                    position.X += character.XOffset;
                    position.Y += character.YOffset;
                    _delta.X += character.XAdvance + _font.LetterSpacing;
                }

                _current = new BitmapFontGlyph
                {
                    CharacterID = codePoint,
                    Character = character,
                    Position = position
                };

                _previous = character;
                return true;
            }

            public void Reset()
            {
                _index = -1;
                _delta = Vector2.Zero;
                _previous = null;
            }

            public void Dispose()
            {
            }
        }

        public override string ToString() => Face;
    }
}
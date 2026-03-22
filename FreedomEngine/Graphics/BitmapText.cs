using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics.BitmapFonts;

using RectangleF = System.Drawing.RectangleF;

namespace FreedomEngine.Graphics
{
    public enum TextHorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public enum TextVerticalAlignment
    {
        Top,
        Middle,
        Bottom
    }

    /// <summary>
    /// Represents a drawable rich bitmap text with cached parsing and cached glyph layout.
    /// </summary>
    public sealed class BitmapText
    {
        private sealed class StyleFrame
        {
            public string Name;
            public TextStyleState Style;
        }

        private struct TextStyleState
        {
            public Color Color;
            public Vector2 Scale;
            public float ShakeAmplitude;
        }

        private struct TextRun
        {
            public int Start;
            public int Length;
            public TextStyleState Style;
        }

        private struct GlyphRenderData
        {
            public BitmapFontCharacter Character;
            public Vector2 Position;
            public Color Color;
            public Vector2 Scale;
            public float ShakeAmplitude;
            public float ShakeSeed;
        }

        private readonly List<TextRun> _runs = new List<TextRun>(32);
        private readonly List<GlyphRenderData> _glyphs = new List<GlyphRenderData>(128);

        private string _text = string.Empty;
        private bool _markupDirty = true;
        private bool _layoutDirty = true;
        private int _fontRevision = -1;
        private RectangleF _layoutBounds = RectangleF.Empty;
        private float _time;
        private Random _random = new Random();

        private Color _defaultColor = Color.White;
        private Vector2 _defaultScale = Vector2.One;
        private BitmapFont _font;

        public BitmapFont Font
        {
            get => _font;
            set
            {
                if (ReferenceEquals(_font, value))
                    return;

                _font = value;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the text content. Changing this property invalidates the cached layout.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                value ??= string.Empty;
                if (string.Equals(_text, value, StringComparison.Ordinal))
                    return;

                _text = value;
                _markupDirty = true;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Convenience setter for callers that build text with a StringBuilder.
        /// A snapshot is taken to keep cache invalidation reliable.
        /// </summary>
        public void SetText(StringBuilder builder)
        {
            Text = builder?.ToString() ?? string.Empty;
        }

        public Vector2 Position { get; set; }

        /// <summary>
        /// Rotation applied to the full text block.
        /// </summary>
        public float Rotation { get; set; }

        /// <summary>
        /// Origin used by Rotation, expressed in local text space.
        /// </summary>
        public Vector2 Origin { get; set; }

        public Color DefaultColor
        {
            get => _defaultColor;
            set
            {
                if (_defaultColor == value)
                    return;

                _defaultColor = value;
                _markupDirty = true;
                _layoutDirty = true;
            }
        }

        public Vector2 DefaultScale
        {
            get => _defaultScale;
            set
            {
                if (_defaultScale == value)
                    return;

                _defaultScale = value;
                _markupDirty = true;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Returns the last computed local bounds of the text layout.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                EnsureLayout();
                return _layoutBounds;
            }
        }

        public int Width
        {
            get
            {
                EnsureLayout();
                return (int)Math.Ceiling(_layoutBounds.Width);
            }
        }

        public int Height
        {
            get
            {
                EnsureLayout();
                return (int)Math.Ceiling(_layoutBounds.Height);
            }
        }

        public BitmapText(BitmapFont font, string text)
            : this(font, text, Vector2.Zero)
        {
        }

        public BitmapText(BitmapFont font, string text, Vector2 position)
        {
            _font = font;
            _text = text ?? string.Empty;
            Position = position;
            Origin = Vector2.Zero;
            Rotation = 0f;
            _markupDirty = true;
            _layoutDirty = true;
        }

        public BitmapText(BitmapFont font, string text, int x, int y)
            : this(font, text, new Vector2(x, y))
        {
        }

        public void Update(GameTime gameTime)
        {
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null || Font == null)
                return;

            EnsureLayout();

            if (_glyphs.Count == 0)
                return;

            Matrix rotationMatrix = Rotation != 0f
                ? Matrix.CreateRotationZ(Rotation)
                : Matrix.Identity;

            for (int i = 0; i < _glyphs.Count; i++)
            {
                GlyphRenderData glyph = _glyphs[i];

                Vector2 localPosition = glyph.Position;
                if (Rotation != 0f)
                    localPosition = Vector2.Transform(localPosition - Origin, rotationMatrix) + Origin;

                Vector2 drawPosition = Position + localPosition;

                if (glyph.ShakeAmplitude > 0f)
                {
                    drawPosition.X += _random.Next(-1, 2) * glyph.ShakeAmplitude;
                    drawPosition.Y += _random.Next(-1, 2) * glyph.ShakeAmplitude;
                }

                spriteBatch.Draw(
                    glyph.Character.TextureRegion.Texture,
                    drawPosition,
                    glyph.Character.TextureRegion.SourceRectangle,
                    glyph.Color,
                    0f,
                    Vector2.Zero,
                    glyph.Scale,
                    SpriteEffects.None,
                    0f);
            }
        }

        private void EnsureLayout()
        {
            if (Font == null)
            {
                _glyphs.Clear();
                _layoutBounds = RectangleF.Empty;
                _markupDirty = false;
                _layoutDirty = false;
                _fontRevision = -1;
                return;
            }

            if (_fontRevision != Font.Revision)
                _layoutDirty = true;

            if (_markupDirty)
                ParseMarkup();

            if (_layoutDirty)
                BuildLayout();

            _fontRevision = Font.Revision;
        }

        private void ParseMarkup()
        {
            _runs.Clear();

            if (string.IsNullOrEmpty(_text))
            {
                _markupDirty = false;
                return;
            }

            var stack = new Stack<StyleFrame>(8);
            stack.Push(new StyleFrame
            {
                Name = string.Empty,
                Style = new TextStyleState
                {
                    Color = _defaultColor,
                    Scale = _defaultScale,
                    ShakeAmplitude = 0f
                }
            });

            TextStyleState currentStyle = stack.Peek().Style;
            int segmentStart = 0;

            for (int i = 0; i < _text.Length; i++)
            {
                if (_text[i] != '[')
                    continue;

                int closeIndex = _text.IndexOf(']', i + 1);
                if (closeIndex < 0)
                    break;

                string tag = _text.Substring(i + 1, closeIndex - i - 1).Trim();
                if (!TryConsumeMarkupTag(tag, stack, out TextStyleState newStyle))
                    continue;

                if (i > segmentStart)
                {
                    _runs.Add(new TextRun
                    {
                        Start = segmentStart,
                        Length = i - segmentStart,
                        Style = currentStyle
                    });
                }

                currentStyle = newStyle;
                segmentStart = closeIndex + 1;
                i = closeIndex;
            }

            if (segmentStart < _text.Length)
            {
                _runs.Add(new TextRun
                {
                    Start = segmentStart,
                    Length = _text.Length - segmentStart,
                    Style = currentStyle
                });
            }

            _markupDirty = false;
            _layoutDirty = true;
        }

        private bool TryConsumeMarkupTag(string tag, Stack<StyleFrame> stack, out TextStyleState currentStyle)
        {
            currentStyle = stack.Peek().Style;

            if (string.IsNullOrWhiteSpace(tag))
                return false;

            tag = tag.Trim();

            if (string.Equals(tag, "reset", StringComparison.OrdinalIgnoreCase))
            {
                while (stack.Count > 1)
                    stack.Pop();

                currentStyle = stack.Peek().Style;
                return true;
            }

            if (tag[0] == '/' || tag[0] == '\\')
            {
                string closeName = tag.Substring(1).Trim();
                if (closeName.Length == 0)
                {
                    if (stack.Count > 1)
                        stack.Pop();

                    currentStyle = stack.Peek().Style;
                    return true;
                }

                if (stack.Count <= 1)
                    return false;

                if (!string.Equals(stack.Peek().Name, closeName, StringComparison.OrdinalIgnoreCase))
                    return false;

                stack.Pop();
                currentStyle = stack.Peek().Style;
                return true;
            }

            string[] parts = tag.Split(new[] { ' ', '\t', ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            string name = parts[0].Trim();
            TextStyleState next = stack.Peek().Style;

            switch (name.ToLowerInvariant())
            {
                case "color":
                    if (parts.Length < 2 || !TryParseColor(parts[1], out Color color))
                        return false;

                    next.Color = color;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                case "scale":
                    if (!TryParseScale(parts, out Vector2 scale))
                        return false;

                    next.Scale *= scale;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                case "shake":
                    if (!TryParseShake(parts, out float amplitude))
                        return false;

                    next.ShakeAmplitude = amplitude;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                default:
                    return false;
            }
        }

        private static bool TryParseColor(string token, out Color color)
        {
            color = Color.White;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            token = token.Trim();

            switch (token.ToLowerInvariant())
            {
                case "white": color = Color.White; return true;
                case "black": color = Color.Black; return true;
                case "red": color = Color.Red; return true;
                case "green": color = Color.Green; return true;
                case "blue": color = Color.Blue; return true;
                case "yellow": color = Color.Yellow; return true;
                case "orange": color = Color.Orange; return true;
                case "purple": color = Color.Purple; return true;
                case "cyan": color = Color.Cyan; return true;
                case "magenta": color = Color.Magenta; return true;
                case "gray":
                case "grey": color = Color.Gray; return true;
            }

            if (token[0] == '#')
                token = token.Substring(1);

            try
            {
                if (token.Length == 6)
                {
                    byte r = byte.Parse(token.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(token.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(token.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    color = new Color(r, g, b, (byte)255);
                    return true;
                }

                if (token.Length == 8)
                {
                    byte a = byte.Parse(token.AsSpan(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte r = byte.Parse(token.AsSpan(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte g = byte.Parse(token.AsSpan(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    byte b = byte.Parse(token.AsSpan(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    color = new Color(r, g, b, a);
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        private static bool TryParseScale(string[] parts, out Vector2 scale)
        {
            scale = Vector2.One;

            if (parts.Length < 2)
                return false;

            if (parts.Length == 2)
            {
                if (!TryParseFloat(parts[1], out float uniform))
                    return false;

                scale = new Vector2(uniform, uniform);
                return true;
            }

            if (!TryParseFloat(parts[1], out float x))
                return false;

            if (!TryParseFloat(parts[2], out float y))
                return false;

            scale = new Vector2(x, y);
            return true;
        }

        private static bool TryParseShake(string[] parts, out float amplitude)
        {
            amplitude = 0f;

            if (parts.Length < 1)
                return false;

            if (!TryParseFloat(parts[1], out amplitude))
                return false;

            return true;
        }

        private static bool TryParseFloat(string token, out float value)
        {
            return float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private void BuildLayout()
        {
            _glyphs.Clear();

            if (Font == null || _runs.Count == 0)
            {
                _layoutBounds = RectangleF.Empty;
                _layoutDirty = false;
                return;
            }

            float penX = 0f;
            float penY = 0f;
            float lineStartX = 0f;

            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            bool hasGlyph = false;
            BitmapFontCharacter previousCharacter = null;
            //int glyphIndex = 0;
            float currentLineScaleY = _defaultScale.Y;

            for (int runIndex = 0; runIndex < _runs.Count; runIndex++)
            {
                TextRun run = _runs[runIndex];
                TextStyleState style = run.Style;
                float scaleX = style.Scale.X;
                float scaleY = style.Scale.Y;
                currentLineScaleY = scaleY;

                int end = run.Start + run.Length;
                for (int i = run.Start; i < end; i++)
                {
                    char ch = _text[i];

                    if (ch == '\r')
                        continue;

                    if (ch == '\n')
                    {
                        penX = lineStartX;
                        penY += (Font.LineHeight + Font.LineSpacing) * scaleY;
                        previousCharacter = null;

                        float lineBottom = penY + Font.LineHeight * scaleY;
                        if (lineBottom > maxY)
                            maxY = lineBottom;

                        continue;
                    }

                    int codePoint = ch;
                    if (char.IsHighSurrogate(ch) && i + 1 < end && char.IsLowSurrogate(_text[i + 1]))
                    {
                        codePoint = char.ConvertToUtf32(_text[i], _text[i + 1]);
                        i++;
                    }

                    if (!Font.TryGetCharacter(codePoint, out BitmapFontCharacter character) || character == null)
                        continue;

                    if (Font.UseKernings && previousCharacter != null && previousCharacter.Kernings.TryGetValue(codePoint, out int kern))
                        penX += kern * scaleX;

                    float x = penX + character.XOffset * scaleX;
                    float y = penY + character.YOffset * scaleY;
                    float width = character.TextureRegion.Width * scaleX;
                    float height = character.TextureRegion.Height * scaleY;

                    float left = Math.Min(x, x + width);
                    float top = Math.Min(y, y + height);
                    float right = Math.Max(x, x + width);
                    float bottom = Math.Max(y, y + height);

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

                    _glyphs.Add(new GlyphRenderData
                    {
                        Character = character,
                        Position = new Vector2(x, y),
                        Color = style.Color,
                        Scale = style.Scale,
                        ShakeAmplitude = style.ShakeAmplitude,
                    });

                    penX += (character.XAdvance + Font.LetterSpacing) * scaleX;
                    previousCharacter = character;
                }
            }

            if (!hasGlyph)
            {
                _layoutBounds = RectangleF.Empty;
                _layoutDirty = false;
                return;
            }

            maxY = Math.Max(maxY, penY + Font.LineHeight * currentLineScaleY);
            _layoutBounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            _layoutDirty = false;
        }
    }
}
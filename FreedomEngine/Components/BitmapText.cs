using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreedomEngine.Graphics.BitmapFonts;

using RectangleF = System.Drawing.RectangleF;

namespace FreedomEngine.Components
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
        }

        private readonly List<TextRun> _runs = new List<TextRun>(32);
        private readonly List<GlyphRenderData> _glyphs = new List<GlyphRenderData>(128);

        private string _text = string.Empty;
        private bool _markupDirty = true;
        private bool _layoutDirty = true;
        private int _fontRevision = -1;
        private RectangleF _layoutBounds = RectangleF.Empty;
        private float _time;
        private readonly Random _random = new();

        private Color _defaultColor = Color.White;
        private Vector2 _defaultScale = Vector2.One;
        private BitmapFont _font;

        private int _maxWidth = int.MaxValue;
        private int _jumpHeight = 20;

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

        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Left;
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Top;

        /// <summary>
        /// Gets or sets the horizontal alignment of the text block.
        /// Changing this property invalidates the layout.
        /// </summary>
        public TextHorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                if (_horizontalAlignment == value)
                    return;

                _horizontalAlignment = value;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text block.
        /// Changing this property invalidates the layout.
        /// </summary>
        public TextVerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                if (_verticalAlignment == value)
                    return;

                _verticalAlignment = value;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the maximum line width before wrapping text to a new line.
        /// Changing this property invalidates the layout.
        /// </summary>
        public int MaxWidth
        {
            get => _maxWidth;
            set
            {
                if (_maxWidth == value)
                    return;

                _maxWidth = value;
                _layoutDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the vertical block distance added for each new line.
        /// Changing this property invalidates the layout.
        /// </summary>
        public int JumpHeight
        {
            get => _jumpHeight;
            set
            {
                if (_jumpHeight == value)
                    return;

                _jumpHeight = value;
                _layoutDirty = true;
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
                    Rotation, //0f,
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

        private static bool TryConsumeMarkupTag(string tag, Stack<StyleFrame> stack, out TextStyleState currentStyle)
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
                    if (parts.Length < 2 || !Parsing.TryParseColor(parts[1], out Color color))
                        return false;

                    next.Color = color;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                case "scale":
                    if (!Parsing.TryParseScale(parts, out Vector2 scale))
                        return false;

                    next.Scale *= scale;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                case "shake":
                    if (!Parsing.TryParseShake(parts, out float amplitude))
                        return false;

                    next.ShakeAmplitude = amplitude;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                default:
                    return false;
            }
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
            
            // The baseline represents the ground on which our letters rest.
            float currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;

            // Variables for word-wrapping logic.
            int wordStartIndex = 0;
            float wordStartPenX = 0f;

            BitmapFontCharacter previousCharacter = null;

            // 1. Generate text and handle word-wrapping block per block
            for (int runIndex = 0; runIndex < _runs.Count; runIndex++)
            {
                TextRun run = _runs[runIndex];
                TextStyleState style = run.Style;
                float scaleX = style.Scale.X;
                float scaleY = style.Scale.Y;
                
                int end = run.Start + run.Length;
                
                for (int i = run.Start; i < end; i++)
                {
                    char ch = _text[i];

                    if (ch == '\r')
                        continue;

                    // Manual line-break
                    if (ch == '\n')
                    {
                        penX = 0f;
                        penY += _jumpHeight * _defaultScale.Y;
                        currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;
                        previousCharacter = null;
                        
                        // Restart word tracking for the new line
                        wordStartIndex = _glyphs.Count;
                        wordStartPenX = penX;
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

                    bool isWhitespace = char.IsWhiteSpace(ch);

                    if (Font.UseKernings && previousCharacter != null && previousCharacter.Kernings.TryGetValue(codePoint, out int kern))
                        penX += kern * scaleX;

                    float x = penX + character.XOffset * scaleX;
                    float glyphBaselineOffset = Font.Baseline - character.YOffset;
                    float y = currentLineBaseline - glyphBaselineOffset * scaleY;
                    
                    float width = character.TextureRegion.Width * scaleX;

                    // Word Wrapping Check
                    // If a continuous word exceeds MaxWidth, shift its already parsed characters to a new line.
                    if (!isWhitespace && x + width > _maxWidth && wordStartPenX > 0f)
                    {
                        float shiftX = wordStartPenX;
                        float shiftY = _jumpHeight * _defaultScale.Y;

                        penY += shiftY;
                        currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;

                        // Shift previously added glyphs belonging to this specific word
                        for (int j = wordStartIndex; j < _glyphs.Count; j++)
                        {
                            GlyphRenderData g = _glyphs[j];
                            g.Position.X -= shiftX;
                            g.Position.Y += shiftY;
                            _glyphs[j] = g;
                        }

                        // Align the current character to its new starting origin
                        penX -= shiftX;
                        x -= shiftX;
                        y += shiftY;

                        // Make sure we can't wrap again on this newly created line
                        wordStartPenX = 0f; 
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

                    // Spaces mark the boundary of words. When one is found, next characters belong to a new word.
                    if (isWhitespace)
                    {
                        wordStartIndex = _glyphs.Count;
                        wordStartPenX = penX; // The next word begins after this space
                    }
                }
            }

            if (_glyphs.Count == 0)
            {
                _layoutBounds = RectangleF.Empty;
                _layoutDirty = false;
                return;
            }

            // 2. Measure overall bounding box of the text using final positions post-wrapping
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int i = 0; i < _glyphs.Count; i++)
            {
                GlyphRenderData g = _glyphs[i];
                float left = g.Position.X;
                float top = g.Position.Y;
                float right = left + g.Character.TextureRegion.Width * g.Scale.X;
                float bottom = top + g.Character.TextureRegion.Height * g.Scale.Y;

                if (left < minX) minX = left;
                if (top < minY) minY = top;
                if (right > maxX) maxX = right;
                if (bottom > maxY) maxY = bottom;
            }

            // 3. Compute Alignments Offset
            float alignOffsetX = 0f;
            float alignOffsetY = 0f;

            switch (HorizontalAlignment)
            {
                case TextHorizontalAlignment.Center:
                    alignOffsetX = minX + (maxX - minX) * 0.5f;
                    break;
                case TextHorizontalAlignment.Right:
                    alignOffsetX = maxX;
                    break;
            }

            // The exact baseline of our entire block uses the last penY calculation
            float blockBaseline = penY + Font.Baseline * _defaultScale.Y;

            switch (VerticalAlignment)
            {
                case TextVerticalAlignment.Middle:
                    alignOffsetY = minY + (maxY - minY) * 0.5f;
                    break;
                case TextVerticalAlignment.Bottom:
                    alignOffsetY = blockBaseline;
                    break;
            }

            // 4. Transform all glyphs locally to match the pivot/origin requirements
            if (alignOffsetX != 0f || alignOffsetY != 0f)
            {
                for (int i = 0; i < _glyphs.Count; i++)
                {
                    GlyphRenderData g = _glyphs[i];
                    g.Position.X -= alignOffsetX;
                    g.Position.Y -= alignOffsetY;
                    _glyphs[i] = g;
                }

                minX -= alignOffsetX;
                maxX -= alignOffsetX;
                minY -= alignOffsetY;
                maxY -= alignOffsetY;
            }

            _layoutBounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            _layoutDirty = false;
        }
    }
}
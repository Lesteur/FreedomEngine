using FreedomEngine.Graphics.BitmapFonts;

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using RectangleF = System.Drawing.RectangleF;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Specifies the horizontal alignment of a text block.
    /// </summary>
    public enum TextHorizontalAlignment
    {
        /// <summary>
        /// Aligns the text to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Centers the text horizontally.
        /// </summary>
        Center,

        /// <summary>
        /// Aligns the text to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the vertical alignment of a text block.
    /// </summary>
    public enum TextVerticalAlignment
    {
        /// <summary>
        /// Aligns the text to the top.
        /// </summary>
        Top,

        /// <summary>
        /// Centers the text vertically.
        /// </summary>
        Middle,

        /// <summary>
        /// Aligns the text to the bottom.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Represents a drawable rich bitmap text with cached parsing and cached glyph layout.
    /// Supports inline markup for color, scale, and shake animations.
    /// </summary>
    public sealed class Text
    {
        #region Nested Types

        /// <summary>
        /// Represents a rendering style frame stored in the parsing stack.
        /// </summary>
        private sealed class StyleFrame
        {
            /// <summary>
            /// The name of the style tag.
            /// </summary>
            public string Name;

            /// <summary>
            /// The text style state applied within this frame.
            /// </summary>
            public TextStyleState Style;
        }

        /// <summary>
        /// Represents the current state of text styling, such as color, scale, and shake effect.
        /// </summary>
        private struct TextStyleState
        {
            /// <summary>
            /// The current color of the text.
            /// </summary>
            public Color Color;

            /// <summary>
            /// The current scale of the text.
            /// </summary>
            public Vector2 Scale;

            /// <summary>
            /// The amplitude of the shake effect for the text.
            /// </summary>
            public float ShakeAmplitude;
        }

        /// <summary>
        /// Represents a contiguous sequence of characters that share the same style.
        /// </summary>
        private struct TextRun
        {
            /// <summary>
            /// The starting index of the run in the plain text string.
            /// </summary>
            public int Start;

            /// <summary>
            /// The length of the run.
            /// </summary>
            public int Length;

            /// <summary>
            /// The style state applied to this run.
            /// </summary>
            public TextStyleState Style;
        }

        /// <summary>
        /// Contains all data necessary to render a single parsed and positioned glyph.
        /// </summary>
        private struct GlyphRenderData
        {
            /// <summary>
            /// The underlying bitmap font character.
            /// </summary>
            public BitmapFontCharacter Character;

            /// <summary>
            /// The local position of the glyph.
            /// </summary>
            public Vector2 Position;

            /// <summary>
            /// The tint color of the glyph.
            /// </summary>
            public Color Color;

            /// <summary>
            /// The scale of the glyph.
            /// </summary>
            public Vector2 Scale;

            /// <summary>
            /// The amplitude of the shake effect applied to this specific glyph.
            /// </summary>
            public float ShakeAmplitude;
        }

        #endregion

        #region Fields

        /// <summary>
        /// The list of text runs mapped during markup parsing.
        /// </summary>
        private readonly List<TextRun> _runs = new(32);

        /// <summary>
        /// The list of computed glyph data ready for rendering.
        /// </summary>
        private readonly List<GlyphRenderData> _glyphs = new(128);

        /// <summary>
        /// The random number generator used for calculating shake effects.
        /// </summary>
        private readonly Random _random = new();

        /// <summary>
        /// The font used to measure and render the text.
        /// </summary>
        private BitmapFont _font;

        /// <summary>
        /// The raw string text containing potential markup tags.
        /// </summary>
        private string _text = string.Empty;
        
        /// <summary>
        /// The default color applied when no markup defines a specific color.
        /// </summary>
        private Color _defaultColor = Color.White;

        /// <summary>
        /// The default scale applied when no markup defines a specific scale.
        /// </summary>
        private Vector2 _defaultScale = Vector2.One;
        
        /// <summary>
        /// The internal tracking for horizontal alignment.
        /// </summary>
        private TextHorizontalAlignment _horizontalAlignment = TextHorizontalAlignment.Left;

        /// <summary>
        /// The internal tracking for vertical alignment.
        /// </summary>
        private TextVerticalAlignment _verticalAlignment = TextVerticalAlignment.Top;

        /// <summary>
        /// The cached local layout bounds of the entire text block.
        /// </summary>
        private RectangleF _layoutBounds = RectangleF.Empty;
        
        /// <summary>
        /// Flag indicating whether the text markup string needs to be parsed again.
        /// </summary>
        private bool _markupDirty = true;

        /// <summary>
        /// Flag indicating whether the glyph layout needs to be rebuilt.
        /// </summary>
        private bool _layoutDirty = true;
        
        /// <summary>
        /// The cached font revision to track external font changes.
        /// </summary>
        private int _fontRevision = -1;

        /// <summary>
        /// The maximum line width before word-wrapping occurs.
        /// </summary>
        private int _maxWidth = int.MaxValue;

        /// <summary>
        /// The vertical distance applied when jumping to a new line.
        /// </summary>
        private int _jumpHeight = 20;
        
        /// <summary>
        /// The total accumulated time used for driving layout animations (e.g., shake).
        /// </summary>
        private float _time;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class with a font and a text content.
        /// </summary>
        /// <param name="font">The bitmap font used for rendering.</param>
        /// <param name="text">The raw text content, which may contain markup.</param>
        public Text(BitmapFont font, string text)
            : this(font, text, Vector2.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class with specific X and Y coordinates.
        /// </summary>
        /// <param name="font">The bitmap font used for rendering.</param>
        /// <param name="text">The raw text content, which may contain markup.</param>
        /// <param name="x">The X position of the text.</param>
        /// <param name="y">The Y position of the text.</param>
        public Text(BitmapFont font, string text, int x, int y)
            : this(font, text, new Vector2(x, y))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Text"/> class at the given position.
        /// </summary>
        /// <param name="font">The bitmap font used for rendering.</param>
        /// <param name="text">The raw text content, which may contain markup.</param>
        /// <param name="position">The global position of the text.</param>
        public Text(BitmapFont font, string text, Vector2 position)
        {
            _font = font;
            _text = text ?? string.Empty;
            Position = position;
            _markupDirty = true;
            _layoutDirty = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the font used by the text instance. Changing this invalidates the layout.
        /// </summary>
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
        /// Gets or sets the raw text content. Changing this property invalidates parsed markup and cached layout.
        /// </summary>
        public string TextString
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
        /// Gets or sets the global position of the text block.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the rotation applied to the full text block, in radians.
        /// </summary>
        public float Rotation { get; set; } = 0f;

        /// <summary>
        /// Gets or sets the origin used by rotation, expressed in local text layout space.
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets or sets the maximum amount of characters to be rendered, supporting typewriter-style animations.
        /// </summary>
        public int LengthSeeing { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets the total string length of the raw text, including all markup tags.
        /// </summary>
        public int Length => _text.Length;

        /// <summary>
        /// Gets the total number of characters in the text ignoring the inner markup content.
        /// </summary>
        public int LengthWithoutMarkup
        {
            get
            {
                int length = 0;
                foreach (TextRun run in _runs)
                    length += run.Length;

                return length;
            }
        }

        /// <summary>
        /// Gets or sets the default base color of the text. Changing this invalidates markup style data.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the default base scale of the text. Changing this invalidates markup style data.
        /// </summary>
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
        /// Gets the last computed local bounds of the text layout.
        /// </summary>
        public RectangleF Bounds
        {
            get
            {
                EnsureLayout();
                return _layoutBounds;
            }
        }

        /// <summary>
        /// Gets the width of the computed text block bounded layout.
        /// </summary>
        public int Width
        {
            get
            {
                EnsureLayout();
                return (int)Math.Ceiling(_layoutBounds.Width);
            }
        }

        /// <summary>
        /// Gets the height of the computed text block bounded layout.
        /// </summary>
        public int Height
        {
            get
            {
                EnsureLayout();
                return (int)Math.Ceiling(_layoutBounds.Height);
            }
        }

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
        /// Gets or sets the maximum line width before wrapping text into a new line.
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
        /// Gets or sets the vertical block distance added for each new line break.
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

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the text logic, specifically processing time to drive animation effects.
        /// </summary>
        /// <param name="gameTime">The current game time.</param>
        public void Update(GameTime gameTime)
        {
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Draws the cached layout of glyphs to the given SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance used for rendering.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (spriteBatch == null || Font == null) return;

            EnsureLayout();

            if (_glyphs.Count == 0)
                return;

            Matrix rotationMatrix = Rotation != 0f ? Matrix.CreateRotationZ(Rotation) : Matrix.Identity;

            float shakeTime = _time * 25f;

            for (int i = 0; i < Math.Min(_glyphs.Count, LengthSeeing); i++)
            {
                GlyphRenderData glyph = _glyphs[i];
                Vector2 localPosition = glyph.Position;

                if (glyph.ShakeAmplitude > 0f)
                {
                    localPosition.X += (float)Math.Sin(shakeTime + i) * glyph.ShakeAmplitude;
                    localPosition.Y += (float)Math.Cos(shakeTime * 1.2f + i) * glyph.ShakeAmplitude;
                }

                if (Rotation != 0f)
                    localPosition = Vector2.Transform(localPosition - Origin, rotationMatrix) + Origin;

                spriteBatch.Draw(
                    glyph.Character.TextureRegion.Texture,
                    Position + localPosition,
                    glyph.Character.TextureRegion.SourceRectangle,
                    glyph.Color,
                    Rotation,
                    Vector2.Zero,
                    glyph.Scale,
                    SpriteEffects.None,
                    0f);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks bounds and invalidation flags, processing markup and layout constraints if necessary.
        /// </summary>
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

        /// <summary>
        /// Reads the text string to create styling blocks (runs) based on bracketed metadata markup tags.
        /// </summary>
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

        /// <summary>
        /// Attempts to parse a markup string into a distinct format modifier to push onto the styling stack.
        /// </summary>
        /// <param name="tag">The markup tag text encapsulated inside the brackets.</param>
        /// <param name="stack">The current style stack keeping track of document nested styling contexts.</param>
        /// <param name="currentStyle">Throws out the updated text format style generated up to this tag's application.</param>
        /// <returns>True if the markup tag was valid and successfully consumed, false otherwise.</returns>
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

            string[] parts = tag.Split([' ', '\t', ','], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            string name = parts[0].Trim();
            TextStyleState next = stack.Peek().Style;

            // Remove the first part, which is the name of the tag, and pass the rest as
            parts = parts[1..];

            switch (name.ToLowerInvariant())
            {
                case "color":
                    if (parts.Length < 1 || !Parsing.TryParseColor(parts[0], out Color color))
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
                    if (!Parsing.TryParseFloat(parts[0], out float amplitude))
                        return false;

                    next.ShakeAmplitude = amplitude;
                    stack.Push(new StyleFrame { Name = name, Style = next });
                    currentStyle = next;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Caches character glyph mapping, establishes physical placement layouts matching style runs, aligns dimensions, and finalizes measurements.
        /// </summary>
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
            
            // Baseline for the current line
            float currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;

            int wordStartIndex = 0;
            float wordStartPenX = 0f;

            BitmapFontCharacter previousCharacter = null;

            // List to accurately track the index (in _glyphs) where each line starts
            List<int> lineStarts = [0];

            // 1. Generation and word-wrapping management
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

                    // Manual line break
                    if (ch == '\n')
                    {
                        penX = 0f;
                        penY += _jumpHeight * _defaultScale.Y;
                        currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;
                        previousCharacter = null;
                        
                        wordStartIndex = _glyphs.Count;
                        wordStartPenX = penX;

                        // Mark the start of this new line (if not a duplicate)
                        if (lineStarts[^1] != _glyphs.Count)
                            lineStarts.Add(_glyphs.Count);
                            
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

                    // Automatic word-wrapping (transfer word to a new line)
                    if (!isWhitespace && x + width > _maxWidth && wordStartPenX > 0f)
                    {
                        float shiftX = wordStartPenX;
                        float shiftY = _jumpHeight * _defaultScale.Y;

                        penY += shiftY;
                        currentLineBaseline = penY + Font.Baseline * _defaultScale.Y;

                        // Shift the glyphs of the current word
                        for (int j = wordStartIndex; j < _glyphs.Count; j++)
                        {
                            GlyphRenderData g = _glyphs[j];
                            g.Position.X -= shiftX;
                            g.Position.Y += shiftY;
                            _glyphs[j] = g;
                        }

                        // The starting point of the word is exactly the start of our new line
                        if (lineStarts[^1] != wordStartIndex)
                            lineStarts.Add(wordStartIndex);

                        penX -= shiftX;
                        x -= shiftX;
                        y += shiftY;

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

                    if (isWhitespace)
                    {
                        wordStartIndex = _glyphs.Count;
                        wordStartPenX = penX; 
                    }
                }
            }

            if (_glyphs.Count == 0)
            {
                _layoutBounds = RectangleF.Empty;
                _layoutDirty = false;
                return;
            }

            // Close the last line to simplify loop logic
            lineStarts.Add(_glyphs.Count);

            // 2. Calculate the global vertical offset (concerning the entire text block)
            float globalMinY = float.MaxValue;
            float globalMaxY = float.MinValue;

            for (int i = 0; i < _glyphs.Count; i++)
            {
                float top = _glyphs[i].Position.Y;
                float bottom = top + _glyphs[i].Character.TextureRegion.Height * _glyphs[i].Scale.Y;

                if (top < globalMinY) globalMinY = top;
                if (bottom > globalMaxY) globalMaxY = bottom;
            }

            float blockBaseline = penY + Font.Baseline * _defaultScale.Y;
            float alignOffsetY = 0f;

            switch (VerticalAlignment)
            {
                case TextVerticalAlignment.Middle:
                    alignOffsetY = globalMinY + (globalMaxY - globalMinY) * 0.5f;
                    break;
                case TextVerticalAlignment.Bottom:
                    alignOffsetY = blockBaseline;
                    break;
            }

            // 3. Align each line individually on the X axis (Horizontal) and apply Y axis
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            for (int lineIndex = 0; lineIndex < lineStarts.Count - 1; lineIndex++)
            {
                int start = lineStarts[lineIndex];
                int end = lineStarts[lineIndex + 1];

                if (start >= end)
                    continue;

                // Calculate the exact dimensions of the CURRENT LINE
                float lineMinX = float.MaxValue;
                float lineMaxX = float.MinValue;

                for (int i = start; i < end; i++)
                {
                    GlyphRenderData g = _glyphs[i];
                    float left = g.Position.X;
                    float right = left + g.Character.TextureRegion.Width * g.Scale.X;

                    if (left < lineMinX) lineMinX = left;
                    if (right > lineMaxX) lineMaxX = right;
                }

                // Determine how much to offset this line
                float lineAlignOffsetX = 0f;

                switch (HorizontalAlignment)
                {
                    case TextHorizontalAlignment.Center:
                        // Place its perfect center on the X Origin
                        lineAlignOffsetX = lineMinX + (lineMaxX - lineMinX) * 0.5f;
                        break;
                    case TextHorizontalAlignment.Right:
                        // Place the right side of the text on the X Origin
                        lineAlignOffsetX = lineMaxX;
                        break;
                }

                // Apply offsets and deduce the final bounding box (Bounds) of the whole block
                for (int i = start; i < end; i++)
                {
                    GlyphRenderData g = _glyphs[i];
                    g.Position.X -= lineAlignOffsetX;
                    g.Position.Y -= alignOffsetY;
                    _glyphs[i] = g;

                    float left = g.Position.X;
                    float top = g.Position.Y;
                    float right = left + g.Character.TextureRegion.Width * g.Scale.X;
                    float bottom = top + g.Character.TextureRegion.Height * g.Scale.Y;

                    if (left < minX) minX = left;
                    if (top < minY) minY = top;
                    if (right > maxX) maxX = right;
                    if (bottom > maxY) maxY = bottom;
                }
            }

            // Faithfully update the Bounding Box
            _layoutBounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            _layoutDirty = false;
        }

        #endregion
    }
}
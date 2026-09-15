using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics.BitmapFonts
{
    /// <summary>
    /// Represents a character in a bitmap font. This class cannot be inherited.
    /// </summary>
    public sealed class BitmapFontCharacter
    {
        #region Properties

        /// <summary>
        /// Gets the Unicode code point this glyph represents.
        /// </summary>
        public int Character { get; }

        /// <summary>
        /// Gets the texture page that contains this glyph's image.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the region within <see cref="Texture"/> that contains this glyph's image.
        /// </summary>
        public Rectangle Rectangle { get; }

        /// <summary>
        /// Gets the horizontal offset, in pixels, applied to the pen position when rendering the character.
        /// </summary>
        public int XOffset { get; }

        /// <summary>
        /// Gets the vertical offset, in pixels, from the top of the line to the top of the glyph image.
        /// </summary>
        public int YOffset { get; }

        /// <summary>
        /// Gets the horizontal distance, in pixels, the pen advances after rendering this character.
        /// </summary>
        public int XAdvance { get; }

        /// <summary>
        /// Gets the kerning adjustments that apply when this character is followed by another.
        /// </summary>
        /// <remarks>
        /// Keyed by the code point of the <em>following</em> character; the value is the extra
        /// horizontal offset, in pixels, applied before that character is drawn. The dictionary is
        /// created empty and is expected to be populated by the font loader after construction.
        /// </remarks>
        public Dictionary<int, int> Kernings { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapFontCharacter"/> class.
        /// </summary>
        /// <param name="character">The Unicode code point this glyph represents.</param>
        /// <param name="texture">The texture page that contains the character's image.</param>
        /// <param name="rectangle">The region within <paramref name="texture"/> that contains the character's image.</param>
        /// <param name="xOffset">The horizontal offset, in pixels, applied to the pen position when rendering the character.</param>
        /// <param name="yOffset">The vertical offset, in pixels, from the top of the line to the top of the glyph image.</param>
        /// <param name="xAdvance">The horizontal distance, in pixels, the pen advances after rendering this character.</param>
        /// <exception cref="ArgumentNullException"><paramref name="texture"/> is <see langword="null"/>.</exception>
        public BitmapFontCharacter(int character, Texture2D texture, Rectangle rectangle, int xOffset, int yOffset, int xAdvance)
        {
            ArgumentNullException.ThrowIfNull(texture);

            Character = character;
            Texture = texture;
            Rectangle = rectangle;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
            Kernings = [];
        }

        #endregion
    }
}
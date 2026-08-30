using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FreedomEngine.Graphics.BitmapFonts
{
    /// <summary>
    /// Represents a character in a bitmap font. This class cannot be inherited.
    /// </summary>
    public sealed class BitmapFontCharacter
    {
        #region Properties

        /// <summary>
        /// Gets the character code.
        /// </summary>
        public int Character { get; }

        public Texture2D Texture { get; }

        public Rectangle Rectangle { get; }

        /// <summary>
        /// Gets the horizontal offset for rendering the character.
        /// </summary>
        public int XOffset { get; }

        /// <summary>
        /// Gets the vertical offset for rendering the character.
        /// </summary>
        public int YOffset { get; }

        /// <summary>
        /// Gets the horizontal advance value for rendering the next character.
        /// </summary>
        public int XAdvance { get; }

        /// <summary>
        /// Gets the dictionary of kerning values for pairs of characters.
        /// </summary>
        public Dictionary<int, int> Kernings { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapFontCharacter"/> class.
        /// </summary>
        /// <param name="character">The character code.</param>
        /// <param name="textureRegion">The texture region that contains the character's image.</param>
        /// <param name="xOffset">The horizontal offset for rendering the character.</param>
        /// <param name="yOffset">The vertical offset for rendering the character.</param>
        /// <param name="xAdvance">The horizontal advance value for rendering the next character.</param>
        public BitmapFontCharacter(int character, Texture2D texture, Rectangle rectangle, int xOffset, int yOffset, int xAdvance)
        {
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
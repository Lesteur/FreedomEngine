using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a rectangular region within a texture.
    /// </summary>
    public sealed class TextureRegion
    {
        #region Properties

        /// <summary>
        /// Gets the source texture this texture region is part of.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the source rectangle boundary of this texture region within the source texture.
        /// </summary>
        public Rectangle SourceRectangle { get; }

        /// <summary>
        /// Gets the width, in pixels, of this texture region.
        /// </summary>
        public int Width => SourceRectangle.Width;

        /// <summary>
        /// Gets the height, in pixels, of this texture region.
        /// </summary>
        public int Height => SourceRectangle.Height;

        /// <summary>
        /// Gets the top normalized texture coordinate of this region.
        /// </summary>
        public float TopTextureCoordinate => SourceRectangle.Top / (float)Texture.Height;

        /// <summary>
        /// Gets the bottom normalized texture coordinate of this region.
        /// </summary>
        public float BottomTextureCoordinate => SourceRectangle.Bottom / (float)Texture.Height;

        /// <summary>
        ///  Gets the left normalized texture coordinate of this region.
        /// </summary>
        public float LeftTextureCoordinate => SourceRectangle.Left / (float)Texture.Width;

        /// <summary>
        /// Gets the right normalized texture coordinate of this region.
        /// </summary>
        public float RightTextureCoordinate => SourceRectangle.Right / (float)Texture.Width;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="TextureRegion"/> class using the specified source texture and source rectangle parameters.
        /// </summary>
        /// <param name="texture">The texture to use as the source texture for this texture region.</param>
        /// <param name="x">The x-coordinate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
        /// <param name="y">The y-coordinate position of the upper-left corner of this texture region relative to the upper-left corner of the source texture.</param>
        /// <param name="width">The width, in pixels, of this texture region.</param>
        /// <param name="height">The height, in pixels, of this texture region.</param>
        public TextureRegion(Texture2D texture, int x = 0, int y = 0, int width = 0, int height = 0)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture), "Source texture cannot be null.");

            // Ensure the source rectangle is within the bounds of the source texture
            if (x < 0 || y < 0 || x + width > texture.Width || y + height > texture.Height)
                throw new ArgumentException("The source rectangle must be within the bounds of the source texture.");

            Texture = texture;
            SourceRectangle = new Rectangle(x, y, width, height);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Submit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            // Chain to the most complex overload
            Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Submit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region on screen.</param>
        /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region on screen.</param>
        /// <param name="scale">The scale factor to apply when drawing this texture region on screen.</param>
        /// <param name="effects">Specifies if this texture region should be flipped horizontally, vertically, or both when drawing on screen.</param>
        /// <param name="layerDepth">The depth of the layer to use when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            // Convert uniform scale to Vector2 scale and chain
            Draw(spriteBatch, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        /// <summary>
        /// Submit this texture region for drawing in the current batch.
        /// </summary>
        /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
        /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
        /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
        /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region on screen.</param>
        /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region on screen.</param>
        /// <param name="scale">The amount of scaling to apply to the x- and y-axes when drawing this texture region on screen.</param>
        /// <param name="effects">Specifies if this texture region should be flipped horizontally, vertically, or both when drawing on screen.</param>
        /// <param name="layerDepth">The depth of the layer to use when drawing this texture region on screen.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            spriteBatch.Draw(
                Texture,
                position,
                SourceRectangle,
                color,
                rotation,
                origin,
                scale,
                effects,
                layerDepth
            );
        }

        #endregion
    }
}
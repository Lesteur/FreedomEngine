using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of named texture regions and sprites.
    /// </summary>
    public sealed class TextureAtlas
    {
        #region Fields

        /// <summary>
        /// Stores the mapping of texture region names to their corresponding <see cref="TextureRegion"/> objects.
        /// </summary>
        private readonly Dictionary<string, TextureRegion> _regions;

        /// <summary>
        /// Stores the mapping of sprite names to their corresponding <see cref="Sprite"/> objects.
        /// </summary>
        private readonly Dictionary<string, Sprite> _sprites;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source texture represented by this texture atlas.
        /// </summary>
        public Texture2D Texture { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="TextureAtlas"/> class using the specified source texture.
        /// </summary>
        /// <param name="texture">The source texture to be used for this texture atlas.</param>
        public TextureAtlas(Texture2D texture)
        {
            if (texture == null)
                throw new ArgumentNullException(nameof(texture), "The source texture cannot be null.");

            Texture = texture;

            _regions = [];
            _sprites = [];
        }

        #endregion

        #region Public Methods (Regions)

        /// <summary>
        /// Creates a new region and adds it to this texture atlas.
        /// </summary>
        /// <param name="name">The name to give the texture region.</param>
        /// <param name="region">The texture region to add.</param>
        public void AddRegion(string name, TextureRegion region)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The name cannot be null or empty.");

            _regions.Add(name, region);
        }

        /// <summary>
        /// Gets the region from this texture atlas with the specified name.
        /// </summary>
        /// <param name="name">The name of the region to retrieve.</param>
        /// <returns>The TextureRegion with the specified name.</returns>
        public TextureRegion GetRegion(string name)
        {
            return _regions[name];
        }

        /// <summary>
        /// Removes the region from this texture atlas with the specified name.
        /// </summary>
        /// <param name="name">The name of the region to remove.</param>
        /// <returns></returns>
        public bool RemoveRegion(string name)
        {
            return _regions.Remove(name);
        }

        #endregion

        #region Public Methods (Sprites)

        /// <summary>
        /// Adds the given sprite to this texture atlas with the specified name.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to add.</param>
        /// <param name="sprite">The sprite to add.</param>
        public void AddSprite(string name, Sprite sprite)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name), "The name cannot be null or empty.");

            if (sprite == null)
                throw new ArgumentNullException(nameof(sprite), "The sprite cannot be null.");

            _sprites.Add(name, sprite);
        }

        /// <summary>
        /// Gets the sprite from this texture atlas with the specified name.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to retrieve.</param>
        /// <returns>The sprite with the specified name.</returns>
        public Sprite GetSprite(string spriteName)
        {
            return _sprites[spriteName];
        }

        /// <summary>
        /// Removes the sprite with the specified name from this texture atlas.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to remove.</param>
        /// <returns>true if the sprite is removed successfully; otherwise, false.</returns>
        public bool RemoveSprite(string spriteName)
        {
            return _sprites.Remove(spriteName);
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Removes all texture regions and sprites from this texture atlas.
        /// </summary>
        public void Clear()
        {
            _regions.Clear();
            _sprites.Clear();
        }

        #endregion
    }
}
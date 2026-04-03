using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

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

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="TextureAtlas"/> class with no source texture.
        /// </summary>
        public TextureAtlas()
        {
            _regions = [];
            _sprites = [];
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TextureAtlas"/> class using the specified source texture.
        /// </summary>
        /// <param name="texture">The source texture to be used for this texture atlas.</param>
        public TextureAtlas(Texture2D texture) : this()
        {
            Texture = texture;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source texture represented by this texture atlas.
        /// </summary>
        public Texture2D Texture { get; set; }

        #endregion

        #region Public Methods (Regions)

        /// <summary>
        /// Creates a new region and adds it to this texture atlas.
        /// </summary>
        public void AddRegion(string name, ushort x, ushort y, ushort width, ushort height)
        {
            if (Texture == null)
                throw new InvalidOperationException("Cannot add a region to an atlas without a source texture.");

            TextureRegion region = new(Texture, x, y, width, height);
            _regions.Add(name, region);
        }

        /// <summary>
        /// Gets the region from this texture atlas with the specified name.
        /// </summary>
        public TextureRegion GetRegion(string name)
        {
            return _regions[name];
        }

        /// <summary>
        /// Removes the region from this texture atlas with the specified name.
        /// </summary>
        public bool RemoveRegion(string name)
        {
            return _regions.Remove(name);
        }

        #endregion

        #region Public Methods (Sprites)

        /// <summary>
        /// Adds the given sprite definition to this texture atlas mapped by the specified name.
        /// </summary>
        public void AddSprite(string spriteName, Sprite sprite)
        {
            _sprites.Add(spriteName, sprite);
        }

        /// <summary>
        /// Retrieves a recognized sprite sequence mapped within this texture atlas by name.
        /// </summary>
        public Sprite GetSprite(string spriteName)
        {
            return _sprites[spriteName];
        }

        /// <summary>
        /// Removes the sprite object bound to the specified name from this texture atlas.
        /// </summary>
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
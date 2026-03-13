using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of named texture regions and sprites, enabling efficient organization and retrieval of
    /// graphical assets from a single source texture.
    /// </summary>
    public class TextureAtlas
    {
        /// <summary>
        /// Stores the mapping of texture region names to their corresponding TextureRegion objects.
        /// </summary>
        private readonly Dictionary<string, TextureRegion> _regions;

        /// <summary>
        /// Stores the mapping of sprite names to their corresponding Sprite objects.
        /// </summary>
        private readonly Dictionary<string, Sprite> _sprites;


        /// <summary>
        /// Gets or Sets the source texture represented by this texture atlas.
        /// </summary>
        public Texture2D Texture { get; set; }


        /// <summary>
        /// Creates a new texture atlas.
        /// </summary>
        public TextureAtlas()
        {
            _regions = [];
            _sprites = [];
        }

        /// <summary>
        /// Creates a new texture atlas instance using the given texture.
        /// </summary>
        /// <param name="texture">The source texture represented by the texture atlas.</param>
        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regions = [];
            _sprites = [];
        }


        /// <summary>
        /// Creates a new region and adds it to this texture atlas.
        /// </summary>
        /// <param name="name">The name to give the texture region.</param>
        /// <param name="x">The top-left x-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
        /// <param name="y">The top-left y-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
        /// <param name="width">The width, in pixels, of the region.</param>
        /// <param name="height">The height, in pixels, of the region.</param>
        public void AddRegion(string name, UInt16 x, UInt16 y, UInt16 width, UInt16 height)
        {
            TextureRegion region = new(Texture, x, y, width, height);
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
        /// <returns>true if the region was successfully removed; otherwise, false.</returns>
        public bool RemoveRegion(string name)
        {
            return _regions.Remove(name);
        }

        /// <summary>
        /// Adds the given sprite definition to this texture atlas mapped by the specified name.
        /// </summary>
        /// <param name="spriteName">The name to index the sprite under.</param>
        /// <param name="sprite">The sprite instance containing frame details.</param>
        public void AddSprite(string spriteName, Sprite sprite)
        {
            _sprites.Add(spriteName, sprite);
        }

        /// <summary>
        /// Retrieves a recognized sprite sequence mapped within this texture atlas by name.
        /// </summary>
        /// <param name="spriteName">The key name of the Sprite object.</param>
        /// <returns>The Sprite mapped to the specified name.</returns>
        public Sprite GetSprite(string spriteName)
        {
            return _sprites[spriteName];
        }

        /// <summary>
        /// Removes the sprite object bound to the specified name from this texture atlas.
        /// </summary>
        /// <param name="spriteName">The name indicating the target sprite mapping within the atlas.</param>
        /// <returns>true if the bounding mapping is removed successfully; otherwise, false.</returns>
        public bool RemoveSprite(string spriteName)
        {
            return _sprites.Remove(spriteName);
        }

        /// <summary>
        /// Removes all texture regions and sprites from this texture atlas.
        /// </summary>
        public void Clear()
        {
            _regions.Clear();
            _sprites.Clear();
        }
    }
}

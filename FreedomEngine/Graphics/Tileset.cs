using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of tiles, including their textures and animations, used for rendering in a grid layout.
    /// </summary>
    public sealed class Tileset
    {
        #region Properties

        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the collection of texture regions that represent the individual tiles in the tileset.
        /// </summary>
        public Rectangle[] Tiles { get; }

        /// <summary>
        /// Gets the dictionary containing tile animations, mapped by their ID.
        /// </summary>
        public Dictionary<ushort, TileAnimation> Animations { get; }

        /// <summary>
        /// Gets the width of a single tile in pixels.
        /// </summary>
        public ushort TileWidth { get; }

        /// <summary>
        /// Gets the height of a single tile in pixels.
        /// </summary>
        public ushort TileHeight { get; }

        /// <summary>
        /// Gets the total number of columns in this tileset.
        /// </summary>
        public ushort Columns { get; }

        /// <summary>
        /// Gets the total number of rows in this tileset.
        /// </summary>
        public ushort Rows { get; }

        /// <summary>
        /// Gets the total count of tiles within the tileset.
        /// </summary>
        public ushort Count { get; }

        #endregion

        #region Constructors

        public Tileset(Texture2D texture, Rectangle[] tiles, Dictionary<ushort, TileAnimation> animations)
        {
            Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");

            if (tiles == null || tiles.Length == 0)
                throw new ArgumentException("Tiles collection cannot be null or empty.", nameof(tiles));

            Tiles = tiles;
            TileWidth = (ushort)tiles[0].Width;
            TileHeight = (ushort)tiles[0].Height;
            Columns = (ushort)(texture.Width / TileWidth);
            Rows = (ushort)(texture.Height / TileHeight);
            Count = (ushort)tiles.Length;
            Animations = animations;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given index.
        /// </summary>
        /// <param name="index">The index of the texture region in this tile set.</param>
        /// <returns>The texture region for the tile form this tileset at the given index.</returns>
        public Rectangle GetTile(int index) => Tiles[index];

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given location.
        /// </summary>
        /// <param name="column">The column in this tileset of the texture region.</param>
        /// <param name="row">The row in this tileset of the texture region.</param>
        /// <returns>The texture region for the tile from this tileset at given location.</returns>
        public Rectangle GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }

        /// <summary>
        /// Adds a mono-delay tile animation sequence mapping.
        /// </summary>
        public void AddAnimation(ushort tileID, ushort[] frameTilesIDs, TimeSpan delay)
        {
            Animations[tileID] = new TileAnimation(frameTilesIDs, delay);
        }

        /// <summary>
        /// Adds a variable-delay tile animation sequence mapping.
        /// </summary>
        public void AddAnimation(ushort tileID, ushort[] frameTilesIDs, TimeSpan[] delays)
        {
            Animations[tileID] = new TileAnimation(frameTilesIDs, delays);
        }

        public void RemoveAnimation(ushort tileID)
        {
            Animations.Remove(tileID);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of tiles, including their textures and animations, used for rendering in a grid layout.
    /// </summary>
    public class Tileset
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class using a base <see cref="TextureRegion"/>.
        /// Parses the texture into a grid of tiles based on provided dimensions and margins.
        /// </summary>
        public Tileset(TextureRegion textureRegion, ushort tileWidth, ushort tileHeight, ushort xMargin = 0, ushort yMargin = 0, ushort xSpacing = 0, ushort ySpacing = 0)
        {
            Animations = [];
            Delay = TimeSpan.FromSeconds(0.125);
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            int availableWidth = textureRegion.Width - (2 * xMargin);
            int availableHeight = textureRegion.Height - (2 * yMargin);

            Columns = (ushort)((availableWidth + xSpacing) / (tileWidth + xSpacing));
            Rows = (ushort)((availableHeight + ySpacing) / (tileHeight + ySpacing));
            Count = (ushort)(Columns * Rows);

            Tiles = new TextureRegion[Count];

            int index = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    int x = xMargin + col * (tileWidth + xSpacing);
                    int y = yMargin + row * (tileHeight + ySpacing);

                    Tiles[index] = new TextureRegion(textureRegion.Texture, x, y, tileWidth, tileHeight);
                    index++;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of texture regions that represent the individual tiles in the tileset.
        /// </summary>
        public TextureRegion[] Tiles { get; }

        /// <summary>
        /// Gets the dictionary containing tile animations, mapped by their ID.
        /// </summary>
        public Dictionary<ushort, List<ushort>> Animations { get; }

        /// <summary>
        /// Gets the default delay between frames for tile animations.
        /// </summary>
        public TimeSpan Delay { get; }

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

        #region Public Methods

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given index.
        /// </summary>
        public TextureRegion GetTile(int index) => Tiles[index];

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given location.
        /// </summary>
        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }

        /// <summary>
        /// Adds a tile animation sequence mapping.
        /// </summary>
        public void AddAnimation(ushort tileID, List<ushort> frameTilesIDs)
        {
            Animations[tileID] = frameTilesIDs;
        }

        /// <summary>
        /// Removes a tile animation from the tileset definitions.
        /// </summary>
        public void RemoveAnimation(ushort tileID)
        {
            Animations.Remove(tileID);
        }

        /// <summary>
        /// Gets the tile ID of the current frame of the animation for the given tile ID based on the elapsed time.
        /// </summary>
        public ushort TryGetAnimation(ushort tileID, TimeSpan elapsed)
        {
            if (Animations.TryGetValue(tileID, out List<ushort> frameTileIDs))
            {
                int frameIndex = (int)(elapsed.TotalSeconds / Delay.TotalSeconds) % frameTileIDs.Count;
                return frameTileIDs[frameIndex];
            }

            return tileID;
        }

        #endregion
    }
}
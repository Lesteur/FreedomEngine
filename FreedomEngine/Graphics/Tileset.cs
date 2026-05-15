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

        /// <summary>
        /// Gets the collection of texture regions that represent the individual tiles in the tileset.
        /// </summary>
        public TextureRegion[] Tiles { get; }

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

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class using a base <see cref="TextureRegion"/>.
        /// Parses the texture into a grid of tiles based on provided dimensions and margins.
        /// </summary>
        /// <param name="textureRegion">The texture region that contains the tiles for the tileset.</param>
        /// <param name="tileWidth">The width of each tile in the tileset.</param>
        /// <param name="tileHeight">The height of each tile in the tileset.</param>
        /// <param name="xMargin">The horizontal margin in pixels between the edge of the texture and the first column of tiles.</param>
        /// <param name="yMargin">The vertical margin in pixels between the edge of the texture and the first row of tiles.</param>
        /// <param name="xSpacing">The horizontal spacing in pixels between adjacent tiles.</param>
        /// <param name="ySpacing">The vertical spacing in pixels between adjacent tiles.</param>
        public Tileset(TextureRegion textureRegion, ushort tileWidth, ushort tileHeight, ushort xMargin = 0, ushort yMargin = 0, ushort xSpacing = 0, ushort ySpacing = 0)
        {
            if (textureRegion == null)
                throw new ArgumentNullException(nameof(textureRegion), "Texture region cannot be null.");

            Animations = new Dictionary<ushort, TileAnimation>();
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

        #region Public Methods

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given index.
        /// </summary>
        /// <param name="index">The index of the texture region in this tile set.</param>
        /// <returns>The texture region for the tile form this tileset at the given index.</returns>
        public TextureRegion GetTile(int index) => Tiles[index];

        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given location.
        /// </summary>
        /// <param name="column">The column in this tileset of the texture region.</param>
        /// <param name="row">The row in this tileset of the texture region.</param>
        /// <returns>The texture region for the tile from this tileset at given location.</returns>
        public TextureRegion GetTile(int column, int row)
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
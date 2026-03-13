using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of tiles, including their textures and animations, used for rendering in a grid layout.
    /// </summary>
    public class Tileset
    {
        /// <summary>
        /// Gets the collection of texture regions that represent the individual tiles in the tileset.
        /// </summary>
        public TextureRegion[] Tiles { get; }

        /// <summary>
        /// Gets the dictionary containing tile animations, mapped by their ID.
        /// </summary>
        public Dictionary<UInt16, List<UInt16>> Animations { get; }

        /// <summary>
        /// Gets the default delay between frames for tile animations.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the width of a single tile in pixels.
        /// </summary>
        public UInt16 TileWidth { get; }

        /// <summary>
        /// Gets the height of a single tile in pixels.
        /// </summary>
        public UInt16 TileHeight { get; }

        /// <summary>
        /// Gets the total number of columns in this tileset.
        /// </summary>
        public UInt16 Columns { get; }

        /// <summary>
        /// Gets the total number of rows in this tileset.
        /// </summary>
        public UInt16 Rows { get; }

        /// <summary>
        /// Gets the total count of tiles within the tileset.
        /// </summary>
        public UInt16 Count { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class using a base <see cref="TextureRegion"/>.
        /// Parses the texture into a grid of tiles based on provided dimensions and margins.
        /// </summary>
        /// <param name="textureRegion">The base texture area representing the complete tileset.</param>
        /// <param name="tileWidth">The width of a single tile.</param>
        /// <param name="tileHeight">The height of a single tile.</param>
        /// <param name="xMargin">Horizontal margin offset in pixels.</param>
        /// <param name="yMargin">Vertical margin offset in pixels.</param>
        /// <param name="xSpacing">Horizontal spacing between adjacent tiles.</param>
        /// <param name="ySpacing">Vertical spacing between adjacent tiles.</param>
        public Tileset(TextureRegion textureRegion, UInt16 tileWidth, UInt16 tileHeight, UInt16 xMargin = 0, UInt16 yMargin = 0, UInt16 xSpacing = 0, UInt16 ySpacing = 0)
        {
            Animations = [];
            Delay = TimeSpan.FromSeconds(0.125);
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            int availableWidth = textureRegion.Width - (2 * xMargin);
            int availableHeight = textureRegion.Height - (2 * yMargin);

            Columns = (UInt16)((availableWidth + xSpacing) / (tileWidth + xSpacing));
            Rows = (UInt16)((availableHeight + ySpacing) / (tileHeight + ySpacing));
            Count = (UInt16)(Columns * Rows);

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
        /// Adds a tile animation sequence mapping.
        /// </summary>
        /// <param name="tileID">The starting tile ID which triggers the animation.</param>
        /// <param name="frameTilesIDs">The sequence of tile IDs representing the frames.</param>
        public void AddAnimation(UInt16 tileID, List<UInt16> frameTilesIDs)
        {
            Animations[tileID] = frameTilesIDs;
        }

        /// <summary>
        /// Removes a tile animation from the tileset definitions.
        /// </summary>
        /// <param name="tileID">The starting tile ID of the animation to remove.</param>
        public void RemoveAnimation(UInt16 tileID)
        {
            Animations.Remove(tileID);
        }

        /// <summary>
        /// Gets the tile ID of the current frame of the animation for the given tile ID based on the elapsed time.
        /// If the given tile ID does not have an animation, the original tile ID is returned.
        /// </summary>
        /// <param name="tileID">The tile ID to get the animation for.</param>
        /// <param name="_elapsed">The elapsed time used to determine the current frame of the animation.</param>
        /// <returns>The tile ID of the current frame of the animation for the given tile ID based on the elapsed time, or the original tile ID if there is no animation.</returns>
        public UInt16 TryGetAnimation(UInt16 tileID, TimeSpan _elapsed)
        {
            if (Animations.TryGetValue(tileID, out List<UInt16> frameTileIDs))
            {
                int frameIndex = (int)(_elapsed.TotalSeconds / Delay.TotalSeconds) % frameTileIDs.Count;
                return frameTileIDs[frameIndex];
            }

            return tileID;
        }
    }
}
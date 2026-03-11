using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    public class Tileset
    {
        private readonly TextureRegion[] _tiles;

        private Dictionary<UInt16, List<UInt16>> _animations;

        private readonly TimeSpan _delay;

        private readonly UInt16 _tileWidth;

        private readonly UInt16 _tileHeight;

        private readonly UInt16 _columns;

        private readonly UInt16 _rows;

        private readonly UInt16 _count;


        public IReadOnlyDictionary<UInt16, List<UInt16>> Animations => _animations;

        public TimeSpan Delay => _delay;

        public UInt16 TileWidth => _tileWidth;

        public UInt16 TileHeight => _tileHeight;

        public UInt16 Columns => _columns;

        public UInt16 Rows => _rows;

        public UInt16 Count => _count;


        public Tileset(TextureRegion textureRegion, UInt16 tileWidth, UInt16 tileHeight, UInt16 xMargin = 0, UInt16 yMargin = 0, UInt16 xSpacing = 0, UInt16 ySpacing = 0)
        {
            _animations = [];

            _delay = TimeSpan.FromSeconds(0.1);

            _tileWidth = tileWidth;
            _tileHeight = tileHeight;

            _columns = 0;
            _rows = 0;
            _count = 0;

            int availableWidth = textureRegion.Width - (2 * xMargin);
            int availableHeight = textureRegion.Height - (2 * yMargin);

            _columns = (UInt16)((availableWidth + xSpacing) / (tileWidth + xSpacing));
            _rows = (UInt16)((availableHeight + ySpacing) / (tileHeight + ySpacing));

            _count = (UInt16)(_columns * _rows);

            _tiles = new TextureRegion[_count];

            int index = 0;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _columns; col++)
                {
                    int x = xMargin + col * (tileWidth + xSpacing);
                    int y = yMargin + row * (tileHeight + ySpacing);

                    _tiles[index] = new TextureRegion(textureRegion.Texture, x, y, tileWidth, tileHeight);
                    index++;
                }
            }
        }


        /// <summary>
        /// Gets the texture region for the tile from this tileset at the given index.
        /// </summary>
        /// <param name="index">The index of the texture region in this tile set.</param>
        /// <returns>The texture region for the tile form this tileset at the given index.</returns>
        public TextureRegion GetTile(int index) => _tiles[index];

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

        public void AddAnimation(UInt16 tileID, List<UInt16> frameTilesIDs)
        {
            _animations[tileID] = frameTilesIDs;
        }

        public void RemoveAnimation(UInt16 tileID)
        {
            _animations.Remove(tileID);
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
            if (_animations.TryGetValue(tileID, out List<UInt16> frameTileIDs))
            {
                int frameIndex = (int)(_elapsed.TotalSeconds / _delay.TotalSeconds) % frameTileIDs.Count;
                return frameTileIDs[frameIndex];
            }

            return tileID;
        }
    }
}
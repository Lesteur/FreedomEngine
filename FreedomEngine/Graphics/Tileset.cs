using System;

namespace FreedomEngine.Graphics
{
    public class Tileset
    {
        protected Texture _texture;

        protected UInt16 _tileWidth;

        protected UInt16 _tileHeight;

        protected UInt16 _xMargin;

        protected UInt16 _yMargin;

        protected UInt16 _xSpacing;

        protected UInt16 _ySpacing;


        public Texture Texture => _texture;

        public UInt16 TileWidth => _tileWidth;

        public UInt16 TileHeight => _tileHeight;

        public UInt16 XMargin => _xMargin;

        public UInt16 YMargin => _yMargin;

        public UInt16 XSpacing => _xSpacing;

        public UInt16 YSpacing => _ySpacing;


        public Tileset(Texture texture, UInt16 tileWidth, UInt16 tileHeight, UInt16 xMargin = 0, UInt16 yMargin = 0, UInt16 xSpacing = 0, UInt16 ySpacing = 0)
        {
            _texture = texture;
            _tileWidth = tileWidth;
            _tileHeight = tileHeight;
            _xMargin = xMargin;
            _yMargin = yMargin;
            _xSpacing = xSpacing;
            _ySpacing = ySpacing;
        }


        public int GetTilesPerRow()
        {
            int availableWidth = _texture.Width - (2 * _xMargin);
            return (availableWidth + _xSpacing) / (_tileWidth + _xSpacing);
        }

        public int GetTilesPerColumn()
        {
            int availableHeight = _texture.Height - (2 * _yMargin);
            return (availableHeight + _ySpacing) / (_tileHeight + _ySpacing);
        }

        public int GetTotalTileCount()
        {
            return GetTilesPerRow() * GetTilesPerColumn();
        }
    }
}
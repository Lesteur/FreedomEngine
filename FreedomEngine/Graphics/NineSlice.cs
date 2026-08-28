using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Graphics
{
    public class NineSlice
    {
        #region Fields

        private readonly TextureRegion[] _regions = new TextureRegion[9];

        private int _width;

        private int _height;

        #endregion

        #region Properties

        public Texture2D Texture { get; }

        public int Width
        {
            get => _width;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Width must be greater than zero.");
                _width = value;
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Height must be greater than zero.");
                _height = value;
            }
        }

        #endregion

        #region Constructors

        public NineSlice(TextureRegion region, int squareSize)
        {
            if (squareSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(squareSize), "Square size must be greater than zero.");

            if (region.Width / squareSize != 3 || region.Height / squareSize != 3)
                throw new ArgumentException("The provided texture region must be divisible into a 3x3 grid based on the specified square size.", nameof(region));

            Texture = region.Texture;

            _regions[0] = new TextureRegion(Texture, region.SourceRectangle.X, region.SourceRectangle.Y, squareSize, squareSize);
            _regions[1] = new TextureRegion(Texture, region.SourceRectangle.X + squareSize, region.SourceRectangle.Y, squareSize, squareSize);
            _regions[2] = new TextureRegion(Texture, region.SourceRectangle.X + region.SourceRectangle.Width - squareSize, region.SourceRectangle.Y, squareSize, squareSize);

            _regions[3] = new TextureRegion(Texture, region.SourceRectangle.X, region.SourceRectangle.Y + squareSize, squareSize, squareSize);
            _regions[4] = new TextureRegion(Texture, region.SourceRectangle.X + squareSize, region.SourceRectangle.Y + squareSize, squareSize, squareSize);
            _regions[5] = new TextureRegion(Texture, region.SourceRectangle.X + region.SourceRectangle.Width - squareSize, region.SourceRectangle.Y + squareSize, squareSize, squareSize);

            _regions[6] = new TextureRegion(Texture, region.SourceRectangle.X, region.SourceRectangle.Y + region.SourceRectangle.Height - squareSize, squareSize, squareSize);
            _regions[7] = new TextureRegion(Texture, region.SourceRectangle.X + squareSize, region.SourceRectangle.Y + region.SourceRectangle.Height - squareSize, squareSize, squareSize);
            _regions[8] = new TextureRegion(Texture, region.SourceRectangle.X + region.SourceRectangle.Width - squareSize, region.SourceRectangle.Y + region.SourceRectangle.Height - squareSize, squareSize, squareSize);
        }

        #endregion

        #region Public Methods

        #endregion
    }
}

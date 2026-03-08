using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Graphics
{
    public class Sprite
    {
        protected Texture _texture;

        protected UInt16 _frameCount;

        protected UInt16 _frameWidth;

        protected UInt16 _frameHeight;

        protected UInt16 _xOrigin;

        protected UInt16 _yOrigin;

        protected UInt16 _xMarging;

        protected UInt16 _yMarging;

        public Texture Texture => _texture;

        public UInt16 FrameCount => _frameCount;

        public UInt16 FrameWidth => _frameWidth;

        public UInt16 FrameHeight => _frameHeight;

        public UInt16 XOrigin => _xOrigin;

        public UInt16 YOrigin => _yOrigin;

        public UInt16 XMarging => _xMarging;

        public UInt16 YMarging => _yMarging;


        public Sprite(Texture texture)
        {
            ArgumentNullException.ThrowIfNull(texture);

            //if (frameCount <= 0)
            //    throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));

            _texture = texture;
            _frameCount = 1;

            _frameWidth = texture.Width;
            _frameHeight = texture.Height;

            _xOrigin = 0;
            _yOrigin = 0;

            _xMarging = 0;
            _yMarging = 0;
        }

        public Sprite(Texture texture, UInt16 frameCount, UInt16 frameWidth, UInt16 frameHeight, UInt16 xOrigin = 0, UInt16 yOrigin = 0, UInt16 xMarging = 0, UInt16 yMarging = 0)
        {
            ArgumentNullException.ThrowIfNull(texture);

            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));

            if (frameWidth <= 0 || frameHeight <= 0 || frameWidth > texture.Width || frameHeight > texture.Height)
                throw new ArgumentException("Frame width and height must be greater than zero and less than or equal to the texture dimensions.", nameof(frameWidth));

            _texture = texture;
            _frameCount = frameCount;

            _frameWidth = frameWidth;
            _frameHeight = frameHeight;

            _xOrigin = xOrigin;
            _yOrigin = yOrigin;

            _xMarging = xMarging;
            _yMarging = yMarging;
        }


        public Rectangle GetSourceRectangle(int frameNumber)
        {
            // Wrap frame number to prevent out-of-bounds access
            frameNumber %= _frameCount;

            return new Rectangle(
                frameNumber * _frameWidth + _xMarging,
                _yMarging,
                _frameWidth,
                _frameHeight
            );
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    public class Sprite
    {
        /// <summary>
        /// The texture regions that make up the frames of this animation. The order of the regions within the collection
        /// are the order that the frames should be displayed in.
        /// </summary>
        private readonly List<TextureRegion> _frames;

        /// <summary>
        /// The amount of time to delay between each frame before moving to the next frame for this animation.
        /// </summary>
        private readonly TimeSpan _delay;

        /// <summary>
        /// The x-coordinate position of the upper-left corner of this sprite relative to the upper-left corner of the sprite.
        /// </summary>
        private readonly UInt16 _xOrigin;

        /// <summary>
        /// The y-coordinate position of the upper-left corner of this sprite relative to the upper-left corner of the sprite.
        /// </summary>
        private readonly UInt16 _yOrigin;


        /// <summary>
        /// Gets the amount of time to delay between each frame before moving to the next frame for this animation.
        /// </summary>
        public List<TextureRegion> Frames => _frames;

        /// <summary>
        /// Gets the amount of time to delay between each frame before moving to the next frame for this animation.
        /// </summary>
        public TimeSpan Delay => _delay;

        /// <summary>
        /// Gets the x-coordinate position of the upper-left corner of this sprite relative to the upper-left corner of the sprite.
        /// </summary>
        public UInt16 XOrigin => _xOrigin;

        /// <summary>
        /// Gets the y-coordinate position of the upper-left corner of this sprite relative to the upper-left corner of the sprite.
        /// </summary>
        public UInt16 YOrigin => _yOrigin;


        public Sprite(Texture2D texture, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            TextureRegion region = new(texture, 0, 0, (UInt16) texture.Width, (UInt16) texture.Height);

            _frames = [];
            _frames.Add(region);

            _delay = TimeSpan.Zero;
            _xOrigin = xOrigin;
            _yOrigin = yOrigin;
        }

        /// <summary>
        /// Creates a new animation with the specified frames and delay.
        /// </summary>
        /// <param name="frames">An ordered collection of the frames for this animation.</param>
        /// <param name="delay">The amount of time to delay between each frame of this animation.</param>
        public Sprite(List<TextureRegion> frames, TimeSpan delay, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            if (frames == null || frames.Count == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));
            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            _frames = frames;
            _delay = delay;

            _xOrigin = xOrigin;
            _yOrigin = yOrigin;
        }

        public Sprite(Texture2D texture2D, UInt16 frameCount, TimeSpan delay, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            if (texture2D == null)
                throw new ArgumentNullException(nameof(texture2D), "Texture cannot be null.");
            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));
            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            _frames = [];
            UInt16 frameWidth = (UInt16) (texture2D.Width / frameCount);
            UInt16 frameHeight = (UInt16) texture2D.Height;

            for (int i = 0; i < frameCount; i++)
            {
                TextureRegion region = new(texture2D, i * frameWidth, 0, frameWidth, frameHeight);
                _frames.Add(region);
            }

            _delay = delay;

            _xOrigin = xOrigin;
            _yOrigin = yOrigin;
        }
    }
}
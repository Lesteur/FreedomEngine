using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite, which can be either a static image or an animated sequence of frames.
    /// </summary>
    public class Sprite
    {
        #region Properties

        public Texture2D Texture { get; }

        public Animation Animation { get; }

        /// <summary>
        /// Gets the origin offset for rendering this sprite.
        /// </summary>
        public Vector2 Origin { get; }

        public float Width => Animation.Frames[0].Width;

        public float Height => Animation.Frames[0].Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class using an array of texture regions as animation frames.
        /// </summary>
        /// <param name="frames">Ordered segment regions functioning sequentially as animation frames.</param>
        /// <param name="delay">Configured structural playback speed timing value applied across frames globally.</param>
        /// <param name="origin">The origin offset for rendering this sprite.</param>
        public Sprite(Texture2D texture, Rectangle[] frames, TimeSpan delay, Vector2 origin = default)
        {
            if (frames == null || frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));

            // Delay can be Zero for static sprites, but not negative
            if (delay < TimeSpan.Zero)
                throw new ArgumentException("Delay cannot be negative.", nameof(delay));

            Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
            Animation = new Animation(frames, delay);
            Origin = origin;
        }

        public Sprite(Texture2D texture, Rectangle[] frames, TimeSpan[] delays, Vector2 origin = default)
        {
            if (frames == null || frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));

            if (delays == null || delays.Length != frames.Length)
                throw new ArgumentException("Delays collection must match the number of frames.", nameof(delays));

            foreach (var delay in delays)
            {
                if (delay < TimeSpan.Zero)
                    throw new ArgumentException("Delays cannot contain negative values.", nameof(delays));
            }

            Texture = texture ?? throw new ArgumentNullException(nameof(texture), "Texture cannot be null.");
            Animation = new Animation(frames, delays);
            Origin = origin;
        }

        #endregion
    }
}
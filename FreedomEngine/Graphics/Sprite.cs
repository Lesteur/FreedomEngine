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
        /// Creates a new instance of the <see cref="Sprite"/> class using a single texture as a static image.
        /// </summary>
        /// <param name="texture">The base source texture applied initially to form this Sprite.</param>
        /// <param name="origin">The origin offset for rendering this sprite.</param>
        public Sprite(Texture2D texture, Vector2 origin) : this(
                [new TextureRegion(texture ?? throw new ArgumentNullException(nameof(texture)), 0, 0, (ushort)texture.Width, (ushort)texture.Height)],
                TimeSpan.Zero,
                origin)
        {
            // Logic is handled by the primary animation constructor via chaining
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class using an array of texture regions as animation frames.
        /// </summary>
        /// <param name="frames">Ordered segment regions functioning sequentially as animation frames.</param>
        /// <param name="delay">Configured structural playback speed timing value applied across frames globally.</param>
        /// <param name="origin">The origin offset for rendering this sprite.</param>
        public Sprite(TextureRegion[] frames, TimeSpan delay, Vector2 origin = default)
        {
            if (frames == null || frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));

            // Delay can be Zero for static sprites, but not negative
            if (delay < TimeSpan.Zero)
                throw new ArgumentException("Delay cannot be negative.", nameof(delay));

            Animation = new Animation(frames, delay);
            Origin = origin;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class by splitting a single texture into multiple frames based on the specified frame count and delay.
        /// </summary>
        /// <param name="texture2D">Source layout sheet holding structured frame cells across length linearly.</param>
        /// <param name="frameCount">Defines total valid contiguous split iterations mapping sequential valid region definitions initially across given texture axis horizontally.</param>
        /// <param name="delay">Length definition representing the internal frame offset rate timing constraints uniformly across animation scope lifetime iterations internally natively dynamically initially.</param>
        /// <param name="origin">The origin offset for rendering this sprite.</param>
        public Sprite(Texture2D texture2D, ushort frameCount, TimeSpan delay, Vector2 origin = default)
        {
            if (texture2D == null)
                throw new ArgumentNullException(nameof(texture2D), "Texture cannot be null.");

            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));

            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            ushort frameWidth = (ushort)(texture2D.Width / frameCount);
            ushort frameHeight = (ushort)texture2D.Height;

            TextureRegion[] frames = new TextureRegion[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                TextureRegion region = new(texture2D, i * frameWidth, 0, frameWidth, frameHeight);
                frames[i] = region;
            }

            Animation = new Animation(frames, delay);
            Origin = origin;
        }

        public Sprite(Texture2D texture2D, ushort frameCount, TimeSpan delay, Vector2 origin, ushort x, ushort y, ushort width, ushort height, ushort xMargin, ushort yMargin)
        {
            if (texture2D == null)
                throw new ArgumentNullException(nameof(texture2D), "Texture cannot be null.");

            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));

            if (delay < TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than or equal to zero.", nameof(delay));

            TextureRegion[] frames = new TextureRegion[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                TextureRegion region = new(texture2D, x + i * (width + xMargin), y + yMargin, width, height);
                frames[i] = region;
            }

            Animation = new Animation(frames, delay);
            Origin = origin;
        }

        #endregion
    }
}
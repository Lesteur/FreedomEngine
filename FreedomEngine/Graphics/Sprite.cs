using System;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite, which can be either a static image or an animated sequence of frames.
    /// </summary>
    public class Sprite
    {
        #region Properties

        /// <summary>
        /// Gets the collection of texture regions that make up the frames of this sprite.
        /// </summary>
        public TextureRegion[] Frames { get; }

        /// <summary>
        /// Gets the amount of time to delay before moving to the next frame.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the local X offset indicating drawing origin center.
        /// </summary>
        public ushort XOrigin { get; }

        /// <summary>
        /// Gets the local Y offset indicating drawing origin center.
        /// </summary>
        public ushort YOrigin { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class using a single texture as a static image.
        /// </summary>
        /// <param name="texture">The base source texture applied initially to form this Sprite.</param>
        /// <param name="xOrigin">X starting origin of rendering operations on this sprite bounds.</param>
        /// <param name="yOrigin">Y starting origin of rendering operations on this sprite bounds.</param>
        public Sprite(Texture2D texture, ushort xOrigin = 0, ushort yOrigin = 0) : this(
                [new TextureRegion(texture ?? throw new ArgumentNullException(nameof(texture)), 0, 0, (ushort)texture.Width, (ushort)texture.Height)],
                TimeSpan.Zero,
                xOrigin,
                yOrigin)
        {
            // Logic is handled by the primary animation constructor via chaining
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class using an array of texture regions as animation frames.
        /// </summary>
        /// <param name="frames">Ordered segment regions functioning sequentially as animation frames.</param>
        /// <param name="delay">Configured structural playback speed timing value applied across frames globally.</param>
        /// <param name="xOrigin">The offset applied during frame calculation along X plane relative natively to origin offset constraints.</param>
        /// <param name="yOrigin">The offset applied during frame calculation along Y plane relative natively to origin offset constraints.</param>
        public Sprite(TextureRegion[] frames, TimeSpan delay, ushort xOrigin = 0, ushort yOrigin = 0)
        {
            if (frames == null || frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));

            // Delay can be Zero for static sprites, but not negative
            if (delay < TimeSpan.Zero)
                throw new ArgumentException("Delay cannot be negative.", nameof(delay));

            Frames = frames;
            Delay = delay;
            XOrigin = xOrigin;
            YOrigin = yOrigin;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Sprite"/> class by splitting a single texture into multiple frames based on the specified frame count and delay.
        /// </summary>
        /// <param name="texture2D">Source layout sheet holding structured frame cells across length linearly.</param>
        /// <param name="frameCount">Defines total valid contiguous split iterations mapping sequential valid region definitions initially across given texture axis horizontally.</param>
        /// <param name="delay">Length definition representing the internal frame offset rate timing constraints uniformly across animation scope lifetime iterations internally natively dynamically initially.</param>
        /// <param name="xOrigin">Start offset local X-constraint axis offset mappings.</param>
        /// <param name="yOrigin">Start offset local Y-constraint axis offset mappings.</param>
        public Sprite(Texture2D texture2D, ushort frameCount, TimeSpan delay, ushort xOrigin = 0, ushort yOrigin = 0)
        {
            if (texture2D == null)
                throw new ArgumentNullException(nameof(texture2D), "Texture cannot be null.");

            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));

            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            ushort frameWidth = (ushort)(texture2D.Width / frameCount);
            ushort frameHeight = (ushort)texture2D.Height;

            Frames = new TextureRegion[frameCount];

            for (int i = 0; i < frameCount; i++)
            {
                TextureRegion region = new(texture2D, i * frameWidth, 0, frameWidth, frameHeight);
                Frames[i] = region;
            }

            Delay = delay;
            XOrigin = xOrigin;
            YOrigin = yOrigin;
        }

        #endregion
    }
}
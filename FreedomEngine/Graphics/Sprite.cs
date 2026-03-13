using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite, which can be either a static image or an animated sequence of frames.
    /// </summary>
    public class Sprite
    {
        /// <summary>
        /// Gets the collection of texture regions that make up the frames of this sprite.
        /// </summary>
        public List<TextureRegion> Frames { get; }

        /// <summary>
        /// Gets the amount of time to delay before moving to the next frame.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the local X offset indicating drawing origin center.
        /// </summary>
        public UInt16 XOrigin { get; }

        /// <summary>
        /// Gets the local Y offset indicating drawing origin center.
        /// </summary>
        public UInt16 YOrigin { get; }

        /// <summary>
        /// Creates a static single-frame Sprite constructed entirely around a source texture region.
        /// </summary>
        /// <param name="texture">The base source texture applied initially to form this Sprite.</param>
        /// <param name="xOrigin">X starting origin of rendering operations on this sprite bounds.</param>
        /// <param name="yOrigin">Y starting origin of rendering operations on this sprite bounds.</param>
        public Sprite(Texture2D texture, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            TextureRegion region = new(texture, 0, 0, (UInt16) texture.Width, (UInt16) texture.Height);

            Frames = [region];

            Delay = TimeSpan.Zero;
            XOrigin = xOrigin;
            YOrigin = yOrigin;
        }

        /// <summary>
        /// Creates a dynamically mapped animation comprising the listed texture regions.
        /// </summary>
        /// <param name="frames">Ordered segment regions functioning sequentially as animation frames.</param>
        /// <param name="delay">Configured structural playback speed timing value applied across frames globally.</param>
        /// <param name="xOrigin">The offset applied during frame calculation along X plane relative natively to origin offset constraints.</param>
        /// <param name="yOrigin">The offset applied during frame calculation along Y plane relative natively to origin offset constraints.</param>
        public Sprite(List<TextureRegion> frames, TimeSpan delay, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            if (frames == null || frames.Count == 0)
                throw new ArgumentException("Frames collection cannot be null or empty.", nameof(frames));
            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            Frames = frames;
            Delay = delay;

            XOrigin = xOrigin;
            YOrigin = yOrigin;
        }

        /// <summary>
        /// Splits a single source sheet into a linear array of uniformly sized regions mapped sequentially into a new animation instance.
        /// </summary>
        /// <param name="texture2D">Source layout sheet holding structured frame cells across length linearly.</param>
        /// <param name="frameCount">Defines total valid contiguous split iterations mapping sequential valid region definitions initially across given texture axis horizontally.</param>
        /// <param name="delay">Length definition representing the internal frame offset rate timing constraints uniformly across animation scope lifetime iterations internally natively dynamically initially.</param>
        /// <param name="xOrigin">Start offset local X-constraint axis offset mappings.</param>
        /// <param name="yOrigin">Start offset local Y-constraint axis offset mappings.</param>
        public Sprite(Texture2D texture2D, UInt16 frameCount, TimeSpan delay, UInt16 xOrigin = 0, UInt16 yOrigin = 0)
        {
            if (texture2D == null)
                throw new ArgumentNullException(nameof(texture2D), "Texture cannot be null.");
            if (frameCount <= 0)
                throw new ArgumentException("Frame count must be greater than zero.", nameof(frameCount));
            if (delay <= TimeSpan.Zero)
                throw new ArgumentException("Delay must be greater than zero.", nameof(delay));

            Frames = [];
            UInt16 frameWidth = (UInt16) (texture2D.Width / frameCount);
            UInt16 frameHeight = (UInt16) texture2D.Height;

            for (int i = 0; i < frameCount; i++)
            {
                TextureRegion region = new(texture2D, i * frameWidth, 0, frameWidth, frameHeight);
                Frames.Add(region);
            }

            Delay = delay;

            XOrigin = xOrigin;
            YOrigin = yOrigin;
        }
    }
}
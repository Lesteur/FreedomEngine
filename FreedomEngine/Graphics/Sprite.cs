using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a 2D sprite, which can be either a static image or an animated sequence of frames
    /// drawn from a shared texture.
    /// </summary>
    /// <remarks>
    /// A sprite with a single frame and a zero delay behaves as a static image. Frame timing is
    /// governed by the base <see cref="AnimatedResource"/> class, using either a single shared delay
    /// or individual per-frame delays.
    /// </remarks>
    public class Sprite : AnimatedResource
    {
        #region Properties

        /// <summary>
        /// Gets the source texture that the sprite's frames are drawn from.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the ordered texture regions, within <see cref="Texture"/>, that make up the animation frames.
        /// </summary>
        public Rectangle[] Frames { get; }

        /// <summary>
        /// Gets the origin offset, in pixels, used when rendering this sprite.
        /// </summary>
        public Vector2 Origin { get; }

        /// <summary>
        /// Gets the width, in pixels, of the sprite's first frame.
        /// </summary>
        public float Width => Frames[0].Width;

        /// <summary>
        /// Gets the height, in pixels, of the sprite's first frame.
        /// </summary>
        public float Height => Frames[0].Height;

        /// <summary>
        /// Gets the number of frames in the sprite's animation.
        /// </summary>
        public override int Length => Frames.Length;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class using a single delay shared
        /// by every frame.
        /// </summary>
        /// <param name="texture">The source texture the frames are drawn from.</param>
        /// <param name="frames">The ordered texture regions that make up the animation frames.</param>
        /// <param name="delay">
        /// The delay applied between every frame. Use <see cref="TimeSpan.Zero"/> for a static sprite.
        /// </param>
        /// <param name="origin">The origin offset, in pixels, used when rendering this sprite.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="texture"/> or <paramref name="frames"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="frames"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> is negative.</exception>
        public Sprite(Texture2D texture, Rectangle[] frames, TimeSpan delay, Vector2 origin = default) : base(delay)
        {
            ArgumentNullException.ThrowIfNull(texture);
            ArgumentNullException.ThrowIfNull(frames);

            if (frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be empty.", nameof(frames));

            Texture = texture;
            Frames = frames;
            Origin = origin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite"/> class using an individual delay for
        /// each frame.
        /// </summary>
        /// <param name="texture">The source texture the frames are drawn from.</param>
        /// <param name="frames">The ordered texture regions that make up the animation frames.</param>
        /// <param name="delays">
        /// The per-frame delays. Must contain the same number of elements as <paramref name="frames"/>.
        /// </param>
        /// <param name="origin">The origin offset, in pixels, used when rendering this sprite.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="texture"/>, <paramref name="frames"/>, or <paramref name="delays"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="frames"/> is empty, or <paramref name="delays"/> does not contain the same
        /// number of elements as <paramref name="frames"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delays"/> contains a negative value.
        /// </exception>
        public Sprite(Texture2D texture, Rectangle[] frames, TimeSpan[] delays, Vector2 origin = default) : base(delays)
        {
            ArgumentNullException.ThrowIfNull(texture);
            ArgumentNullException.ThrowIfNull(frames);

            if (frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be empty.", nameof(frames));

            if (frames.Length != delays.Length)
                throw new ArgumentException("Delays collection must match the number of frames.", nameof(delays));

            Texture = texture;
            Frames = frames;
            Origin = origin;
        }

        #endregion
    }
}
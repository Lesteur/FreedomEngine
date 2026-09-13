using System;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents an animation sequence for a specific tile, defined as an ordered list of tile IDs
    /// within a <see cref="Tileset"/>.
    /// </summary>
    public class TileAnimation : AnimatedResource
    {
        #region Properties

        /// <summary>
        /// Gets the number of frames in this tile animation.
        /// </summary>
        public override int Length => Frames.Length;

        /// <summary>
        /// Gets the ordered tile IDs, within the owning <see cref="Tileset"/>, that make up the frames
        /// of this animation.
        /// </summary>
        public ushort[] Frames { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TileAnimation"/> class using a single delay
        /// shared by every frame.
        /// </summary>
        /// <param name="frames">The ordered tile IDs that make up the frames of this animation.</param>
        /// <param name="delay">The amount of time to wait before advancing to the next frame.</param>
        /// <exception cref="ArgumentNullException"><paramref name="frames"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="frames"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> is negative.</exception>
        public TileAnimation(ushort[] frames, TimeSpan delay) : base(delay)
        {
            ArgumentNullException.ThrowIfNull(frames);

            if (frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be empty.", nameof(frames));

            Frames = frames;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileAnimation"/> class using an individual
        /// delay for each frame.
        /// </summary>
        /// <param name="frames">The ordered tile IDs that make up the frames of this animation.</param>
        /// <param name="delays">
        /// The per-frame delays. Must contain the same number of elements as <paramref name="frames"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="frames"/> or <paramref name="delays"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="frames"/> is empty, or <paramref name="delays"/> does not contain the same
        /// number of elements as <paramref name="frames"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delays"/> contains a negative value.
        /// </exception>
        public TileAnimation(ushort[] frames, TimeSpan[] delays) : base(delays)
        {
            ArgumentNullException.ThrowIfNull(frames);

            if (frames.Length == 0)
                throw new ArgumentException("Frames collection cannot be empty.", nameof(frames));

            if (frames.Length != delays.Length)
                throw new ArgumentException("The number of frames must match the number of delays.", nameof(delays));

            Frames = frames;
        }

        #endregion
    }
}
using System;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents an animation sequence for a specific tile.
    /// </summary>
    public class TileAnimation : AnimatedResource
    {
        #region Properties

        public override int Length => Frames.Length;

        /// <summary>
        /// Gets the collection of tile IDs that make up the frames of this animation.
        /// </summary>
        public ushort[] Frames { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="TileAnimation"/> class using a consistent delay across frames.
        /// </summary>
        /// <param name="frames">The collection of tile IDs that make up the frames of this animation.</param>
        /// <param name="delay">The amount of time to delay before moving to the next frame.</param>
        public TileAnimation(ushort[] frames, TimeSpan delay) : base(delay)
        {
            Frames = frames ?? throw new ArgumentNullException(nameof(frames));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TileAnimation"/> class using individual delays for each frame.
        /// </summary>
        /// <param name="frames">The collection of tile IDs that make up the frames of this animation.</param>
        /// <param name="delays">The list of delays for each frame in the animation, allowing for variable frame timing.</param>
        public TileAnimation(ushort[] frames, TimeSpan[] delays) : base(delays)
        {
            if (frames.Length != delays.Length)
                throw new ArgumentException("The number of frames must match the number of delays.", nameof(delays));

            Frames = frames ?? throw new ArgumentNullException(nameof(frames));
        }

        #endregion
    }
}
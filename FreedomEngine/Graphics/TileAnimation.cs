using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents an animation sequence for a specific tile.
    /// </summary>
    public class TileAnimation
    {
        #region Properties

        /// <summary>
        /// Gets the collection of tile IDs that make up the frames of this animation.
        /// </summary>
        public ushort[] Frames { get; }

        /// <summary>
        /// Gets the amount of time to delay before moving to the next frame.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the list of delays for each frame in the animation, allowing for variable frame timing.
        /// </summary>
        public TimeSpan[] Delays { get; }

        /// <summary>
        /// Gets a value indicating whether this tile uses a single frame delay for all frames (true) or individual delays per frame (false).
        /// </summary>
        public bool MonoFrameDelay { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="TileAnimation"/> class using a consistent delay across frames.
        /// </summary>
        /// <param name="frames">The collection of tile IDs that make up the frames of this animation.</param>
        /// <param name="delay">The amount of time to delay before moving to the next frame.</param>
        public TileAnimation(ushort[] frames, TimeSpan delay) : this(frames, delay, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TileAnimation"/> class using individual delays for each frame.
        /// </summary>
        /// <param name="frames">The collection of tile IDs that make up the frames of this animation.</param>
        /// <param name="delays">The list of delays for each frame in the animation, allowing for variable frame timing.</param>
        public TileAnimation(ushort[] frames, TimeSpan[] delays) : this(frames, TimeSpan.Zero, false)
        {
            if (frames.Length != delays.Length)
                throw new ArgumentException("The number of frames must match the number of delays.", nameof(delays));

            Delays = delays;
        }

        private TileAnimation(ushort[] frames, TimeSpan delay, bool monoFrameDelay)
        {
            Frames = frames ?? throw new ArgumentNullException(nameof(frames));
            Delay = delay;
            MonoFrameDelay = monoFrameDelay;
            Delays = monoFrameDelay ? null : new TimeSpan[frames.Length];
        }

        #endregion

        #region Public Methods

        public int GetNextFrame(int currentFrameIndex, TimeSpan elapsedTime, out TimeSpan newElapsedTime)
        {
            if (MonoFrameDelay)
            {
                if (elapsedTime >= Delay)
                {
                    newElapsedTime = elapsedTime - Delay;
                    return (currentFrameIndex + 1) % Frames.Length;
                }
            }
            else
            {
                if (elapsedTime >= Delays[currentFrameIndex])
                {
                    newElapsedTime = elapsedTime - Delays[currentFrameIndex];
                    return (currentFrameIndex + 1) % Frames.Length;
                }
            }

            newElapsedTime = elapsedTime;
            return currentFrameIndex;
        }

        #endregion
    }
}
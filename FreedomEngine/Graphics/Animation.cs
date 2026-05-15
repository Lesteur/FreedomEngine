using System;
using System.Collections.Generic;

namespace FreedomEngine.Graphics
{
    public class Animation
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
        /// Gets the list of delays for each frame in the animation, allowing for variable frame timing.
        /// </summary>
        public TimeSpan[] Delays { get; }

        /// <summary>
        /// Gets a value indicating whether this sprite uses a single frame delay for all frames (true) or individual delays per frame (false).
        /// </summary>
        public bool MonoFrameDelay { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Animation"/> class using a single texture as a static image.
        /// </summary>
        /// <param name="frames">The collection of texture regions that make up the frames of this animation.</param>
        /// <param name="delay">The amount of time to delay before moving to the next frame.</param>
        public Animation(TextureRegion[] frames, TimeSpan delay) : this(frames, delay, true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Animation"/> class using a single texture as a static image.
        /// </summary>
        /// <param name="frames">The collection of texture regions that make up the frames of this animation.</param>
        /// <param name="delays">The array of delays for each frame in the animation, allowing for variable frame timing.</param>
        public Animation(TextureRegion[] frames, TimeSpan[] delays) : this(frames, TimeSpan.Zero, false)
        {
            if (frames.Length != delays.Length)
                throw new ArgumentException("The number of frames must match the number of delays.", nameof(delays));

            Delays = delays;
        }

        private Animation(TextureRegion[] frames, TimeSpan delay, bool monoFrameDelay)
        {
            Frames = frames ?? throw new ArgumentNullException(nameof(frames));
            Delay = delay;
            MonoFrameDelay = monoFrameDelay;
            Delays = monoFrameDelay ? null : [.. new TimeSpan[frames.Length]];
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

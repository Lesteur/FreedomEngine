using System;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Provides the base functionality for a resource that plays back a sequence of frames over time,
    /// such as a sprite animation or an animated tile.
    /// </summary>
    /// <remarks>
    /// A derived resource can use either a single, uniform delay applied to every frame
    /// (<see cref="Delay"/>) or an individual delay per frame (<see cref="Delays"/>). Which mode is
    /// active is reported by <see cref="MonoFrameDelay"/> and is fixed for the lifetime of the
    /// instance, based on which constructor was used to create it.
    /// </remarks>
    public abstract class AnimatedResource
    {
        #region Properties

        /// <summary>
        /// Gets the total number of frames in the animation.
        /// </summary>
        public abstract int Length { get; }

        /// <summary>
        /// Gets the amount of time to wait before advancing to the next frame, when a single delay
        /// is shared by every frame.
        /// </summary>
        /// <remarks>
        /// This value is only meaningful when <see cref="MonoFrameDelay"/> is <see langword="true"/>.
        /// </remarks>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Gets the per-frame delays for the animation, allowing each frame to have its own duration.
        /// </summary>
        /// <remarks>
        /// This value is only meaningful when <see cref="MonoFrameDelay"/> is <see langword="false"/>;
        /// otherwise it is <see langword="null"/>. When set, its length matches <see cref="Length"/>.
        /// </remarks>
        public TimeSpan[] Delays { get; }

        /// <summary>
        /// Gets a value indicating whether this resource uses a single, shared delay for every frame
        /// (<see langword="true"/>), or an individual delay per frame (<see langword="false"/>).
        /// </summary>
        public bool MonoFrameDelay { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedResource"/> class using a single delay
        /// shared by every frame.
        /// </summary>
        /// <param name="delay">The amount of time to wait before advancing to the next frame.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> is negative.</exception>
        protected AnimatedResource(TimeSpan delay)
        {
            if (delay < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(delay), delay, "Delay cannot be negative.");

            Delay = delay;
            MonoFrameDelay = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimatedResource"/> class using an individual
        /// delay for each frame.
        /// </summary>
        /// <param name="delays">
        /// The per-frame delays. Its length determines <see cref="Length"/> in derived types.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="delays"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="delays"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delays"/> contains a negative value.
        /// </exception>
        protected AnimatedResource(TimeSpan[] delays)
        {
            ArgumentNullException.ThrowIfNull(delays);

            if (delays.Length == 0)
                throw new ArgumentException("Delays collection cannot be empty.", nameof(delays));

            for (int i = 0; i < delays.Length; i++)
            {
                if (delays[i] < TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(delays), delays[i], "Delays cannot contain negative values.");
            }

            Delays = delays;
            MonoFrameDelay = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Advances the animation by the given elapsed time and determines the resulting frame index.
        /// </summary>
        /// <param name="currentFrameIndex">The index of the frame the animation is currently on.</param>
        /// <param name="elapsedTime">
        /// The amount of time that has elapsed since <paramref name="currentFrameIndex"/> was reached.
        /// </param>
        /// <param name="newElapsedTime">
        /// When this method returns, contains the remaining elapsed time after accounting for any
        /// frame advance.
        /// </param>
        /// <returns>
        /// The frame index the animation should be on. This is either <paramref name="currentFrameIndex"/>,
        /// unchanged, or the next frame (wrapping back to <c>0</c> after the last frame) if enough time
        /// has elapsed.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="currentFrameIndex"/> is negative or is not less than <see cref="Length"/>, or
        /// <paramref name="elapsedTime"/> is negative.
        /// </exception>
        public int GetNextFrame(int currentFrameIndex, TimeSpan elapsedTime, out TimeSpan newElapsedTime)
        {
            if (currentFrameIndex < 0 || currentFrameIndex >= Length)
                throw new ArgumentOutOfRangeException(nameof(currentFrameIndex), currentFrameIndex, $"Value must be between 0 and {Length - 1}.");

            if (elapsedTime < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(elapsedTime), elapsedTime, "Elapsed time cannot be negative.");

            TimeSpan frameDelay = MonoFrameDelay ? Delay : Delays[currentFrameIndex];

            if (elapsedTime >= frameDelay)
            {
                newElapsedTime = elapsedTime - frameDelay;
                return (currentFrameIndex + 1) % Length;
            }

            newElapsedTime = elapsedTime;
            return currentFrameIndex;
        }

        #endregion
    }
}
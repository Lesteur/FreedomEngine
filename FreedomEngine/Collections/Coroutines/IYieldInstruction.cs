using System;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Represents a yield instruction that suspends a coroutine until some condition is met.
    /// </summary>
    /// <remarks>
    /// A coroutine yields an instance of this interface to pause its own execution. The owning
    /// <see cref="Coroutine"/> calls <see cref="IsDone"/> once per frame while the instruction is the
    /// current yielded value, and only advances the coroutine once it returns <see langword="true"/>.
    /// </remarks>
    public interface IYieldInstruction
    {
        /// <summary>
        /// Determines whether the coroutine may resume.
        /// </summary>
        /// <param name="elapsedTime">The amount of time that has elapsed since the previous frame.</param>
        /// <returns>
        /// <see langword="true"/> if the coroutine should advance to its next step; otherwise,
        /// <see langword="false"/> to keep waiting.
        /// </returns>
        /// <remarks>
        /// An implementation may be stateful and is called once per frame, so it should expect to be
        /// polled repeatedly until it reports completion.
        /// </remarks>
        bool IsDone(TimeSpan elapsedTime);
    }

    /// <summary>
    /// A yield instruction that waits for a fixed amount of time.
    /// </summary>
    /// <remarks>
    /// This instruction is stateful and single-use: it counts down as it is polled, so an instance
    /// that has already completed reports itself done immediately if yielded again. Create a new
    /// instance for each wait.
    /// </remarks>
    public sealed class WaitForSeconds : IYieldInstruction
    {
        /// <summary>
        /// The remaining amount of time to wait.
        /// </summary>
        private TimeSpan _remainingTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForSeconds"/> class.
        /// </summary>
        /// <param name="duration">The amount of time to wait.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative.</exception>
        public WaitForSeconds(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");

            _remainingTime = duration;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForSeconds"/> class.
        /// </summary>
        /// <param name="seconds">The number of seconds to wait.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="seconds"/> is negative, or is <see cref="double.NaN"/>.
        /// </exception>
        public WaitForSeconds(double seconds)
        {
            if (double.IsNaN(seconds) || seconds < 0.0)
                throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Seconds cannot be negative or NaN.");

            _remainingTime = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Subtracts the elapsed time from the remaining wait and reports whether the wait is over.
        /// </summary>
        /// <param name="elapsedTime">The amount of time that has elapsed since the previous frame.</param>
        /// <returns><see langword="true"/> once the requested duration has elapsed; otherwise, <see langword="false"/>.</returns>
        public bool IsDone(TimeSpan elapsedTime)
        {
            _remainingTime -= elapsedTime;
            return _remainingTime <= TimeSpan.Zero;
        }
    }

    /// <summary>
    /// A yield instruction that waits for a single frame.
    /// </summary>
    /// <remarks>
    /// This instruction is stateful and single-use: once it has let a frame pass, it reports itself
    /// done for every subsequent poll. Create a new instance for each wait.
    /// </remarks>
    public sealed class WaitForNextFrame : IYieldInstruction
    {
        /// <summary>
        /// Indicates whether one frame has passed.
        /// </summary>
        private bool _hasWaited;

        /// <summary>
        /// Reports whether a frame has passed since this instruction was first polled.
        /// </summary>
        /// <param name="elapsedTime">The amount of time that has elapsed since the previous frame. Unused.</param>
        /// <returns><see langword="false"/> on the first call; otherwise, <see langword="true"/>.</returns>
        public bool IsDone(TimeSpan elapsedTime)
        {
            if (_hasWaited)
                return true;

            _hasWaited = true;
            return false;
        }
    }

    /// <summary>
    /// A yield instruction that waits until a condition becomes <see langword="true"/>.
    /// </summary>
    public sealed class WaitUntil : IYieldInstruction
    {
        /// <summary>
        /// The condition to wait for.
        /// </summary>
        private readonly Func<bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil"/> class.
        /// </summary>
        /// <param name="predicate">The condition to wait for. Evaluated once per frame.</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Evaluates the condition and reports whether the wait is over.
        /// </summary>
        /// <param name="elapsedTime">The amount of time that has elapsed since the previous frame. Unused.</param>
        /// <returns>The current value of the condition.</returns>
        public bool IsDone(TimeSpan elapsedTime)
        {
            return _predicate();
        }
    }

    /// <summary>
    /// A yield instruction that waits while a condition remains <see langword="true"/>.
    /// </summary>
    public sealed class WaitWhile : IYieldInstruction
    {
        /// <summary>
        /// The condition to wait while true.
        /// </summary>
        private readonly Func<bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitWhile"/> class.
        /// </summary>
        /// <param name="predicate">The condition to wait while true. Evaluated once per frame.</param>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is <see langword="null"/>.</exception>
        public WaitWhile(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Evaluates the condition and reports whether the wait is over.
        /// </summary>
        /// <param name="elapsedTime">The amount of time that has elapsed since the previous frame. Unused.</param>
        /// <returns>The negated value of the condition.</returns>
        public bool IsDone(TimeSpan elapsedTime)
        {
            return !_predicate();
        }
    }
}
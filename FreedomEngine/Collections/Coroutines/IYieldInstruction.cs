using System;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Interface for custom yield instructions that can control coroutine timing.
    /// </summary>
    public interface IYieldInstruction
    {
        bool IsDone(TimeSpan timeSpan);
    }

    /// <summary>
    /// Yield instruction that waits for a specified number of seconds.
    /// </summary>
    public sealed class WaitForSeconds : IYieldInstruction
    {
        /// <summary>
        /// The remaining time to wait in seconds.
        /// </summary>
        private TimeSpan _remainingTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitForSeconds"/> class.
        /// </summary>
        /// <param name="timeSpan">The amount of time to wait.</param>
        public WaitForSeconds(TimeSpan timeSpan)
        {
            _remainingTime = timeSpan;
        }

        public WaitForSeconds(double seconds)
        {
            _remainingTime = TimeSpan.FromSeconds(seconds);
        }

        public bool IsDone(TimeSpan timeSpan)
        {
            _remainingTime -= timeSpan;
            return _remainingTime <= TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Yield instruction that waits for the next frame.
    /// </summary>
    public sealed class WaitForNextFrame : IYieldInstruction
    {
        /// <summary>
        /// Indicates whether one frame has passed.
        /// </summary>
        private bool _hasWaited;

        public bool IsDone(TimeSpan timeSpan)
        {
            if (_hasWaited)
                return true;

            _hasWaited = true;
            return false;
        }
    }

    /// <summary>
    /// Yield instruction that waits until a condition becomes true.
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
        /// <param name="predicate">The condition to wait for.</param>
        public WaitUntil(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public bool IsDone(TimeSpan timeSpan)
        {
            return _predicate();
        }
    }

    /// <summary>
    /// Yield instruction that waits while a condition is true.
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
        /// <param name="predicate">The condition to wait while true.</param>
        public WaitWhile(Func<bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public bool IsDone(TimeSpan timeSpan)
        {
            return !_predicate();
        }
    }
}

using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Tweens
{
    /// <summary>
    /// Interface allowing the TweenManager to update tweens of any generic type.
    /// </summary>
    public interface ITween : IControllableProcess
    {
    }

    public class Tween<T> : ITween
    {
        #region Fields

        private readonly Action<T> _setter;

        private readonly Func<T, T, float, T> _lerpFunc;

        /// <summary>
        /// Indicates whether this coroutine is currently paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Indicates whether this coroutine has completed execution.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region Properties

        public T From { get; private set; }

        public T To { get; private set; }

        public TimeSpan Duration { get; private set; }

        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// Gets whether this coroutine is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets whether this coroutine has finished executing.
        /// </summary>
        public bool IsFinished => _isFinished || Elapsed >= Duration;

        /// <summary>
        /// Gets whether this coroutine is currently running (not paused and not finished).
        /// </summary>
        public bool IsRunning => !_isFinished && !_isPaused;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new tween for a specific value type.
        /// </summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration of the tween.</param>
        /// <param name="setter">Action to apply the interpolated value to the target property.</param>
        /// <param name="lerpFunc">Function used to interpolate between the from and to values.</param>
        public Tween(T from, T to, TimeSpan duration, Action<T> setter, Func<T, T, float, T> lerpFunc)
        {
            _setter = setter;
            _lerpFunc = lerpFunc;

            From = from;
            To = to;
            Duration = duration;
            Elapsed = TimeSpan.Zero;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the tween's state and applies the animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public void Update(GameTime gameTime)
        {
            if (_isFinished || _isPaused)
                return;

            Elapsed += gameTime.ElapsedGameTime;

            // Calculate progress between 0 and 1
            float t = MathHelper.Clamp((float)(Elapsed.TotalSeconds / Duration.TotalSeconds), 0f, 1f);

            // Interpolate and apply the value
            T currentValue = _lerpFunc(From, To, t);
            _setter(currentValue);
        }

        #endregion

        #region Public Methods

        public void Pause()
        {
            _isPaused = true;
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void Stop()
        {
            _isFinished = true;
        }

        #endregion
    }
}

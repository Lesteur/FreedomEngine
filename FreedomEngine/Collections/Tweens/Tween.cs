using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Core;
using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Tweens
{
    public class Tween : IControllableProcess
    {
        #region Fields

        private static TweenManager Controller => Application.Tweens;

        /// <summary>
        /// Indicates whether this coroutine is currently paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Indicates whether this coroutine has completed execution.
        /// </summary>
        private bool _isFinished;

        /// <summary>
        /// The normalized progress of the tween, ranging from 0 to 1.
        /// </summary>
        protected float _progress;

        #endregion

        #region Properties

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
        /// <param name="duration">Duration of the tween.</param>
        public Tween(TimeSpan duration)
        {
            Duration = duration;
            Elapsed = TimeSpan.Zero;

            Controller.Add(this);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the tween's state and applies the animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (Elapsed >= Duration || _isFinished || _isPaused)
                return;

            Elapsed += gameTime.ElapsedGameTime;

            // Calculate progress between 0 and 1
            _progress = MathHelper.Clamp((float)(Elapsed.TotalSeconds / Duration.TotalSeconds), 0f, 1f);
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

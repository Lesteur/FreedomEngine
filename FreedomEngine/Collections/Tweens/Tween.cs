using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Tweens
{
    /// <summary>
    /// Represents a timed interpolation that progresses from 0 to 1 over a fixed duration.
    /// </summary>
    /// <remarks>
    /// <see cref="Tween"/> only tracks timing and normalized <see cref="_progress"/>; it does not
    /// interpolate or apply any value on its own. A derived class is expected to override
    /// <see cref="Update"/>, call the base implementation to advance <see cref="_progress"/>, and then
    /// apply its own interpolated value.
    /// </remarks>
    public abstract class Tween : IControllableProcess
    {
        #region Fields

        /// <summary>
        /// Indicates whether this tween is currently paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Indicates whether this tween has been explicitly stopped.
        /// </summary>
        private bool _isFinished;

        /// <summary>
        /// The normalized progress of the tween, ranging from 0 to 1.
        /// </summary>
        protected float _progress;

        /// <summary>
        /// The easing function used to shape the normalized progress before interpolation.
        /// </summary>
        protected Func<float, float> _func;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the manager that newly created tweens are registered with.
        /// </summary>
        /// <remarks>
        /// Must be assigned before constructing a <see cref="Tween"/>; the engine assigns this as
        /// part of a scene transition. See <see cref="Core.Application.ChangeScene"/>.
        /// </remarks>
        public static ProcessManager<Tween> Controller { get; set; }

        /// <summary>
        /// Gets the total duration of the tween.
        /// </summary>
        public TimeSpan Duration { get; protected set; }

        /// <summary>
        /// Gets the amount of time that has elapsed since the tween started.
        /// </summary>
        public TimeSpan Elapsed { get; protected set; }

        /// <summary>
        /// Gets whether this tween is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets whether this tween has finished, either because it was explicitly stopped or because
        /// <see cref="Elapsed"/> has reached <see cref="Duration"/>.
        /// </summary>
        public bool IsFinished => _isFinished || Elapsed >= Duration;

        /// <summary>
        /// Gets whether this tween is currently running (not paused and not finished).
        /// </summary>
        public bool IsRunning => !_isFinished && !_isPaused;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the tween's state, advancing <see cref="Elapsed"/> and recalculating <see cref="_progress"/>.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// A class deriving from <see cref="Tween"/> should call this base implementation first, then
        /// use the updated <see cref="_progress"/> to apply its own interpolated value.
        /// </remarks>
        public virtual void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            if (IsFinished || _isPaused)
                return;

            Elapsed += gameTime.ElapsedGameTime;

            // Calculate progress between 0 and 1
            _progress = MathHelper.Clamp((float)(Elapsed.TotalSeconds / Duration.TotalSeconds), 0f, 1f);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses the execution of this tween.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resumes the execution of this tween.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Stops the execution of this tween immediately.
        /// </summary>
        public void Stop()
        {
            _isFinished = true;
        }

        #endregion
    }
}

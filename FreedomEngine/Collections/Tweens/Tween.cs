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

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the manager that newly created tweens are registered with.
        /// </summary>
        /// <remarks>
        /// Must be assigned before constructing a <see cref="Tween"/>; the engine assigns this as
        /// part of a scene transition. See <see cref="FreedomEngine.Core.Application.ChangeScene"/>.
        /// </remarks>
        public static TweenManager Controller { get; set; }

        /// <summary>
        /// Gets the total duration of the tween.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets the amount of time that has elapsed since the tween started.
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

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

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tween"/> class and registers it with
        /// <see cref="Controller"/>.
        /// </summary>
        /// <param name="duration">
        /// The total duration of the tween. A duration of <see cref="TimeSpan.Zero"/> produces a tween
        /// that is already complete, so a derived class applies its target value on its first update.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Controller"/> has not been assigned.</exception>
        protected Tween(TimeSpan duration)
        {
            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");

            Duration = duration;
            Elapsed = TimeSpan.Zero;

            // A zero-length tween is complete on creation; jump straight to the end of the curve so a
            // derived class applies its target value rather than its starting value.
            _progress = duration > TimeSpan.Zero ? 0f : 1f;

            if (Controller == null)
                throw new InvalidOperationException($"{nameof(Tween)}.{nameof(Controller)} must be assigned a {nameof(TweenManager)} before creating a tween.");

            Controller.Add(this);
        }

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
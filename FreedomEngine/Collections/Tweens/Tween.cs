using FreedomEngine.Collections.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.Metrics;

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
    public abstract class Tween<T> : ITween where T : struct
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
        /// The action used to apply each interpolated value to the target property.
        /// </summary>
        protected readonly Action<T> _setter;

        /// <summary>
        /// The easing function used to shape the normalized progress before interpolation.
        /// </summary>
        protected readonly Func<float, float> _func;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the manager that newly created tweens are registered with.
        /// </summary>
        /// <remarks>
        /// Must be assigned before constructing a <see cref="ITween"/>; the engine assigns this as
        /// part of a scene transition. See <see cref="Core.Application.ChangeScene"/>.
        /// </remarks>
        public static ProcessManager<ITween> Controller { get; set; }

        /// <summary>
        /// Gets the value the tween interpolates from.
        /// </summary>
        public T From { get; }

        /// <summary>
        /// Gets the value the tween interpolates to.
        /// </summary>
        public T To { get; }

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
        /// Initializes a new instance of the <see cref="T"/> class and registers it with
        /// <see cref="Tween.Controller"/>.
        /// </summary>
        /// <param name="from">The starting value.</param>
        /// <param name="to">The target value.</param>
        /// <param name="duration">The total duration of the tween.</param>
        /// <param name="setter">The action used to apply the interpolated value to the target property.</param>
        /// <param name="func">
        /// The easing function applied to the normalized progress before interpolation. It receives a
        /// value in the range [0, 1] and is expected to return an eased value, typically also in [0, 1].
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="setter"/> or <paramref name="func"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="duration"/> is negative.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Tween.Controller"/> has not been assigned.</exception>
        protected Tween(T from, T to, TimeSpan duration, Action<T> setter, Func<float, float> func)
        {
            ArgumentNullException.ThrowIfNull(setter);
            ArgumentNullException.ThrowIfNull(func);

            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");

            if (Controller == null)
                throw new InvalidOperationException($"{nameof(Tween<T>)}.{nameof(Controller)} must be assigned a {nameof(ProcessManager<ITween>)} before creating a tween.");

            _progress = duration > TimeSpan.Zero ? 0f : 1f;

            Duration = duration;
            Elapsed = TimeSpan.Zero;

            From = from;
            To = to;

            _setter = setter;
            _func = func;

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
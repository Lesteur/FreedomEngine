using System;

namespace FreedomEngine.Collections.Tweens
{
    /// <summary>
    /// Represents a tween that interpolates a value of type <typeparamref name="T"/>
    /// between a start and an end value over a fixed duration.
    /// </summary>
    /// <typeparam name="T">The type of the value being interpolated.</typeparam>
    public abstract class TweenGeneric<T> : Tween where T : struct
    {
        #region Fields

        /// <summary>
        /// The action used to apply each interpolated value to the target property.
        /// </summary>
        protected readonly Action<T> _setter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value the tween interpolates from.
        /// </summary>
        public T From { get; }

        /// <summary>
        /// Gets the value the tween interpolates to.
        /// </summary>
        public T To { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TweenGeneric{T}"/> class and registers it with
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
        protected TweenGeneric(T from, T to, TimeSpan duration, Action<T> setter, Func<float, float> func)
        {
            ArgumentNullException.ThrowIfNull(setter);
            ArgumentNullException.ThrowIfNull(func);

            if (duration < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "Duration cannot be negative.");

            if (Controller == null)
                throw new InvalidOperationException($"{nameof(TweenGeneric<T>)}.{nameof(Controller)} must be assigned a {nameof(ProcessManager<Tween>)} before creating a tween.");

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
    }
}
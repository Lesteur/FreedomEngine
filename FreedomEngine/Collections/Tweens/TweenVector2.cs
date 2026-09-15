using System;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Tweens
{
    /// <summary>
    /// Interpolates a <see cref="Vector2"/> value between a start and a target value over a fixed
    /// duration, applying each interpolated value through a caller-supplied setter.
    /// </summary>
    public class TweenVector2 : Tween
    {
        #region Fields

        /// <summary>
        /// The action used to apply each interpolated value to the target property.
        /// </summary>
        private readonly Action<Vector2> _setter;

        /// <summary>
        /// The easing function used to shape the normalized progress before interpolation.
        /// </summary>
        private readonly Func<float, float> _func;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value the tween interpolates from.
        /// </summary>
        public Vector2 From { get; }

        /// <summary>
        /// Gets the value the tween interpolates to.
        /// </summary>
        public Vector2 To { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TweenVector2"/> class and registers it with
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
        public TweenVector2(Vector2 from, Vector2 to, TimeSpan duration, Action<Vector2> setter, Func<float, float> func) : base(duration)
        {
            ArgumentNullException.ThrowIfNull(setter);
            ArgumentNullException.ThrowIfNull(func);

            From = from;
            To = to;

            _setter = setter;
            _func = func;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Advances the tween and applies the resulting interpolated value through the setter.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        /// <remarks>While the tween is paused, no value is applied.</remarks>
        public override void Update(GameTime gameTime)
        {
            if (IsPaused)
                return;

            base.Update(gameTime);

            // Calculate the eased progress using the specified easing function
            float easedProgress = _func(_progress);

            // Interpolate between the From and To values based on the eased progress
            Vector2 currentValue = From + (To - From) * easedProgress;

            // Apply the interpolated value using the setter
            _setter(currentValue);
        }

        #endregion
    }
}
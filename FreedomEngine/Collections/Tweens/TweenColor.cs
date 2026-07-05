using System;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Tweens
{
    public class TweenColor : Tween
    {
        #region Fields

        private readonly Action<Color> _setter;

        private readonly Func<float, float> _func;

        #endregion

        #region Properties

        public Color From { get; private set; }

        public Color To { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new tween for a specific value type.
        /// </summary>
        /// <param name="from">Starting value.</param>
        /// <param name="to">Target value.</param>
        /// <param name="duration">Duration of the tween.</param>
        /// <param name="setter">Action to apply the interpolated value to the target property.</param>
        /// <param name="func">Function used to interpolate between the from and to values.</param>
        public TweenColor(Color from, Color to, TimeSpan duration, Action<Color> setter, Func<float, float> func) : base(duration)
        {
            From = from;
            To = to;

            _setter = setter;
            _func = func;
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculate the eased progress using the specified easing function
            float easedProgress = _func(_progress);

            // Interpolate between the From and To values based on the eased progress
            Color currentValue = new(
                (byte)(From.R + (To.R - From.R) * easedProgress),
                (byte)(From.G + (To.G - From.G) * easedProgress),
                (byte)(From.B + (To.B - From.B) * easedProgress),
                (byte)(From.A + (To.A - From.A) * easedProgress)
            );

            // Apply the interpolated value using the setter
            _setter(currentValue);
        }

        #endregion
    }
}

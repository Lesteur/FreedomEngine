using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.UI
{
    /// <summary>
    /// Interface allowing the TweenManager to update tweens of any generic type.
    /// </summary>
    public interface ITween : IUpdate
    {
        bool IsComplete { get; }

        bool IsKilled { get; }
        
        /// <summary>
        /// Instantly stops the tween and marks it for removal from the TweenManager.
        /// </summary>
        void Kill();
    }

    public class Tween<T> : ITween
    {
        #region Fields

        private readonly Action<T> _setter;

        private readonly Func<T, T, float, T> _lerpFunc;

        #endregion

        #region Properties

        public T From { get; private set; }

        public T To { get; private set; }

        public TimeSpan Duration { get; private set; }

        public TimeSpan Elapsed { get; private set; }
        
        public bool IsKilled { get; private set; }
        
        public bool IsComplete => Elapsed >= Duration || IsKilled;

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
            IsKilled = false;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the tween's state and applies the animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public void Update(GameTime gameTime)
        {
            if (IsComplete)
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

        public void Kill()
        {
            IsKilled = true;
        }

        #endregion
    }
}

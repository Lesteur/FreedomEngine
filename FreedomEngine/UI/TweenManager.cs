using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Components;

namespace FreedomEngine.UI
{
    public class TweenManager : IUpdate
    {
        #region Fields

        private readonly List<ITween> _tweens;

        private readonly List<ITween> _pendingTweens; 

        #endregion

        #region Constructors

        public TweenManager()
        {
            _tweens = [];
            _pendingTweens = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state of all active tweens, progressing their animations based on the elapsed time.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public void Update(GameTime gameTime)
        {
            // Add any newly created tweens since the last frame
            if (_pendingTweens.Count > 0)
            {
                _tweens.AddRange(_pendingTweens);
                _pendingTweens.Clear();
            }

            // Iterate backwards to safely remove elements while looping
            for (int i = _tweens.Count - 1; i >= 0; i--)
            {
                var tween = _tweens[i];
                tween.Update(gameTime);

                // This will now also remove tweens that have been killed using .Kill()
                if (tween.IsComplete)
                {
                    _tweens.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Animates the position of an Entity or UIElement.
        /// </summary>
        public ITween TweenPosition(Entity entity, Vector2 from, Vector2 to, TimeSpan duration)
        {
            var tween = new Tween<Vector2>(from, to, duration, val => entity.Position = val, Vector2.Lerp);
            _pendingTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Animates the scale of an Entity or UIElement.
        /// </summary>
        public ITween TweenScale(Entity entity, Vector2 from, Vector2 to, TimeSpan duration)
        {
            var tween = new Tween<Vector2>(from, to, duration, val => entity.Scale = val, Vector2.Lerp);
            _pendingTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Animates the color of an Entity or UIElement.
        /// </summary>
        public ITween TweenColor(Entity entity, Color from, Color to, TimeSpan duration)
        {
            var tween = new Tween<Color>(from, to, duration, val => entity.Color = val, Color.Lerp);
            _pendingTweens.Add(tween);
            return tween;
        }

        /// <summary>
        /// Animates the rotation of an Entity or UIElement.
        /// </summary>
        public ITween TweenRotation(Entity entity, float fromRadians, float toRadians, TimeSpan duration)
        {
            var tween = new Tween<float>(fromRadians, toRadians, duration, val => entity.Rotation = (int)MathHelper.ToDegrees(val), MathHelper.Lerp);
            _pendingTweens.Add(tween);
            return tween;
        }

        #endregion
    }
}
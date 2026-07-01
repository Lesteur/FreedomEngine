using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace FreedomEngine.Collections.Tweens
{
    /// <summary>
    /// Manages the state and execution of all active tweens.
    /// </summary>
    public class TweenManager : IProcessManager
    {
        #region Fields

        /// <summary>
        /// The list of all currently active tweens.
        /// </summary>
        private readonly List<ITween> _tweens;

        /// <summary>
        /// Pending tweens to add, processed safely.
        /// </summary>
        private readonly List<ITween> _pendingTweens; 

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of currently active tweens.
        /// </summary>
        public int ActiveCount => _tweens.Count;

        /// <summary>
        /// Gets whether there are any active tweens.
        /// </summary>
        public bool HasActiveProcesses => _tweens.Count > 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TweenManager"/> class.
        /// </summary>
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

                if (tween.IsFinished)
                {
                    _tweens.RemoveAt(i);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses all active tweens.
        /// </summary>
        public void PauseAll()
        {
            foreach (var tween in _tweens)
            {
                tween.Pause();
            }
        }

        /// <summary>
        /// Resumes all active tweens.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var tween in _tweens)
            {
                tween.Resume();
            }
        }

        /// <summary>
        /// Stops all active tweens immediately.
        /// </summary>
        public void StopAll()
        {
            foreach (var tween in _tweens)
            {
                tween.Stop();
            }

            foreach (var tween in _pendingTweens)
            {
                tween.Stop();
            }

            _tweens.Clear();
            _pendingTweens.Clear();
        }

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
            var tween = new Tween<float>(fromRadians, toRadians, duration, val => entity.Rotation = val, MathHelper.Lerp);
            _pendingTweens.Add(tween);
            return tween;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this tween manager and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this tween manager and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            StopAll();
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Components;
using FreedomEngine.Collections.Interfaces;

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
        private readonly List<Tween> _tweens;

        /// <summary>
        /// Pending tweens to add, processed safely.
        /// </summary>
        private readonly List<Tween> _pendingTweens; 

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
            if (_pendingTweens.Count > 0)
            {
                _tweens.AddRange(_pendingTweens);
                _pendingTweens.Clear();
            }

            int aliveCount = 0;
            for (int i = 0; i < _tweens.Count; i++)
            {
                var tween = _tweens[i];
                tween.Update(gameTime);

                if (!tween.IsFinished)
                {
                    _tweens[aliveCount++] = tween;
                }
            }

            if (aliveCount < _tweens.Count)
            {
                _tweens.RemoveRange(aliveCount, _tweens.Count - aliveCount);
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

        #endregion

        #region Internal Methods

        internal void Add(Tween tween)
        {
            _pendingTweens.Add(tween);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this tween manager and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            StopAll();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
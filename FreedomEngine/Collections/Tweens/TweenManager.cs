using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

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
        /// Pending tweens to add, processed at the end of the frame to avoid collection modification
        /// during iteration.
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

        /// <summary>
        /// Gets a value indicating whether this manager has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

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
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

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
        /// Pauses all running and pending tweens.
        /// </summary>
        public void PauseAll()
        {
            foreach (var tween in _tweens)
            {
                tween.Pause();
            }

            foreach (var tween in _pendingTweens)
            {
                tween.Pause();
            }
        }

        /// <summary>
        /// Resumes all running and pending tweens.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var tween in _tweens)
            {
                tween.Resume();
            }

            foreach (var tween in _pendingTweens)
            {
                tween.Resume();
            }
        }

        /// <summary>
        /// Stops all running and pending tweens immediately, and clears them from this manager.
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

        /// <inheritdoc/>
        public void Clear()
        {
            StopAll();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Queues a tween to be added to this manager on the next <see cref="Update"/> call.
        /// </summary>
        /// <param name="tween">The tween to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tween"/> is <see langword="null"/>.</exception>
        internal void Add(Tween tween)
        {
            ArgumentNullException.ThrowIfNull(tween);

            _pendingTweens.Add(tween);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this tween manager, stopping all tweens and cleaning up resources.
        /// </summary>
        /// <remarks>Calling this method more than once has no additional effect.</remarks>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            StopAll();

            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
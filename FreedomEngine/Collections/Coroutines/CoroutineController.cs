using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Collections.Tweens;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Manages the state and execution of all active coroutines.
    /// </summary>
    public class CoroutineController : IProcessManager
    {
        #region Fields

        /// <summary>
        /// The list of all currently active coroutines.
        /// </summary>
        private readonly List<Coroutine> _coroutines;

        /// <summary>
        /// Pending coroutines to add, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly List<Coroutine> _pendingCoroutines;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of currently active coroutines.
        /// </summary>
        public int ActiveCount => _coroutines.Count;

        /// <summary>
        /// Gets whether there are any active coroutines.
        /// </summary>
        public bool HasActiveProcesses => _coroutines.Count > 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineController"/> class.
        /// </summary>
        public CoroutineController()
        {
            _coroutines = [];
            _pendingCoroutines = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates all active coroutines.
        /// Called once per frame by the game loop.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (_pendingCoroutines.Count > 0)
            {
                _coroutines.AddRange(_pendingCoroutines);
                _pendingCoroutines.Clear();
            }

            int aliveCount = 0;
            for (int i = 0; i < _coroutines.Count; i++)
            {
                var coroutine = _coroutines[i];
                coroutine.Update(gameTime);

                if (!coroutine.IsFinished)
                {
                    _coroutines[aliveCount++] = coroutine;
                }
            }

            if (aliveCount < _coroutines.Count)
            {
                _coroutines.RemoveRange(aliveCount, _coroutines.Count - aliveCount);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses all running coroutines.
        /// </summary>
        public void PauseAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Pause();
            }
        }

        /// <summary>
        /// Resumes all paused coroutines.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Resume();
            }
        }

        /// <summary>
        /// Stops all running coroutines immediately.
        /// </summary>
        public void StopAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }

            _coroutines.Clear();
            _pendingCoroutines.Clear();
        }

        #endregion

        #region Internal Methods

        internal void Add(Coroutine coroutine)
        {
            _pendingCoroutines.Add(coroutine);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this coroutine controller and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            StopAll();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
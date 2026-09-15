using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Manages the state and execution of all active coroutines.
    /// </summary>
    public class CoroutineManager : IProcessManager
    {
        #region Fields

        /// <summary>
        /// The list of all currently active coroutines.
        /// </summary>
        private readonly List<Coroutine> _coroutines;

        /// <summary>
        /// Pending coroutines to add, processed at the end of the frame to avoid collection
        /// modification during iteration.
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

        /// <summary>
        /// Gets a value indicating whether this manager has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineManager"/> class.
        /// </summary>
        public CoroutineManager()
        {
            _coroutines = [];
            _pendingCoroutines = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates all active coroutines, adding any pending coroutines first.
        /// Called once per frame by the game loop.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

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
        /// Pauses all running and pending coroutines.
        /// </summary>
        public void PauseAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Pause();
            }

            foreach (var coroutine in _pendingCoroutines)
            {
                coroutine.Pause();
            }
        }

        /// <summary>
        /// Resumes all running and pending coroutines.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Resume();
            }

            foreach (var coroutine in _pendingCoroutines)
            {
                coroutine.Resume();
            }
        }

        /// <summary>
        /// Stops all running and pending coroutines immediately, and clears them from this manager.
        /// </summary>
        public void StopAll()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }

            foreach (var coroutine in _pendingCoroutines)
            {
                coroutine.Stop();
            }

            _coroutines.Clear();
            _pendingCoroutines.Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            StopAll();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Queues a coroutine to be added to this manager on the next <see cref="Update"/> call.
        /// </summary>
        /// <param name="coroutine">The coroutine to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="coroutine"/> is <see langword="null"/>.</exception>
        internal void Add(Coroutine coroutine)
        {
            ArgumentNullException.ThrowIfNull(coroutine);

            _pendingCoroutines.Add(coroutine);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this coroutine manager, stopping all coroutines and cleaning up resources.
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
using FreedomEngine.Collections.Interfaces;
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
        private readonly List<Coroutine> _coroutinesToAdd;

        /// <summary>
        /// Pending coroutines to remove, processed at the end of the frame to avoid collection modification during iteration.
        /// </summary>
        private readonly HashSet<Coroutine> _coroutinesToRemove;

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
            _coroutinesToAdd = [];
            _coroutinesToRemove = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates all active coroutines.
        /// Called once per frame by the game loop.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Update all active coroutines
            int coroutineCount = _coroutines.Count;
            for (int i = 0; i < coroutineCount; i++)
            {
                var coroutine = _coroutines[i];
                coroutine.Update(gameTime);

                // Update returns false when coroutine is finished
                if (coroutine.IsFinished)
                {
                    _coroutinesToRemove.Add(coroutine);
                }
            }

            // Process pending operations
            ProcessPendingOperations();
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
            _coroutinesToAdd.Clear();
            _coroutinesToRemove.Clear();
        }

        /// <summary>
        /// Starts a new coroutine from an IEnumerator.
        /// </summary>
        /// <param name="enumerator">The enumerator that defines the coroutine behavior.</param>
        /// <returns>The created coroutine instance that can be used to control execution.</returns>
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                Logger.Error("Cannot start coroutine with null enumerator");
                return null;
            }

            var coroutine = new Coroutine(enumerator);
            _coroutinesToAdd.Add(coroutine);
            return coroutine;
        }

        #endregion

        #region Private Methods

        private void ProcessPendingOperations()
        {
            if (_coroutinesToRemove.Count > 0)
            {
                foreach (var coroutine in _coroutinesToRemove)
                    _coroutines.Remove(coroutine);

                _coroutinesToRemove.Clear();
            }

            if (_coroutinesToAdd.Count > 0)
            {
                _coroutines.AddRange(_coroutinesToAdd);
                _coroutinesToAdd.Clear();
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this coroutine controller and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this coroutine controller and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            StopAll();
        }

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Coroutines
{
    public sealed class CoroutineController : IUpdate
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
        public int ActiveCoroutineCount => _coroutines.Count;

        /// <summary>
        /// Gets whether there are any active coroutines.
        /// </summary>
        public bool HasActiveCoroutines => _coroutines.Count > 0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CoroutineManager"/> class.
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

                // Update returns false when coroutine is finished
                if (!coroutine.Update(gameTime))
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

        /// <summary>
        /// Stops a running coroutine.
        /// </summary>
        /// <param name="coroutine">The coroutine to stop.</param>
        public void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null)
                return;

            coroutine.Stop();
            _coroutinesToRemove.Add(coroutine);
        }

        /// <summary>
        /// Stops all running coroutines immediately.
        /// </summary>
        public void StopAllCoroutines()
        {
            foreach (var coroutine in _coroutines)
            {
                coroutine.Stop();
            }

            _coroutines.Clear();
            _coroutinesToAdd.Clear();
            _coroutinesToRemove.Clear();
        }

        public void Clear()
        {
            StopAllCoroutines();
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
    }
}
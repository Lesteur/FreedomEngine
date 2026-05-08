using System;
using System.Collections;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Represents a coroutine that can be executed over multiple frames.
    /// Wraps an IEnumerator to provide pause/resume functionality similar to Unity.
    /// </summary>
    public sealed class Coroutine
    {
        #region Fields

        /// <summary>
        /// The underlying enumerator that represents the coroutine execution.
        /// </summary>
        private readonly IEnumerator _enumerator;

        /// <summary>
        /// Indicates whether this coroutine has completed execution.
        /// </summary>
        private bool _isFinished;

        /// <summary>
        /// Indicates whether this coroutine is currently paused.
        /// </summary>
        private bool _isPaused;

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether this coroutine has finished executing.
        /// </summary>
        public bool IsFinished => _isFinished;

        /// <summary>
        /// Gets whether this coroutine is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets whether this coroutine is currently running (not paused and not finished).
        /// </summary>
        public bool IsRunning => !_isFinished && !_isPaused;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Coroutine"/> class.
        /// </summary>
        /// <param name="enumerator">The enumerator that represents the coroutine logic.</param>
        public Coroutine(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _isFinished = false;
            _isPaused = false;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the coroutine by one step.
        /// Called by the CoroutineManager each frame.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last frame.</param>
        /// <returns>True if the coroutine should continue, false if it has finished.</returns>
        internal bool Update(GameTime gameTime)
        {
            if (_isFinished || _isPaused)
                return !_isFinished;

            // Check if current yield instruction needs to wait
            if (_enumerator.Current is IYieldInstruction yieldInstruction)
            {
                if (!yieldInstruction.IsDone(gameTime.ElapsedGameTime))
                    return true;
            }

            // Move to next step
            if (!_enumerator.MoveNext())
            {
                _isFinished = true;
                return false;
            }

            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses the execution of this coroutine.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resumes the execution of this coroutine.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Stops the execution of this coroutine immediately.
        /// </summary>
        public void Stop()
        {
            _isFinished = true;
        }

        #endregion
    }
}
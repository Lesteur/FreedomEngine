using System;
using System.Collections;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.Coroutines
{
    /// <summary>
    /// Represents a coroutine that can be executed over multiple frames.
    /// </summary>
    /// <remarks>
    /// A coroutine wraps an <see cref="IEnumerator"/>, typically produced by a C# iterator method,
    /// and advances it one step per <see cref="Update"/> call. Yielding an <see cref="IYieldInstruction"/>
    /// pauses that advance until the instruction reports itself done.
    /// </remarks>
    public sealed class Coroutine : IControllableProcess
    {
        #region Fields

        /// <summary>
        /// The underlying enumerator that represents the coroutine execution.
        /// </summary>
        private readonly IEnumerator _enumerator;

        /// <summary>
        /// Indicates whether this coroutine is currently paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Indicates whether this coroutine has completed execution.
        /// </summary>
        private bool _isFinished;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the manager that newly created coroutines are registered with.
        /// </summary>
        /// <remarks>
        /// Must be assigned before constructing a <see cref="Coroutine"/>; the engine assigns this
        /// as part of a scene transition. See <see cref="FreedomEngine.Core.Application.ChangeScene"/>.
        /// </remarks>
        public static ProcessManager<Coroutine> Controller { get; set; }

        /// <summary>
        /// Gets whether this coroutine is currently paused.
        /// </summary>
        public bool IsPaused => _isPaused;

        /// <summary>
        /// Gets whether this coroutine has finished executing.
        /// </summary>
        public bool IsFinished => _isFinished;

        /// <summary>
        /// Gets whether this coroutine is currently running (not paused and not finished).
        /// </summary>
        public bool IsRunning => !_isFinished && !_isPaused;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Coroutine"/> class and registers it with
        /// <see cref="Controller"/>.
        /// </summary>
        /// <param name="enumerator">The enumerator that represents the coroutine logic.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Controller"/> has not been assigned.</exception>
        public Coroutine(IEnumerator enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));

            if (Controller == null)
                throw new InvalidOperationException($"{nameof(Coroutine)}.{nameof(Controller)} must be assigned a {nameof(ProcessManager<Coroutine>)} before creating a coroutine.");

            Controller.Add(this);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Advances the coroutine by one step: waiting on the current yield instruction if it is not
        /// yet done, otherwise moving to the next step of the underlying enumerator.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            if (_isFinished || _isPaused)
                return;

            // Check if current yield instruction needs to wait
            if (_enumerator.Current is IYieldInstruction yieldInstruction)
            {
                if (!yieldInstruction.IsDone(gameTime.ElapsedGameTime))
                    return;
            }

            // Move to next step
            if (!_enumerator.MoveNext())
            {
                _isFinished = true;
            }
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
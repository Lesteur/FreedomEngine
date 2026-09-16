using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections
{
    public class ProcessManager<TProcess> : IProcessManager where TProcess : IControllableProcess
    {
        #region Fields

        /// <summary>
        /// The list of all currently active tweens.
        /// </summary>
        private readonly List<TProcess> _processes;

        /// <summary>
        /// Pending tweens to add, processed at the end of the frame to avoid collection modification
        /// during iteration.
        /// </summary>
        private readonly List<TProcess> _pendingProcesses;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of currently active processes.
        /// </summary>
        public int ActiveCount => _processes.Count;

        /// <summary>
        /// Gets whether there are any active processes.
        /// </summary>
        public bool HasActiveProcesses => _processes.Count > 0;

        /// <summary>
        /// Gets a value indicating whether this manager has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessManager{TProcess}"/> class.
        /// </summary>
        public ProcessManager()
        {
            _processes = [];
            _pendingProcesses = [];
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state of all active processes, progressing their execution based on the elapsed time.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            if (_pendingProcesses.Count > 0)
            {
                _processes.AddRange(_pendingProcesses);
                _pendingProcesses.Clear();
            }

            int aliveCount = 0;
            for (int i = 0; i < _processes.Count; i++)
            {
                var process = _processes[i];
                process.Update(gameTime);

                if (!process.IsFinished)
                    _processes[aliveCount++] = process;
            }

            if (aliveCount < _processes.Count)
                _processes.RemoveRange(aliveCount, _processes.Count - aliveCount);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Pauses all running and pending processes.
        /// </summary>
        public void PauseAll()
        {
            foreach (var process in _processes)
                process.Pause();

            foreach (var process in _pendingProcesses)
                process.Pause();
        }

        /// <summary>
        /// Resumes all running and pending processes.
        /// </summary>
        public void ResumeAll()
        {
            foreach (var process in _processes)
                process.Resume();

            foreach (var process in _pendingProcesses)
                process.Resume();
        }

        /// <summary>
        /// Stops all running and pending processes immediately, and clears them from this manager.
        /// </summary>
        public void StopAll()
        {
            foreach (var process in _processes)
                process.Stop();

            foreach (var process in _pendingProcesses)
                process.Stop();

            _processes.Clear();
            _pendingProcesses.Clear();
        }

        /// <inheritdoc/>
        public void Clear()
        {
            StopAll();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Queues a process to be added to this manager on the next <see cref="Update"/> call.
        /// </summary>
        /// <param name="process">The process to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="process"/> is <see langword="null"/>.</exception>
        internal void Add(TProcess process)
        {
            ArgumentNullException.ThrowIfNull(process);

            _pendingProcesses.Add(process);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this process manager, stopping all processes and cleaning up resources.
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
using System;

namespace FreedomEngine.Collections.Interfaces
{
    /// <summary>
    /// Represents a manager that updates and controls a collection of processes.
    /// </summary>
    public interface IProcessManager : IUpdate, IDisposable
    {
        /// <summary>
        /// Gets the number of active processes managed by this instance.
        /// </summary>
        public int ActiveCount { get; }

        /// <summary>
        /// Gets a value indicating whether the manager currently tracks at least one active process.
        /// </summary>
        public bool HasActiveProcesses { get; }

        /// <summary>
        /// Pauses all managed processes.
        /// </summary>
        public void PauseAll();

        /// <summary>
        /// Resumes all managed processes.
        /// </summary>
        public void ResumeAll();

        /// <summary>
        /// Stops all managed processes immediately.
        /// </summary>
        public void StopAll();

        /// <summary>
        /// Stops and removes all managed processes, releasing any resources they hold. Typically
        /// called when transitioning between scenes or game states.
        /// </summary>
        public void Clear();
    }
}
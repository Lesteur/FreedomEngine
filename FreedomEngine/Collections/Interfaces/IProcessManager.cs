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
        int ActiveCount { get; }

        /// <summary>
        /// Gets whether the manager currently tracks at least one process.
        /// </summary>
        bool HasActiveProcesses { get; }

        /// <summary>
        /// Pauses all managed processes.
        /// </summary>
        void PauseAll();

        /// <summary>
        /// Resumes all managed processes.
        /// </summary>
        void ResumeAll();

        /// <summary>
        /// Stops all managed processes immediately.
        /// </summary>
        void StopAll();
    }
}
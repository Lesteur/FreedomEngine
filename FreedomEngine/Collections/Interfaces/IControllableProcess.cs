using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Interfaces
{
    /// <summary>
    /// Represents an updatable process that can be paused, resumed, or stopped.
    /// </summary>
    public interface IControllableProcess : IUpdate
    {
        /// <summary>
        /// Gets whether the process is currently paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Gets whether the process has stopped or finished.
        /// </summary>
        bool IsFinished { get; }

        /// <summary>
        /// Gets whether the process is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Pauses the process execution.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes the process execution.
        /// </summary>
        void Resume();

        /// <summary>
        /// Stops the process execution immediately.
        /// </summary>
        void Stop();
    }
}
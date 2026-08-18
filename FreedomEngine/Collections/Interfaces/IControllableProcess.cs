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
        public bool IsPaused { get; }

        /// <summary>
        /// Gets whether the process has stopped or finished.
        /// </summary>
        public bool IsFinished { get; }

        /// <summary>
        /// Gets whether the process is currently running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Pauses the process execution.
        /// </summary>
        public void Pause();

        /// <summary>
        /// Resumes the process execution.
        /// </summary>
        public void Resume();

        /// <summary>
        /// Stops the process execution immediately.
        /// </summary>
        public void Stop();
    }
}
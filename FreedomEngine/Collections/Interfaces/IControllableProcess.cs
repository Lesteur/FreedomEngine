namespace FreedomEngine.Collections.Interfaces
{
    /// <summary>
    /// Represents an updatable process that can be paused, resumed, or stopped.
    /// </summary>
    public interface IControllableProcess : IUpdate
    {
        /// <summary>
        /// Gets a value indicating whether the process is currently paused.
        /// </summary>
        /// <remarks>
        /// While paused, an implementation is expected to skip its update logic until
        /// <see cref="Resume"/> is called.
        /// </remarks>
        public bool IsPaused { get; }

        /// <summary>
        /// Gets a value indicating whether the process has stopped or finished running.
        /// </summary>
        /// <remarks>
        /// Once <see langword="true"/>, the process is not expected to produce further updates.
        /// </remarks>
        public bool IsFinished { get; }

        /// <summary>
        /// Gets a value indicating whether the process is currently running.
        /// </summary>
        /// <remarks>
        /// A process is expected to be considered running only while it is neither paused nor
        /// finished. Pausing a process is expected to make this <see langword="false"/> until
        /// <see cref="Resume"/> is called.
        /// </remarks>
        public bool IsRunning { get; }

        /// <summary>
        /// Pauses the process, suspending its update logic until <see cref="Resume"/> is called.
        /// </summary>
        public void Pause();

        /// <summary>
        /// Resumes the process, allowing its update logic to run again after a call to <see cref="Pause"/>.
        /// </summary>
        public void Resume();

        /// <summary>
        /// Stops the process immediately, without waiting for it to reach a natural end point.
        /// </summary>
        public void Stop();
    }
}
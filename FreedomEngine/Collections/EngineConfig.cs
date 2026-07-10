namespace FreedomEngine.Collections
{
    /// <summary>
    /// Defines global configuration constants for the game engine.
    /// Contains settings for virtual resolution, window dimensions, and frame rate behavior.
    /// These values are compile-time constants and cannot be changed at runtime.
    /// </summary>
    public static class EngineConfig
    {
        #region Virtual Resolution

        /// <summary>
        /// The internal virtual width used for rendering, in pixels.
        /// </summary>
        public const int VirtualWidth = 640;

        /// <summary>
        /// The internal virtual height used for rendering, in pixels.
        /// </summary>
        public const int VirtualHeight = 360;

        #endregion

        #region UI Rendering Settings

        public const int UIRenderWidth = 640;

        public const int UIRenderHeight = 360;

        #endregion

        #region Window Settings

        /// <summary>
        /// The width of the game window, in pixels.
        /// </summary>
        public const int WindowWidth = 1280;

        /// <summary>
        /// The height of the game window, in pixels.
        /// </summary>
        public const int WindowHeight = 720;

        #endregion

        #region Frame Rate Settings

        /// <summary>
        /// Indicates whether vertical synchronization (VSync) is enabled.
        /// When true, the frame rate is synchronized with the display's refresh rate to prevent screen tearing.
        /// </summary>
        public const bool VSync = true;

        /// <summary>
        /// Indicates whether the game uses a fixed time step for updates.
        /// When true, the Update method is called at a consistent rate defined by <see cref="TargetFPS"/>.
        /// This ensures deterministic gameplay regardless of rendering performance.
        /// </summary>
        public const bool IsFixedTimeStep = true;

        /// <summary>
        /// The target frame rate for game updates, in frames per second.
        /// Only applies when <see cref="IsFixedTimeStep"/> is true.
        /// Defines how many times per second the Update method should be called.
        /// </summary>
        public const int TargetFPS = 60;

        #endregion
    }
}
namespace FreedomEngine.Collections
{
    /// <summary>
    /// Defines global configuration constants for the game engine: virtual resolution, window
    /// dimensions, and frame rate behavior.
    /// </summary>
    /// <remarks>
    /// Every value here is a compile-time constant; none can be changed at runtime. The actual
    /// current window or back buffer size, which can differ from <see cref="WindowWidth"/> and
    /// <see cref="WindowHeight"/> once the window is resized or toggled to fullscreen, is read from
    /// <c>Application.GraphicsDevice</c> instead.
    /// </remarks>
    public static class EngineConfig
    {
        #region Virtual Resolution

        /// <summary>
        /// The internal virtual width used for rendering, in pixels.
        /// </summary>
        /// <remarks>
        /// Also the width of the coordinate space that <c>UIElement</c> hit-testing and
        /// <c>MouseInfo.VirtualPosition</c> work in, so a UI camera and the mouse always agree on
        /// where things are.
        /// </remarks>
        public const int VirtualWidth = 640;

        /// <summary>
        /// The internal virtual height used for rendering, in pixels.
        /// </summary>
        /// <remarks>
        /// Also the height of the coordinate space that <c>UIElement</c> hit-testing and
        /// <c>MouseInfo.VirtualPosition</c> work in, so a UI camera and the mouse always agree on
        /// where things are.
        /// </remarks>
        public const int VirtualHeight = 360;

        #endregion

        #region Window Settings

        /// <summary>
        /// The initial width of the game window, in pixels, applied when the graphics device is configured.
        /// </summary>
        /// <remarks>
        /// This is only the starting size. If the window can be resized or toggled to fullscreen at
        /// runtime, the actual current size no longer matches this constant — read it from
        /// <c>Application.GraphicsDevice.Viewport</c> instead.
        /// </remarks>
        public const int WindowWidth = 1280;

        /// <summary>
        /// The initial height of the game window, in pixels, applied when the graphics device is configured.
        /// </summary>
        /// <remarks>
        /// This is only the starting size. If the window can be resized or toggled to fullscreen at
        /// runtime, the actual current size no longer matches this constant — read it from
        /// <c>Application.GraphicsDevice.Viewport</c> instead.
        /// </remarks>
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
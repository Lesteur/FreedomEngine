using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Input
{
    /// <summary>
    /// Provides access to the current and previous keyboard states, enabling
    /// detection of key presses, releases, and real-time keyboard input changes.
    /// </summary>
    public class KeyboardInfo : IUpdate
    {
        #region Properties

        /// <summary>
        /// Gets the state of keyboard input during the previous update cycle.
        /// </summary>
        public KeyboardState PreviousState { get; private set; }

        /// <summary>
        /// Gets the state of keyboard input during the current input cycle.
        /// </summary>
        public KeyboardState CurrentState { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="KeyboardInfo"/> class, initializing the previous and current keyboard states.
        /// </summary>
        public KeyboardInfo()
        {
            PreviousState = new KeyboardState();
            CurrentState = Keyboard.GetState();
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state information about keyboard input.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a value that indicates if the specified key is currently down.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the specified key is currently down; otherwise, false.</returns>
        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified key is currently up.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the specified key is currently up; otherwise, false.</returns>
        public bool IsKeyUp(Keys key)
        {
            return CurrentState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns a value that indicates if the specified key was just pressed on the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the specified key was just pressed on the current frame; otherwise, false.</returns>
        public bool WasKeyJustPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        /// <summary>
        /// Returns a value that indicates if the specified key was just released on the current frame.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>true if the specified key was just released on the current frame; otherwise, false.</returns>
        public bool WasKeyJustReleased(Keys key)
        {
            return CurrentState.IsKeyUp(key) && PreviousState.IsKeyDown(key);
        }

        #endregion
    }
}

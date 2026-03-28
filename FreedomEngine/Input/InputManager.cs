using Microsoft.Xna.Framework;

namespace FreedomEngine.Input
{
    /// <summary>
    /// Manages input from keyboard, mouse, and up to four gamepads, providing
    /// access to their current state for interactive applications.
    /// </summary>
    public class InputManager
    {
        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="InputManager"/> class, initializing the keyboard, mouse, and gamepad input states.
        /// </summary>
        public InputManager()
        {
            Keyboard = new KeyboardInfo();
            Mouse = new MouseInfo();

            GamePads = new GamePadInfo[4];
            for (int i = 0; i < 4; i++)
            {
                GamePads[i] = new GamePadInfo((PlayerIndex)i);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the state information of keyboard input.
        /// </summary>
        public KeyboardInfo Keyboard { get; private set; }

        /// <summary>
        /// Gets the state information of mouse input.
        /// </summary>
        public MouseInfo Mouse { get; private set; }

        /// <summary>
        /// Gets the state information of a gamepad.
        /// </summary>
        public GamePadInfo[] GamePads { get; private set; }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state information for the keyboard, mouse, and gamepad inputs.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        public void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();

            for (int i = 0; i < 4; i++)
            {
                GamePads[i].Update(gameTime);
            }
        }

        #endregion
    }
}

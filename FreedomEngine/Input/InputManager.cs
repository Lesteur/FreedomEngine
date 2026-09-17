using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Input
{
    /// <summary>
    /// Manages input from keyboard, mouse, and up to four gamepads, providing
    /// access to their current state for interactive applications.
    /// </summary>
    public class InputManager : IUpdate
    {
        #region Constants

        /// <summary>
        /// The number of gamepad slots tracked, matching <see cref="Microsoft.Xna.Framework.Input.GamePad"/>'s
        /// maximum of four simultaneously connected controllers.
        /// </summary>
        private const int MaxGamePads = 4;

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
        /// Gets the state information of each tracked gamepad, indexed by the numeric value of its
        /// <see cref="PlayerIndex"/> (<c>GamePads[0]</c> is <see cref="PlayerIndex.One"/>, and so on).
        /// </summary>
        /// <remarks>
        /// Always has <see cref="MaxGamePads"/> entries, one per slot, whether or not a controller is
        /// physically connected to it — check <see cref="GamePadInfo.IsConnected"/> on the entry itself.
        /// </remarks>
        public GamePadInfo[] GamePads { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="InputManager"/> class, initializing the keyboard, mouse, and gamepad input states.
        /// </summary>
        public InputManager()
        {
            Keyboard = new KeyboardInfo();
            Mouse = new MouseInfo();

            GamePads = new GamePadInfo[MaxGamePads];
            for (int i = 0; i < MaxGamePads; i++)
                GamePads[i] = new GamePadInfo((PlayerIndex)i);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state information for the keyboard, mouse, and gamepad inputs.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            Keyboard.Update(gameTime);
            Mouse.Update(gameTime);

            for (int i = 0; i < MaxGamePads; i++)
                GamePads[i].Update(gameTime);
        }

        #endregion
    }
}
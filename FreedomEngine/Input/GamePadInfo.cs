using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Input
{
    /// <summary>
    /// Represents the state and input information for a gamepad,
    /// including player index, button states, and thumbstick values.
    /// </summary>
    public class GamePadInfo : IUpdate
    {
        #region Fields

        /// <summary>
        /// The amount of time remaining for the current vibration effect on this gamepad.
        /// </summary>
        private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the index of the player this gamepad is for.
        /// </summary>
        public PlayerIndex PlayerIndex { get; }

        /// <summary>
        /// Gets the state of input for this gamepad during the previous update cycle.
        /// </summary>
        public GamePadState PreviousState { get; private set; }

        /// <summary>
        /// Gets the state of input for this gamepad during the current update cycle.
        /// </summary>
        public GamePadState CurrentState { get; private set; }

        /// <summary>
        /// Gets a value that indicates if this gamepad is currently connected.
        /// </summary>
        public bool IsConnected => CurrentState.IsConnected;

        /// <summary>
        /// Gets a value indicating whether a vibration effect started with <see cref="SetVibration(float, TimeSpan)"/>
        /// is currently running on this gamepad.
        /// </summary>
        public bool IsVibrating => _vibrationTimeRemaining > TimeSpan.Zero;

        /// <summary>
        /// Gets the value of the left thumbstick of this gamepad.
        /// </summary>
        public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;

        /// <summary>
        /// Gets the value of the right thumbstick of this gamepad.
        /// </summary>
        public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;

        /// <summary>
        /// Gets the value of the left trigger of this gamepad.
        /// </summary>
        public float LeftTrigger => CurrentState.Triggers.Left;

        /// <summary>
        /// Gets the value of the right trigger of this gamepad.
        /// </summary>
        public float RightTrigger => CurrentState.Triggers.Right;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="GamePadInfo"/> class for the specified player index.
        /// </summary>
        /// <param name="playerIndex">The index of the player for this gamepad.</param>
        public GamePadInfo(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
            PreviousState = new GamePadState();
            CurrentState = GamePad.GetState(playerIndex);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the state information for this gamepad input, and counts down any active vibration effect.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            PreviousState = CurrentState;
            CurrentState = GamePad.GetState(PlayerIndex);

            if (_vibrationTimeRemaining > TimeSpan.Zero)
            {
                _vibrationTimeRemaining -= gameTime.ElapsedGameTime;

                if (_vibrationTimeRemaining <= TimeSpan.Zero)
                    StopVibration();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button is current down.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <returns>true if the specified gamepad button is currently down; otherwise, false.</returns>
        public bool IsButtonDown(Buttons button)
        {
            return CurrentState.IsButtonDown(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button is currently up.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <returns>true if the specified gamepad button is currently up; otherwise, false.</returns>
        public bool IsButtonUp(Buttons button)
        {
            return CurrentState.IsButtonUp(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button was just pressed on the current frame.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <returns>true if the specified gamepad button was just pressed on the current frame; otherwise, false.</returns>
        public bool WasButtonJustPressed(Buttons button)
        {
            return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
        }

        /// <summary>
        /// Returns a value that indicates whether the specified gamepad button was just released on the current frame.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <returns>true if the specified gamepad button was just released on the current frame; otherwise, false.</returns>
        public bool WasButtonJustReleased(Buttons button)
        {
            return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
        }

        /// <summary>
        /// Sets the vibration for both motors of this gamepad to the same strength.
        /// </summary>
        /// <param name="strength">The strength of the vibration, from 0.0f (none) to 1.0f (full).</param>
        /// <param name="time">The amount of time the vibration should run for.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="strength"/> is outside the range [0.0, 1.0], or <paramref name="time"/> is negative.
        /// </exception>
        public void SetVibration(float strength, TimeSpan time)
        {
            SetVibration(strength, strength, time);
        }

        /// <summary>
        /// Sets the vibration for this gamepad's low-frequency (left) and high-frequency (right) motors independently.
        /// </summary>
        /// <param name="leftMotor">The strength of the left, low-frequency motor, from 0.0f (none) to 1.0f (full).</param>
        /// <param name="rightMotor">The strength of the right, high-frequency motor, from 0.0f (none) to 1.0f (full).</param>
        /// <param name="time">The amount of time the vibration should run for.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="leftMotor"/> or <paramref name="rightMotor"/> is outside the range
        /// [0.0, 1.0], or <paramref name="time"/> is negative.
        /// </exception>
        public void SetVibration(float leftMotor, float rightMotor, TimeSpan time)
        {
            if (leftMotor < 0.0f || leftMotor > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(leftMotor), leftMotor, "Motor strength must be between 0.0 and 1.0.");

            if (rightMotor < 0.0f || rightMotor > 1.0f)
                throw new ArgumentOutOfRangeException(nameof(rightMotor), rightMotor, "Motor strength must be between 0.0 and 1.0.");

            if (time < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(time), time, "Time cannot be negative.");

            _vibrationTimeRemaining = time;
            GamePad.SetVibration(PlayerIndex, leftMotor, rightMotor);
        }

        /// <summary>
        /// Stops the vibration of all motors for this gamepad.
        /// </summary>
        /// <remarks>Also cancels any vibration timer started by <see cref="SetVibration(float, TimeSpan)"/>.</remarks>
        public void StopVibration()
        {
            _vibrationTimeRemaining = TimeSpan.Zero;
            GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
        }

        #endregion
    }
}
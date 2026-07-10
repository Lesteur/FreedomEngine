using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;

namespace MyGame.Scripts.Scenes
{
    public class Player : ObjectPhysic
    {
        // Jump parameters
        private int _jumpBufferTime = 30;
        private bool _jumpKeyBuffered = false;
        private int _jumpKeyBufferedTimer = 0;
        private int _jumpMax = 2;
        private int _jumpCount = 0;
        private bool _jumpKey = false;
        private bool _jumpKeyPressed = false;

        private readonly int[] _jumpHoldFrames = [18, 10];
        private readonly float[] _jumpSpeed = [-3.4f, -3.1f];

        public Player(Sprite sprite, Vector2 position, CollisionMask collision) : base(sprite, position, collision)
        {
            // Initialize player-specific properties here if needed
        }

        public override void Update(GameTime gameTime)
        {
            GetControllerInput();

            // Calculate base frame speeds
            _xSpeed = HandlePlayerMovement();
            _ySpeed = HandleGravity();

            // Process jump logic and assign the correct vertical speed
            _ySpeed = HandlePlayerJump();

            base.Update(gameTime);

            if (Core.Input.Keyboard.IsKeyDown(Keys.Enter))
            {
                Logger.Info($"Position: {Position}");
            }
        }

        private void GetControllerInput()
        {
            _jumpKey = Core.Input.Keyboard.IsKeyDown(Keys.Space);
            _jumpKeyPressed = Core.Input.Keyboard.WasKeyJustPressed(Keys.Space);

            // Jump key buffering logic
            if (_jumpKeyPressed)
            {
                _jumpKeyBufferedTimer = _jumpBufferTime;
            }

            if (_jumpKeyBufferedTimer > 0)
            {
                _jumpKeyBuffered = true;
                _jumpKeyBufferedTimer--;
            }
            else
            {
                _jumpKeyBuffered = false;
            }
        }

        private float HandlePlayerJump()
        {
            float ySpeed = _ySpeed;

            // Reset or prepare jumping variables based on ground state
            if (_onGround)
            {
                _jumpCount = 0;
                _jumpHoldTimer = 0; // Fixes IndexOutOfRangeException when landing while holding jump
                _coyoteJumpTimer = _coyoteJumpFrames;
            }
            else
            {
                _coyoteJumpTimer--;

                // Consume the first jump if coyote time window expires in mid-air
                if (_jumpCount == 0 && _coyoteJumpTimer <= 0)
                {
                    _jumpCount = 1;
                }
            }

            // Initiate the jump (Removed _onGround check to allow mid-air double jumping)
            if (_jumpKeyBuffered && _jumpCount < _jumpMax)
            {
                // Reset the input buffer
                _jumpKeyBuffered = false;
                _jumpKeyBufferedTimer = 0;

                // Track current jump index
                _jumpCount++;

                // Set the jump frames hold duration for variable height
                _jumpHoldTimer = _jumpHoldFrames[_jumpCount - 1];

                // Player leaves the ground immediately
                SetOnGround(false);
            }

            // Stop upward acceleration early if the player releases the jump key
            if (!_jumpKey)
            {
                _jumpHoldTimer = 0;
            }

            // Apply variable jump speed while hold timer is active
            if (_jumpHoldTimer > 0)
            {
                ySpeed = _jumpSpeed[_jumpCount - 1];
                _jumpHoldTimer--;
            }

            return ySpeed;
        }

        private float HandlePlayerMovement()
        {
            float xSpeed = 0f;
            int moveDirection = 0;

            if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                moveDirection -= 1;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                moveDirection += 1;
            }

            if (moveDirection != 0)
            {
                xSpeed = moveDirection * 2f;
            }

            return xSpeed;
        }
    }
}
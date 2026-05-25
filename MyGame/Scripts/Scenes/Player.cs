using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FreedomEngine.Collections;
using FreedomEngine.Components;
using FreedomEngine.Graphics;
using FreedomEngine.Core;
using FreedomEngine.Components.Collisions;

namespace MyGame.Scripts.Scenes
{
    public class Player : Entity
    {
        private float _xSpeed = 0f;
        private float _ySpeed = 0f;

        // Gravity & Physics parameters
        private float _grav = .3f;
        private float _maxFallSpeed = 4f;
        private bool _onGround = false;

        // Jump parameters
        private int _jumpBufferTime = 30;
        private bool _jumpKeyBuffered = false;
        private int _jumpKeyBufferedTimer = 0;
        private int _jumpMax = 2;
        private int _jumpCount = 0;
        private int _jumpHoldTimer = 0;
        private bool _jumpKey = false;
        private bool _jumpKeyPressed = false;

        private int[] _jumpHoldFrames = new int[2] { 18, 10 };
        private float[] _jumpSpeed = new float[2] { -3.4f, -3.1f };

        // Hang time / Coyote time parameters
        private int _coyoteHangFrames = 2;
        private int _coyoteHangTimer = 0;

        private int _coyoteJumpFrames = 6;
        private int _coyoteJumpTimer = 0;

        public Player(Sprite sprite, int x, int y) : base(sprite, x, y)
        {
        }

        public override void Update(GameTime gameTime)
        {
            GetControllerInput();

            // Calculate base frame speeds
            _xSpeed = HandlePlayerMovement();
            _ySpeed = HandleGravity();

            // Process jump logic and assign the correct vertical speed
            _ySpeed = HandlePlayerJump();

            // Clamp falling speed before applying delta time
            if (_ySpeed > _maxFallSpeed)
            {
                _ySpeed = _maxFallSpeed;
            }

            // Apply Delta Time multiplier to get exact pixel movement for this frame
            float dtMultiplier = (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
            float moveX = _xSpeed * dtMultiplier;
            float moveY = _ySpeed * dtMultiplier;

            // Handle horizontal and vertical collisions using delta-scaled movement
            HandleXSpeed(ref moveX);
            HandleYSpeed(ref moveY);

            // Apply final validated movement to position
            Position += new Vector2(moveX, moveY);

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                Logger.Info($"Position: {Position}, OnGround: {_onGround}, CoyoteHangTimer: {_coyoteHangTimer}");
                Logger.Info($"Width: {Width}, Height: {Height}");
            }

            base.Update(gameTime);
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

        private void SetOnGround(bool onGround = true)
        {
            if (onGround)
            {
                _onGround = true;
                _coyoteHangTimer = _coyoteHangFrames;
            }
            else
            {
                _onGround = false;
                _coyoteHangTimer = 0;
            }
        }

        private float HandleGravity()
        {
            float ySpeed = _ySpeed;

            if (_coyoteHangTimer > 0)
            {
                _coyoteHangTimer--;
            }
            else if (!_onGround)
            {
                ySpeed += _grav;
            }

            return ySpeed;
        }

        private void HandleXSpeed(ref float moveX)
        {
            // Check horizontal collision for the entire intended frame movement
            if (CollidesWith(1, new Vector2(moveX, 0)))
            {
                float signX = Math.Sign(moveX);

                // Pixel-perfect approach loop
                while (!CollidesWith(1, new Vector2(signX, 0)))
                {
                    Position += new Vector2(signX, 0);
                }

                moveX = 0;
                _xSpeed = 0;
            }
        }

        private void HandleYSpeed(ref float moveY)
        {
            // Check vertical collision for the entire intended frame movement
            if (CollidesWith(1, new Vector2(0, moveY)))
            {
                float signY = Math.Sign(moveY);

                // Pixel-perfect approach loop
                while (!CollidesWith(1, new Vector2(0, signY)))
                {
                    Position += new Vector2(0, signY);
                }

                if (signY > 0) // Landed on the ground
                {
                    SetOnGround(true);
                }
                else if (signY < 0) // Hit a ceiling
                {
                    _jumpHoldTimer = 0; // Instantly cancel upward jump acceleration
                }

                moveY = 0;
                _ySpeed = 0;
            }
            else
            {
                // Verify if player is still riding the floor while moving down or stationary
                if (_ySpeed >= 0 && CollidesWith(1, new Vector2(0, 1f)))
                {
                    SetOnGround(true);
                }
                else
                {
                    _onGround = false;
                }
            }
        }
    }
}
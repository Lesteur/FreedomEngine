using FreedomEngine.Collections;
using FreedomEngine.Components;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MyGame.Scripts.Scenes
{
    public abstract class ObjectPhysic : Entity
    {
        #region Fields

        // Speed variables
        protected float _xSpeed = 0f;
        protected float _ySpeed = 0f;

        // Gravity & Physics parameters
        protected float _grav = .3f;
        protected float _maxFallSpeed = 4f;
        protected bool _onGround = false;

        // Hang time / Coyote time parameters
        protected int _coyoteHangFrames = 10;
        protected int _coyoteHangTimer = 0;
        protected int _coyoteJumpFrames = 6;
        protected int _coyoteJumpTimer = 0;

        protected int _jumpHoldTimer = 0;

        #endregion

        #region Constructors

        public ObjectPhysic(Sprite sprite, Vector2 position) : base(sprite, position)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
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

            base.Update(gameTime);
        }

        #endregion

        #region Protected Methods

        protected void SetOnGround(bool onGround = true)
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

        protected float HandleGravity()
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

        protected void HandleXSpeed(ref float moveX)
        {
            // Check horizontal collision for the entire intended frame movement
            if (CollidesWith(1, new Vector2(moveX, 0)))
            {
                float signX = Math.Sign(moveX);

                // Pixel-perfect approach loop
                while (!CollidesWith(1, new Vector2(signX, 0)))
                {
                    Position += new Vector2(signX, 0);
                    Position = new Vector2((float)Math.Round(Position.X), Position.Y); // Prevent sub-pixel sticking issues
                }

                moveX = 0;
                _xSpeed = 0;
            }
        }

        protected void HandleYSpeed(ref float moveY)
        {
            // Check vertical collision for the entire intended frame movement
            if (CollidesWith(1, new Vector2(0, moveY)))
            {
                float signY = Math.Sign(moveY);

                // Pixel-perfect approach loop
                while (!CollidesWith(1, new Vector2(0, signY)))
                {
                    Position += new Vector2(0, signY);
                    // Position = new Vector2(Position.X, Position.Y); // Prevent sub-pixel sticking issues
                }

                Position = new Vector2(Position.X, (float)Math.Ceiling(Position.Y));

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

        #endregion
    }
}

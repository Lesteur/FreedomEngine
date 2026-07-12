using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Graphics;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public abstract class PhysicalEntity : Entity
    {
        #region Fields

        protected CollisionMask _currentGround;

        protected float _xSpeed = 0f;

        protected float _ySpeed = 0f;

        protected float _grav = .3f;

        protected float _maxFallSpeed = 4f;

        protected bool _onGround = false;

        protected int _coyoteHangFrames = 10;

        protected int _coyoteHangTimer = 0;

        protected int _coyoteJumpFrames = 6;

        protected int _coyoteJumpTimer = 0;

        protected int _jumpHoldTimer = 0;

        protected uint _maskCollisionSolid = 1;

        #endregion

        #region Constructors

        public PhysicalEntity(Sprite sprite, Vector2 position, CollisionMask collision = null) : base(sprite, position, collision)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds * EngineConfig.TargetFPS;
            var moveX = _xSpeed * dt;
            var moveY = _ySpeed * dt;

            HandleMovePlatforms();

            HandleXSpeed(ref moveX);
            HandleYSpeed(ref moveY);

            Position += new Vector2(moveX, moveY);

            HandleFinalMovePlatforms();

            base.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public void SetOnGround(bool onGround = true, CollisionMask ground = null)
        {
            if (onGround)
            {
                _onGround = true;
                _coyoteHangTimer = _coyoteHangFrames;
                _currentGround = ground;
            }
            else
            {
                _onGround = false;
                _coyoteHangTimer = 0;
                _currentGround = null;
            }
        }

        public float HandleGravity()
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

            if (ySpeed > _maxFallSpeed)
            {
                ySpeed = _maxFallSpeed;
            }

            return ySpeed;
        }

        public void HandleXSpeed(ref float moveX)
        {
            if (CollidesWith(_maskCollisionSolid, new Vector2(moveX, 0)))
            {
                float signX = Math.Sign(moveX);

                while (!CollidesWith(_maskCollisionSolid, new Vector2(signX, 0)))
                {
                    Position += new Vector2(signX, 0);
                    Position = new Vector2((float)Math.Round(Position.X), Position.Y); // Prevent sub-pixel sticking issues
                }

                moveX = 0;
                _xSpeed = 0;
            }
        }

        public void HandleYSpeed(ref float moveY)
        {
            var collision = CollidesWithInstance(_maskCollisionSolid, new Vector2(0, moveY));

            if (collision != null)
            {
                float signY = Math.Sign(moveY);

                var col = CollidesWithInstance(_maskCollisionSolid, new Vector2(0, signY));
                while (col == null)
                {
                    collision = col;
                    Position += new Vector2(0, signY);

                    col = CollidesWithInstance(_maskCollisionSolid, new Vector2(0, signY));
                }

                Position = new Vector2(Position.X, (float)Math.Ceiling(Position.Y));
                
                if (signY > 0)
                {
                    SetOnGround(true, collision);
                }
                else
                if (signY < 0)
                {
                    _jumpHoldTimer = 0;
                }

                moveY = 0;
                _ySpeed = 0;
            }
            else
            {
                collision = CollidesWithInstance(_maskCollisionSolid, new Vector2(0, 1f));
                if (_ySpeed >= 0 && collision != null)
                {
                    SetOnGround(true, collision);
                }
                else
                {
                    _currentGround = null;
                    _onGround = false;
                }
            }
        }

        public void HandleMovePlatforms()
        {

        }

        public void HandleFinalMovePlatforms()
        {
            float movePlatXSpeed = 0f;
            float movePlatMaxYSpeed = _maxFallSpeed;

            if (_currentGround != null && _currentGround.Collider is MovingPlatform movingPlatform)
            {
                movePlatXSpeed = movingPlatform.XSpeed;
            }

            if (CollidesWith(_maskCollisionSolid, new Vector2(movePlatXSpeed, 0)))
            {
                float signX = Math.Sign(movePlatXSpeed);

                while (!CollidesWith(_maskCollisionSolid, new Vector2(signX, 0)))
                {
                    Position += new Vector2(signX, 0);
                    Position = new Vector2((float)Math.Round(Position.X), Position.Y); // Prevent sub-pixel sticking issues
                }
            }
            else
            {
                Position += new Vector2(movePlatXSpeed, 0);
            }

            if (_currentGround != null && _currentGround.Collider is MovingPlatform movingPlatformY)
            {
                if (_currentGround.BBoxTop >= Collision.BBoxBottom - movePlatMaxYSpeed)
                {
                    Y += movingPlatformY.YSpeed;
                }
            }
        }

        #endregion
    }
}

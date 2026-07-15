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

            // 1. Inherit movement from the platform we are currently standing on
            HandleMovePlatforms();

            // 2. Handle cases where a moving platform overlapped us during its update loop
            HandleOverlaps();

            // 3. Process inputs, gravity, and apply standard collision checks
            HandleXSpeed(ref moveX);
            HandleYSpeed(ref moveY);

            // 4. Commit final calculated movement
            Position += new Vector2(moveX, moveY);

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

                while (CollidesWithInstance(_maskCollisionSolid, new Vector2(0, signY)) == null)
                {
                    Position += new Vector2(0, signY);
                }

                Position = new Vector2(Position.X, (float)Math.Ceiling(Position.Y));

                if (signY > 0)
                {
                    if (collision.BBoxTop >= Collision.BBoxBottom - Math.Abs(moveY) - 2f)
                    {
                        SetOnGround(true, collision);
                    }
                }
                else if (signY < 0)
                {
                    _jumpHoldTimer = 0;
                }

                moveY = 0;
                _ySpeed = 0;
            }
            else
            {
                var groundCheck = CollidesWithInstance(_maskCollisionSolid, new Vector2(0, 1f));

                if (_ySpeed >= 0 && groundCheck != null && groundCheck.BBoxTop >= Collision.BBoxBottom - 2f)
                {
                    SetOnGround(true, groundCheck);
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
            if (_currentGround != null && _currentGround.Collider is MovingPlatform movingPlatform)
            {
                float platXSpeed = movingPlatform.XSpeed;
                float platYSpeed = movingPlatform.YSpeed;

                if (platXSpeed != 0f)
                {
                    if (CollidesWith(_maskCollisionSolid, new Vector2(platXSpeed, 0)))
                    {
                        float signX = Math.Sign(platXSpeed);
                        while (!CollidesWith(_maskCollisionSolid, new Vector2(signX, 0)))
                        {
                            Position += new Vector2(signX, 0);
                            Position = new Vector2((float)Math.Round(Position.X), Position.Y);
                        }
                    }
                    else
                    {
                        Position += new Vector2(platXSpeed, 0);
                    }
                }

                if (platYSpeed != 0f)
                {
                    Position += new Vector2(0, platYSpeed);
                }
            }
        }

        public void HandleOverlaps()
        {
            var overlap = CollidesWithInstance(_maskCollisionSolid, Vector2.Zero);
            if (overlap != null && overlap.Collider is MovingPlatform movingPlatform)
            {
                if (movingPlatform.XSpeed > 0)
                {
                    float offset = overlap.BBoxRight - Collision.BBoxLeft;
                    Position += new Vector2(offset, 0);
                    _xSpeed = 0f;
                }
                else if (movingPlatform.XSpeed < 0)
                {
                    float offset = overlap.BBoxLeft - Collision.BBoxRight;
                    Position += new Vector2(offset, 0);
                    _xSpeed = 0f;
                }

                if (movingPlatform.YSpeed < 0 && _ySpeed >= movingPlatform.YSpeed)
                {
                    if (Collision.BBoxBottom >= overlap.BBoxTop && Collision.BBoxBottom <= overlap.BBoxBottom)
                    {
                        float offset = overlap.BBoxTop - Collision.BBoxBottom;
                        Position += new Vector2(0, offset);
                        SetOnGround(true, overlap);
                        _ySpeed = 0f;
                    }
                }
                else if (movingPlatform.YSpeed > 0 && Collision.BBoxTop <= overlap.BBoxBottom && Collision.BBoxTop >= overlap.BBoxTop)
                {
                    if (Collision.BBoxTop <= overlap.BBoxBottom && Collision.BBoxTop >= overlap.BBoxTop)
                    {
                        float offset = overlap.BBoxBottom - Collision.BBoxTop;
                        Position += new Vector2(0, offset);

                        _ySpeed = movingPlatform.YSpeed;
                    }
                }
            }
        }

        #endregion
    }
}
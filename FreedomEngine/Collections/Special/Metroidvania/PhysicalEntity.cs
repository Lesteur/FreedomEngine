using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public abstract class PhysicalEntity : Entity
    {
        #region Fields

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

        #endregion

        #region Constructors

        public PhysicalEntity(Sprite sprite, Vector2 position, CollisionMask collision = null) : base(sprite, position, collision)
        {
        }

        #endregion

        #region Public Methods

        public override void Update(GameTime gameTime)
        {
            // Implement physics update logic here
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
            if (CollidesWith(1, new Vector2(moveX, 0)))
            {
                float signX = Math.Sign(moveX);

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
            if (CollidesWith(1, new Vector2(0, moveY)))
            {
                float signY = Math.Sign(moveY);

                while (!CollidesWith(1, new Vector2(0, signY)))
                {
                    Position += new Vector2(0, signY);
                }

                Position = new Vector2(Position.X, (float)Math.Ceiling(Position.Y));

                if (signY > 0)
                {
                    SetOnGround(true);
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

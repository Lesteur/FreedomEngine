using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Components;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;
using System.ComponentModel;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public class MovingPlatform : Entity
    {
        #region Fields

        private TimeSpan _elapsedTime = TimeSpan.Zero;

        private TimeSpan _interval = TimeSpan.FromSeconds(1.0);

        private float _xSpeed = 0f;

        private float _ySpeed = 0f;

        private int _face = 0;

        #endregion

        #region Properties

        public float XSpeed { get; private set; } = 0f;

        public float YSpeed { get; private set; } = 0f;

        #endregion

        #region Constructors

        public MovingPlatform(Sprite sprite, Vector2 position, CollisionMask collision = null) : base(sprite, position, collision)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            if (_elapsedTime >= _interval)
            {
                _elapsedTime = TimeSpan.Zero;
                _face = (_face + 1) % 4;
                switch (_face)
                {
                    case 0:
                        _xSpeed = 0f;
                        _ySpeed = 1f;
                        break;
                    case 1:
                        _xSpeed = 1f;
                        _ySpeed = 0f;
                        break;
                    case 2:
                        _xSpeed = 0f;
                        _ySpeed = -1f;
                        break;
                    case 3:
                        _xSpeed = -1f;
                        _ySpeed = 0f;
                        break;
                }
            }

            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds * EngineConfig.TargetFPS;
            var moveX = _xSpeed * dt;
            var moveY = _ySpeed * dt;

            Position += new Vector2(moveX, moveY);

            XSpeed = moveX;
            YSpeed = moveY;

            _elapsedTime += gameTime.ElapsedGameTime;

            base.Update(gameTime);
        }

        #endregion
        }
}

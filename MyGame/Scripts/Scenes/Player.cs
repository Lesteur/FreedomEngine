using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections;
using FreedomEngine.Components;
using FreedomEngine.Graphics;
using FreedomEngine.Core;

namespace MyGame.Scripts.Scenes
{
    public class Player : Entity
    {
        private float _xSpeed = 0f;
        private float _ySpeed = 0f;

        // Gravity
        private float _grav = .3f;
        private float _maxFallSpeed = 4f;
        private bool _onGround = false;

        // Hang time
        private int _coyoteHangFrames = 2;
        private int _coyoteHangTimer = 0;

        public Player(Sprite sprite, int x, int y) : base(sprite, x, y)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Handle the X movement
            _xSpeed = HandlePlayerMovement();
            // Gravity
            _ySpeed = HandleGravity();

            _ySpeed = HandlePlayerJump();

            // Handle the X Speed
            HandleXSpeed();

            // Handle the Y Speed
            HandleYSpeed();

            Position += new Vector2(_xSpeed, _ySpeed);

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                Logger.Info($"Position: {Position}, OnGround: {_onGround}, CoyoteHangTimer: {_coyoteHangTimer}");
                Logger.Info($"Width: {Width}, Height: {Height}");
            }

            base.Update(gameTime);
        }

        private float HandlePlayerJump()
        {
            float ySpeed = _ySpeed;
            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
            {
                ySpeed = -7f;
                SetOnGround(false);
            }
            return ySpeed;
        }

        private float HandlePlayerMovement()
        {
            float xSpeed = _xSpeed;

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
            else
            {
                xSpeed = 0;
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

        private void HandleXSpeed()
        {
            // On vérifie la collision sur l'ensemble du déplacement horizontal prévu
            if (CollidesWith(1, new Vector2(_xSpeed, 0)))
            {
                float signX = Math.Sign(_xSpeed);

                // On avance pixel par pixel (ou unité par unité) jusqu'à frôler le mur
                while (!CollidesWith(1, new Vector2(signX, 0)))
                {
                    Position += new Vector2(signX, 0);
                }

                _xSpeed = 0;
            }
        }

        private void HandleYSpeed()
        {
            if (_ySpeed > _maxFallSpeed)
            {
                _ySpeed = _maxFallSpeed;
            }

            // On vérifie la collision sur l'ensemble du déplacement vertical prévu
            if (CollidesWith(1, new Vector2(0, _ySpeed)))
            {
                float signY = Math.Sign(_ySpeed);

                // On se rapproche du sol ou du plafond précisément
                while (!CollidesWith(1, new Vector2(0, signY)))
                {
                    Position += new Vector2(0, signY);
                }

                Position = new Vector2(Position.X, (float)Math.Round(Position.Y));

                // Si signY > 0, on tombait, donc on a touché le sol !
                if (signY > 0)
                {
                    SetOnGround(true);
                }

                _ySpeed = 0;
            }
            else
            {
                // Si on ne entre pas en collision, on vérifie si on est toujours sur le sol.
                // On ne le fait que si on ne saute pas (vitesse positive ou nulle).
                if (_ySpeed >= 0 && CollidesWith(1, new Vector2(0, 1f)))
                {
                    SetOnGround(true);
                }
                else
                {
                    // On passe _onGround à false sans appeler SetOnGround(false)
                    // pour permettre au Coyote Time de s'égrener naturellement.
                    _onGround = false;
                }
            }
        }
    }
}

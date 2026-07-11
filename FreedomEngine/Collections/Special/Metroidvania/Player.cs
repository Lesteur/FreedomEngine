using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public class Player : PhysicalEntity
    {
        #region Internal Properties

        internal float XSpeed { get => _xSpeed; set => _xSpeed = value; }

        internal float YSpeed { get => _ySpeed; set => _ySpeed = value; }

        internal bool OnGround => _onGround;

        internal bool JumpKeyPressed => _jumpKeyPressed;

        #endregion

        #region Internal Fields

        internal StateMachinePlayer _machine;

        internal int _jumpBufferTime = 30;

        internal bool _jumpKeyBuffered = false;

        internal int _jumpKeyBufferedTimer = 0;

        internal int _jumpMax = 2;

        internal int _jumpCount = 0;

        internal bool _jumpKey = false;

        internal bool _jumpKeyPressed = false;

        internal readonly int[] _jumpHoldFrames = [18, 10];

        internal readonly float[] _jumpSpeed = [-3.4f, -3.1f];

        #endregion

        #region Constructors

        public Player(Sprite sprite, Vector2 position, CollisionMask collision = null) : base(sprite, position, collision)
        {
            _machine = new StateMachinePlayer(this);
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            GetControllerInput();

            _machine.Update(gameTime);

            base.Update(gameTime);

            if (Application.Input.Keyboard.IsKeyDown(Keys.Enter))
            {
                Logger.Info($"Position: {Position}");
            }
        }

        #endregion

        #region Internal Methods

        internal void GetControllerInput()
        {
            _jumpKey = Application.Input.Keyboard.IsKeyDown(Keys.Space);
            _jumpKeyPressed = Application.Input.Keyboard.WasKeyJustPressed(Keys.Space);

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

        internal float HandlePlayerJump()
        {
            float ySpeed = _ySpeed;

            if (_onGround)
            {
                _jumpCount = 0;
                _jumpHoldTimer = 0;
                _coyoteJumpTimer = _coyoteJumpFrames;
            }
            else
            {
                _coyoteJumpTimer--;

                if (_jumpCount == 0 && _coyoteJumpTimer <= 0)
                {
                    _jumpCount = 1;
                }
            }

            if (_jumpKeyBuffered && _jumpCount < _jumpMax)
            {
                _jumpKeyBuffered = false;
                _jumpKeyBufferedTimer = 0;

                _jumpCount++;

                _jumpHoldTimer = _jumpHoldFrames[_jumpCount - 1];

                SetOnGround(false);
            }

            if (!_jumpKey)
            {
                _jumpHoldTimer = 0;
            }

            if (_jumpHoldTimer > 0)
            {
                ySpeed = _jumpSpeed[_jumpCount - 1];
                _jumpHoldTimer--;
            }

            return ySpeed;
        }

        internal float HandlePlayerMovement()
        {
            float xSpeed = 0f;
            int moveDirection = 0;

            if (Application.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                moveDirection -= 1;
            }

            if (Application.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                moveDirection += 1;
            }

            if (moveDirection != 0)
            {
                xSpeed = moveDirection * 2f;
            }

            return xSpeed;
        }

        #endregion
    }
}

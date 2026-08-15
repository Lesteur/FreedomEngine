using System;
using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Utilities;
using FreedomEngine.Components.Collisions;

namespace MyGame.Scripts.Metroid.States
{
    public enum AnimState
    {
        Stand,
        Run,
        Brake,
        Jump,
        SpinJump,
        Fall
    }

    internal class StateSamusNormal : StateSamus
    {
        #region Constants

        // Define the tag used for your ledge blocks. 
        // You might want to pull this from a global Tags enum/class.
        private const uint LEDGE_TAG = 1;

        // The vertical leniency (in pixels) for grabbing a ledge.
        // If Samus is within this many pixels of the ledge's top edge, she will grab it.
        private const float GRAB_TOLERANCE = 8f;

        #endregion

        #region Fields

        private AnimState _animState;
        private bool _isSpinJumping = false;

        #endregion

        #region Constructors

        public StateSamusNormal(PlayerSamus player, StateMachineSamus stateMachine) : base(player, stateMachine)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            HandleXSpeed();
            HandleYSpeed();

            // Check for ledge grab opportunities before committing to final movement
            //CheckForLedgeGrab();

            base.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public override void OnEnter()
        {
            _animState = AnimState.Stand;
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        #endregion

        #region Private Methods

        private void HandleXSpeed()
        {
            float maxSpeed = _player.InputRun ? _player.Physics.MaxDashSpeed : _player.Physics.MaxRunSpeed;

            if (_player.IsGrounded)
            {
                if (_player.InputMoveX != 0)
                {
                    // Check for sudden direction change (Braking)
                    if ((_player.InputMoveX > 0 && _player.XSpeed < 0) || (_player.InputMoveX < 0 && _player.XSpeed > 0))
                    {
                        _animState = AnimState.Brake;

                        // Fast deceleration during brake
                        _player.XSpeed = MathUtil.Approach(_player.XSpeed, 0, _player.Physics.BrakeFriction);

                        // Once stopped, officially change direction
                        if (Math.Abs(_player.XSpeed) < 0.1f)
                        {
                            _player.Direction = _player.InputMoveX;
                        }
                    }
                    else
                    {
                        // Normal acceleration
                        _player.Direction = _player.InputMoveX;
                        _animState = AnimState.Run;
                        _player.XSpeed = MathUtil.Approach(_player.XSpeed, _player.InputMoveX * maxSpeed, _player.Physics.GroundAccel);
                    }
                }
                else
                {
                    // No input: decelerate to a stop
                    _player.XSpeed = MathUtil.Approach(_player.XSpeed, 0, _player.Physics.GroundDecel);

                    if (Math.Abs(_player.XSpeed) < 0.05f)
                    {
                        _player.XSpeed = 0;
                        _animState = AnimState.Stand;
                    }
                }
            }
            else
            {
                // Airborne horizontal physics
                if (_player.InputMoveX != 0)
                {
                    _player.XSpeed = MathUtil.Approach(_player.XSpeed, _player.InputMoveX * maxSpeed, _player.Physics.AirAccel);
                }
                else
                {
                    _player.XSpeed = MathUtil.Approach(_player.XSpeed, 0, _player.Physics.AirFriction);
                }
            }
        }

        private void HandleYSpeed()
        {
            if (_player.IsGrounded)
            {
                // 1. Initialize Jump
                if (_player.InputJumpPressed)
                {
                    _player.YSpeed = -_player.Physics.JumpImpulse;

                    // Decide between normal jump and spin jump based on horizontal movement
                    if (Math.Abs(_player.XSpeed) > 0.5f && _animState != AnimState.Stand)
                    {
                        _animState = AnimState.SpinJump;
                        _isSpinJumping = true;
                    }
                    else
                    {
                        _animState = AnimState.Jump;
                        _isSpinJumping = false;
                    }
                }
            }
            else
            {
                // 2. Variable Jump Height (Cutoff upward momentum if button released early)
                if (_player.InputJumpReleased && _player.YSpeed < 0)
                {
                    _player.YSpeed *= _player.Physics.JumpCutoffFactor;
                }

                // 3. Apply Gravity
                _player.YSpeed += _player.Physics.Gravity;

                // 4. Terminal Fall Speed Cap
                if (_player.YSpeed > _player.Physics.MaxFallSpeed)
                {
                    _player.YSpeed = _player.Physics.MaxFallSpeed;
                }

                // Transition to fall animation
                if (_player.YSpeed > 0 && !_isSpinJumping)
                {
                    _animState = AnimState.Fall;
                }
            }
        }

        private void CheckForLedgeGrab()
        {
            // 1. Only attempt to grab if we are airborne and falling (or at the exact apex of the jump)
            if (_player.IsGrounded || _player.YSpeed < 0)
                return;

            // Optional: Require the player to be pressing towards the ledge to grab it
            // if (_player.InputMoveX != _player.Direction) return;

            // 2. Cast the collision mask slightly forward to detect the ledge block
            // We use the player's facing direction to determine which way to check.
            Vector2 forwardOffset = new Vector2(_player.Direction * 3f, 0f);
            CollisionMask ledgeMask = _player.CollidesWithInstance(LEDGE_TAG, forwardOffset, false);

            // 3. If a ledge is found, verify vertical alignment
            if (ledgeMask != null && _player.Collision != null)
            {
                // Determine Samus' hand position (usually around the top of her BBox)
                // You may need to add a slight offset depending on your sprite proportions.
                float grabPointY = _player.Collision.BBoxTop;
                float targetLedgeY = ledgeMask.BBoxTop;

                // Check if Samus' hands are within the acceptable vertical range of the ledge corner
                if (Math.Abs(grabPointY - targetLedgeY) <= GRAB_TOLERANCE)
                {
                    // 4. Calculate the required correction to snap perfectly to the ledge
                    // This snaps the player so their BBoxTop exactly aligns with the Ledge's BBoxTop
                    float correctionY = targetLedgeY - grabPointY;
                    _player.Position = new Vector2(_player.Position.X, _player.Position.Y + correctionY);

                    // 5. Change state to Grip
                    // Note: Ensure you have added `GripState` to your StateMachineSamus
                    StateMachine.ChangeState(StateMachine.GripState);
                }
            }
        }

        #endregion
    }
}
using System;
using Microsoft.Xna.Framework;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    internal class IdleState : StatePlayer
    {
        #region Constructors

        public IdleState(Player player, StateMachinePlayer stateMachine) : base(player, stateMachine)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _player.YSpeed = _player.HandleGravity();
            _player.YSpeed = _player.HandlePlayerJump();

            var xSpeed = _player.HandlePlayerMovement();

            // Transition to Walk if moving horizontally
            if (Math.Abs(xSpeed) > 0)
            {
                StateMachine.ChangeState(StateMachine.WalkState);
                return;
            }

            // Transition to Fall if we are no longer on the ground and moving down
            if (!_player.OnGround && _player.YSpeed > 0)
            {
                StateMachine.ChangeState(StateMachine.FallState);
                return;
            }

            // Transition to Jump if we are jumping (negative YSpeed or jump key pressed)
            if (!_player.OnGround && _player.YSpeed < 0)
            {
                StateMachine.ChangeState(StateMachine.JumpState);
                return;
            }

            _player.XSpeed = 0f;
        }

        #endregion

        #region Public Methods

        public override void OnEnter()
        {
            base.OnEnter();
            _player.XSpeed = 0f;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        #endregion
    }
}

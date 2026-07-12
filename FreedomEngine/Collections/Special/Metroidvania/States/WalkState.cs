using System;
using Microsoft.Xna.Framework;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    internal class WalkState : StatePlayer
    {
        #region Constructors

        public WalkState(Player player, StateMachinePlayer stateMachine) : base(player, stateMachine)
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

            _player.XSpeed = xSpeed;

            if (Math.Abs(xSpeed) == 0)
            {
                StateMachine.ChangeState(StateMachine.IdleState);
                return;
            }

            if (!_player.OnGround && _player.YSpeed > 0)
            {
                StateMachine.ChangeState(StateMachine.FallState);
                return;
            }

            if (!_player.OnGround && _player.YSpeed < 0)
            {
                StateMachine.ChangeState(StateMachine.JumpState);
                return;
            }
        }

        #endregion

        #region Public Methods

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        #endregion
    }
}

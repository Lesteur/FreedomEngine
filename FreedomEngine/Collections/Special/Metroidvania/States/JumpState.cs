using System;
using Microsoft.Xna.Framework;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    internal class JumpState : StatePlayer
    {
        #region Constructors

        public JumpState(Player player, StateMachinePlayer stateMachine) : base(player, stateMachine)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _player.XSpeed = _player.HandlePlayerMovement();

            _player.YSpeed = _player.HandleGravity();
            _player.YSpeed = _player.HandlePlayerJump();

            if (_player.YSpeed >= 0)
            {
                StateMachine.ChangeState(StateMachine.FallState);
                return;
            }

            // In some cases, player might land while ascending if they hit their head
            if (_player.OnGround)
            {
                if (Math.Abs(_player.XSpeed) > 0)
                {
                    StateMachine.ChangeState(StateMachine.WalkState);
                }
                else
                {
                    StateMachine.ChangeState(StateMachine.IdleState);
                }
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

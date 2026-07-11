using System;
using Microsoft.Xna.Framework;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    internal class FallState : StatePlayer
    {
        #region Constructors

        public FallState(Player player, StateMachine<StatePlayer> stateMachine) : base(player, stateMachine)
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

            var stateMachinePlayer = (StateMachinePlayer)StateMachine;

            // If we double jumped, transition back to jump state
            if (_player.YSpeed < 0)
            {
                StateMachine.ChangeState(stateMachinePlayer._jumpState);
                return;
            }

            // Transition to ground states when landing
            if (_player.OnGround)
            {
                if (Math.Abs(_player.XSpeed) > 0)
                {
                    StateMachine.ChangeState(stateMachinePlayer._walkState);
                }
                else
                {
                    StateMachine.ChangeState(stateMachinePlayer._idleState);
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

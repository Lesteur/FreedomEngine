using System;
using Microsoft.Xna.Framework;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    internal class WalkState : StatePlayer
    {
        #region Constructors

        public WalkState(Player player, StateMachine<StatePlayer> stateMachine) : base(player, stateMachine)
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
            var stateMachinePlayer = (StateMachinePlayer)_stateMachine;

            _player.XSpeed = xSpeed;

            // Transition to Idle if we stopped moving
            if (Math.Abs(xSpeed) == 0)
            {
                _stateMachine.ChangeState(stateMachinePlayer._idleState);
                return;
            }

            // Transition to Fall if we are no longer on the ground and moving down
            if (!_player.OnGround && _player.YSpeed > 0)
            {
                _stateMachine.ChangeState(stateMachinePlayer._fallState);
                return;
            }

            // Transition to Jump if we are jumping (negative YSpeed)
            if (!_player.OnGround && _player.YSpeed < 0)
            {
                _stateMachine.ChangeState(stateMachinePlayer._jumpState);
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

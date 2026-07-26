using FreedomEngine.Collections.Special.Metroidvania.States;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public class StateMachinePlayer : StateMachine<StatePlayer, StateMachinePlayer>
    {
        #region Fields

        private readonly Player _player;

        #endregion

        #region Internal Fields

        internal IdleState IdleState;

        internal WalkState WalkState;

        internal JumpState JumpState;

        internal FallState FallState;

        #endregion

        #region Constructors

        public StateMachinePlayer(Player player) : base()
        {
            _player = player;

            IdleState = new IdleState(_player, this);
            WalkState = new WalkState(_player, this);
            JumpState = new JumpState(_player, this);
            FallState = new FallState(_player, this);

            ChangeState(IdleState);
        }

        #endregion
    }
}

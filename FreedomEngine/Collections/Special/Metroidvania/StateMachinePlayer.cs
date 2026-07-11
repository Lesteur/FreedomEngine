using FreedomEngine.Collections.Special.Metroidvania.States;
using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public class StateMachinePlayer : StateMachine<StatePlayer>
    {
        #region Fields

        private readonly Player _player;

        #endregion

        #region Internal Fields

        internal IdleState _idleState;

        internal WalkState _walkState;

        internal JumpState _jumpState;

        internal FallState _fallState;

        #endregion

        #region Constructors

        public StateMachinePlayer(Player player) : base()
        {
            _player = player;

            _idleState = new IdleState(_player, this);
            _walkState = new WalkState(_player, this);
            _jumpState = new JumpState(_player, this);
            _fallState = new FallState(_player, this);

            ChangeState(_idleState);
        }

        #endregion
    }
}

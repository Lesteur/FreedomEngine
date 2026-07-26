using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania.States
{
    public class StatePlayer : State<StatePlayer, StateMachinePlayer>
    {
        #region Internal Fields

        internal Player _player;

        #endregion

        #region Constructors

        public StatePlayer(Player player, StateMachinePlayer stateMachine) : base(stateMachine)
        {
            _player = player;
        }

        #endregion
    }
}

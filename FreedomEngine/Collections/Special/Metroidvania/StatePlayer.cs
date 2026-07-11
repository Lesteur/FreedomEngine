using FreedomEngine.Collections.States;

namespace FreedomEngine.Collections.Special.Metroidvania
{
    public class StatePlayer : State<StatePlayer>
    {
        #region Internal Fields

        internal Player _player;

        #endregion

        #region Constructors

        public StatePlayer(Player player, StateMachine<StatePlayer> stateMachine) : base(stateMachine)
        {
            _player = player;
        }

        #endregion
    }
}

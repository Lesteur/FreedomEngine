using FreedomEngine.Collections.States;
using System.Resources;

namespace FreedomEngine.Collections.Special.Metroidvania
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

using FreedomEngine.Collections.Special.Metroidvania;
using FreedomEngine.Collections.States;

namespace MyGame.Scripts.Metroid.States
{
    public class StateSamus : State<StateSamus, StateMachineSamus>
    {
        #region Internal Fields

        internal PlayerSamus _player;

        #endregion

        #region Constructors

        public StateSamus(PlayerSamus player, StateMachineSamus stateMachine) : base(stateMachine)
        {
            _player = player;
        }

        #endregion
    }
}

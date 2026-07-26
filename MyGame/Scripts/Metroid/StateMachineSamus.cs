using Microsoft.Xna.Framework;

using FreedomEngine.Collections.States;

using MyGame.Scripts.Metroid.States;

namespace MyGame.Scripts.Metroid
{
    public class StateMachineSamus : StateMachine<StateSamus, StateMachineSamus>
    {
        #region Fields

        private readonly PlayerSamus _player;

        #endregion

        #region Internal Fields

        internal StateSamusNormal NormalState;

        internal StateSamusGrip GripState;

        #endregion

        #region Constructors

        public StateMachineSamus(PlayerSamus player) : base()
        {
            _player = player;
            
            NormalState = new StateSamusNormal(_player, this);
            GripState = new StateSamusGrip(_player, this);

            ChangeState(NormalState);
        }

        #endregion
    }
}

using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.RPG
{
    public class BattleSystem
    {
        #region Fields

        #endregion

        #region Properties

        public int CurrentTurn { get; set; }

        public List<Battler> Battlers { get; set; }

        public List<Battler> Allies { get; set; }

        public List<Battler> Enemies { get; set; }

        #endregion

        #region Constructors

        public BattleSystem()
        {
            CurrentTurn = 0;
            Battlers = [];
            Allies = [];
            Enemies = [];
        }

        #endregion
    }
}
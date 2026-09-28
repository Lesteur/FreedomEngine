using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    public class BattleSystem
    {
        #region Fields

        #endregion

        #region Properties

        public int CurrentTurn { get; }

        public Battler[] Battlers { get; }

        public BattlerAlly[] Allies { get; }

        public BattlerEnemy[] Enemies { get; }

        public BattleParameters Parameters { get; }

        #endregion

        #region Constructors

        public BattleSystem(BattlerAlly[] allies, BattlerEnemy[] enemies, BattleParameters parameters)
        {
            CurrentTurn = 0;
            Battlers = new Battler[allies.Length + enemies.Length];
            Allies = allies;
            Enemies = enemies;
            Parameters = parameters;

            for (int i = 0; i < allies.Length; i++)
                Battlers[i] = allies[i];

            for (int i = 0; i < enemies.Length; i++)
                Battlers[allies.Length + i] = enemies[i];
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
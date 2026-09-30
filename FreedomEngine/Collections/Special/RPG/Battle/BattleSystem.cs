using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    public class BattleSystem
    {
        #region Fields

        private int _currentBattlerIndex;

        private List<Battler> _turnOrder;

        #endregion

        #region Properties

        public int CurrentTurn { get; private set; }

        public Battler[] Battlers { get; private set; }

        public BattlerAlly[] Allies { get; private set; }

        public BattlerEnemy[] Enemies { get; private set; }

        public BattleParameters Parameters { get; private set; }

        #endregion

        #region Constructors

        public BattleSystem(BattlerAlly[] allies, BattlerEnemy[] enemies, BattleParameters parameters)
        {
            Battler.CurrentBattleSystem = this;

            _currentBattlerIndex = 0;

            CurrentTurn = 0;
            Allies = allies;
            Enemies = enemies;
            Battlers = new Battler[allies.Length + enemies.Length];
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

        private void BeginTurn()
        {
            _currentBattlerIndex = 0;

            _turnOrder = [..Battlers];
            _turnOrder.Sort((a, b) => b.Data.BaseStats.Speed.CompareTo(a.Data.BaseStats.Speed));


        }

        private void ProcessTurn()
        {
            if (_currentBattlerIndex >= _turnOrder.Count)
            {
                EndTurn();
                return;
            }

            Battler currentBattler = _turnOrder[_currentBattlerIndex];
            while (!currentBattler.StartTurn())
            {
                _currentBattlerIndex++;
                if (_currentBattlerIndex >= _turnOrder.Count)
                {
                    EndTurn();
                    return;
                }

                currentBattler = _turnOrder[_currentBattlerIndex];
            }

            _currentBattlerIndex++;
        }

        private void EndTurn()
        {
            CurrentTurn++;
        }

        #endregion
    }
}
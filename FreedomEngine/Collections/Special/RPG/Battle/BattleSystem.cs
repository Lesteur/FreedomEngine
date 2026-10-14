using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    /// <summary>
    /// Runs a turn-based battle between a group of allies and a group of enemies.
    /// </summary>
    /// <remarks>
    /// Battlers act one at a time. The system starts a battler's turn, then waits until that battler
    /// calls back through <see cref="FinishAction"/> before moving on to the next one. Turn progression
    /// is a loop rather than a chain of recursive calls, so a battler that finishes its action
    /// synchronously cannot overflow the stack.
    /// </remarks>
    public class BattleSystem
    {
        #region Fields

        /// <summary>
        /// The battlers that act during the current turn, in acting order.
        /// </summary>
        private readonly List<Battler> _turnOrder = [];

        /// <summary>
        /// The index in <see cref="_turnOrder"/> of the next battler to act.
        /// </summary>
        private int _currentBattlerIndex;

        /// <summary>
        /// The battler whose action is in progress, or <see langword="null"/> if none is.
        /// </summary>
        private Battler _currentBattler;

        /// <summary>
        /// Whether at least one battler has acted during the current turn.
        /// </summary>
        private bool _hasBattlerActed;

        /// <summary>
        /// Whether <see cref="Advance"/> is currently running, used to absorb re-entrant calls.
        /// </summary>
        private bool _isAdvancing;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of turns that have been completed.
        /// </summary>
        public int CurrentTurn { get; private set; }

        /// <summary>
        /// Gets every battler taking part in the battle (allies first, then enemies).
        /// </summary>
        public Battler[] Battlers { get; }

        /// <summary>
        /// Gets the battlers fighting on the player's side.
        /// </summary>
        public BattlerAlly[] Allies { get; }

        /// <summary>
        /// Gets the battlers fighting on the opposing side.
        /// </summary>
        public BattlerEnemy[] Enemies { get; }

        /// <summary>
        /// Gets the parameters of this battle.
        /// </summary>
        public BattleParameters Parameters { get; }

        /// <summary>
        /// Gets whether the battle has been started and has not ended yet.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets whether one of the two sides has no living battler left.
        /// </summary>
        public bool IsBattleOver => !AnyAlive(Allies) || !AnyAlive(Enemies);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BattleSystem"/> class.
        /// </summary>
        /// <param name="allies">The battlers fighting on the player's side.</param>
        /// <param name="enemies">The battlers fighting on the opposing side.</param>
        /// <param name="parameters">The parameters of this battle.</param>
        public BattleSystem(BattlerAlly[] allies, BattlerEnemy[] enemies, BattleParameters parameters)
        {
            Allies = allies;
            Enemies = enemies;
            Parameters = parameters;
            Battlers = new Battler[allies.Length + enemies.Length];

            allies.CopyTo(Battlers, 0);
            enemies.CopyTo(Battlers, allies.Length);

            foreach (Battler battler in Battlers)
                battler.BattleSystem = this;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the battle. Calling this method on a battle that is already running has no effect.
        /// </summary>
        public void StartBattle()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            CurrentTurn = 0;

            Logger.Info($"Battle started with {Allies.Length} allies and {Enemies.Length} enemies.");

            BeginTurn();
            Advance();
        }

        /// <summary>
        /// Reports that a battler has finished its action, and moves on to the next battler.
        /// </summary>
        /// <param name="battler">The battler that finished its action.</param>
        /// <remarks>
        /// Calls from a battler that is not the one currently acting (for example a duplicate call)
        /// are ignored.
        /// </remarks>
        public void FinishAction(Battler battler)
        {
            if (!IsRunning || battler != _currentBattler)
                return;

            _currentBattler = null;

            Advance();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Checks whether at least one of the given battlers is alive.
        /// </summary>
        private static bool AnyAlive(Battler[] battlers)
        {
            foreach (Battler battler in battlers)
            {
                if (battler.IsAlive())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Prepares the acting order for a new turn.
        /// </summary>
        private void BeginTurn()
        {
            _currentBattlerIndex = 0;
            _hasBattlerActed = false;

            _turnOrder.Clear();

            foreach (Battler battler in Battlers)
            {
                if (battler.IsAlive())
                    _turnOrder.Add(battler);
            }

            // TODO: sort by speed once BattlerStats exposes it.
            _turnOrder.Sort((a, b) => b.Data.BaseStats.Speed.CompareTo(a.Data.BaseStats.Speed));
        }

        /// <summary>
        /// Completes the current turn and prepares the next one.
        /// </summary>
        private void EndTurn()
        {
            CurrentTurn++;

            BeginTurn();
        }

        /// <summary>
        /// Starts turns, one battler after another, until a battler's action is in progress or the battle stops.
        /// </summary>
        /// <remarks>
        /// A battler may call <see cref="FinishAction"/> while its turn is being started. That
        /// re-entrant call only clears <see cref="_currentBattler"/>; the loop below then picks
        /// the next battler itself instead of recursing.
        /// </remarks>
        private void Advance()
        {
            Logger.Info($"Advancing battle (turn {CurrentTurn}, current battler index {_currentBattlerIndex}).");

            if (_isAdvancing)
                return;

            _isAdvancing = true;

            try
            {
                while (IsRunning && _currentBattler == null)
                {
                    if (IsBattleOver)
                    {
                        IsRunning = false;
                        break;
                    }

                    if (_currentBattlerIndex >= _turnOrder.Count)
                    {
                        // A full turn went by without any battler able to act: stop instead of looping forever.
                        if (!_hasBattlerActed)
                        {
                            IsRunning = false;
                            break;
                        }

                        EndTurn();
                        continue;
                    }

                    Battler next = _turnOrder[_currentBattlerIndex++];

                    // Skip battlers that died earlier during this turn.
                    if (!next.IsAlive())
                        continue;

                    _currentBattler = next;

                    if (next.StartTurn())
                        _hasBattlerActed = true;
                    else if (_currentBattler == next)
                        _currentBattler = null;
                }
            }
            finally
            {
                _isAdvancing = false;
            }
        }

        #endregion
    }
}
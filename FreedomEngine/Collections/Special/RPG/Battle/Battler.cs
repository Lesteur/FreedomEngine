using System;
using System.Collections;

using FreedomEngine.Collections.Coroutines;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    /// <summary>
    /// Base class for every participant of a <see cref="Battle.BattleSystem"/>.
    /// </summary>
    /// <remarks>
    /// Derived classes only describe <em>what</em> the battler does during its turn by implementing
    /// <see cref="Act"/>. The base class runs that action as a coroutine and notifies the battle
    /// system once it completes, so derived classes never have to signal the end of their own turn.
    /// </remarks>
    public abstract class Battler
    {
        #region Fields

        /// <summary>
        /// The coroutine driving the current turn, or <see langword="null"/> if no turn has started yet.
        /// </summary>
        private Coroutine _turnCoroutine;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the battle system this battler belongs to, or <see langword="null"/> if it has not joined one.
        /// </summary>
        public BattleSystem BattleSystem { get; internal set; }

        /// <summary>
        /// Gets the static data this battler was created from.
        /// </summary>
        public BattlerData Data { get; }

        /// <summary>
        /// Gets the current health points of this battler.
        /// </summary>
        public int CurrentHealthPoints { get; }

        /// <summary>
        /// Gets the current skill points of this battler.
        /// </summary>
        public int CurrentSkillPoints { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Battler"/> class.
        /// </summary>
        /// <param name="data">The data describing this battler.</param>
        protected Battler(BattlerData data)
        {
            ArgumentNullException.ThrowIfNull(nameof(data));

            Data = data;
            CurrentHealthPoints = data.BaseStats.HealthPoints;
            CurrentSkillPoints = data.BaseStats.SkillPoints;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets whether this battler still has health points left.
        /// </summary>
        /// <returns><see langword="true"/> if the battler is alive; otherwise, <see langword="false"/>.</returns>
        public bool IsAlive() => CurrentHealthPoints > 0;

        /// <summary>
        /// Attempts to start this battler's turn.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the turn started and <see cref="Act"/> is now running;
        /// <see langword="false"/> if the battler cannot act and its turn must be skipped.
        /// </returns>
        public virtual bool StartTurn()
        {
            if (!IsAlive())
                return false;

            _turnCoroutine = new Coroutine(TurnRoutine());

            return true;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Describes what this battler does during its turn.
        /// </summary>
        /// <remarks>
        /// Yield instructions (such as <see cref="WaitForSeconds"/>) pause the turn. The turn ends
        /// automatically when the returned enumerator completes.
        /// </remarks>
        /// <returns>An enumerator that represents the action logic.</returns>
        protected abstract IEnumerator Act();

        #endregion

        #region Private Methods

        /// <summary>
        /// Runs <see cref="Act"/>, then reports the end of the turn to the battle system.
        /// </summary>
        /// <returns>An enumerator that represents the whole turn.</returns>
        private IEnumerator TurnRoutine()
        {
            IEnumerator action = Act();

            while (action.MoveNext())
                yield return action.Current;

            BattleSystem?.FinishAction(this);
        }

        #endregion
    }
}
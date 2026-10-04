namespace FreedomEngine.Collections.Special.RPG.Battle
{
    public abstract class Battler
    {
        #region Properties

        public static BattleSystem CurrentBattleSystem { get; set; }

        public BattlerData Data { get; }

        public int CurrentHealthPoints { get; }

        public int CurrentSkillPoints { get; }

        #endregion

        #region Constructors

        public Battler(BattlerData data)
        {
            Data = data;
            CurrentHealthPoints = data?.BaseStats.HealthPoints ?? 0;
            CurrentSkillPoints = data?.BaseStats.SkillPoints ?? 0;
        }

        #endregion

        #region Public Methods

        public static void FinishAction() => CurrentBattleSystem.FinishAction();

        public bool IsAlive() => CurrentHealthPoints > 0;

        public virtual bool StartTurn()
        {
            // Logic to start the battler's turn
            return true; // Return true if the turn was successfully started
        }

        #endregion
    }
}
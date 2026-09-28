namespace FreedomEngine.Collections.Special.RPG.Battle
{
    public abstract class Battler
    {
        #region Properties

        public BattlerData Data { get; }

        public int CurrentHealthPoints { get; }

        public int CurrentSkillPoints { get; }

        #endregion

        #region Constructors

        public Battler(BattlerData data)
        {
            Data = data;
            CurrentHealthPoints = data.BaseStats.HealthPoints;
            CurrentSkillPoints = data.BaseStats.SkillPoints;
        }

        #endregion
    }
}
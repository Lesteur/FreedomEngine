namespace FreedomEngine.Collections.Special.RPG
{
    public abstract class Battler
    {
        public BattlerData Data { get; set; }

        public int CurrentHealthPoints { get; set; }

        public int CurrentSkillPoints { get; set; }
    }
}
using System;

namespace FreedomEngine.Collections.Special.RPG
{
    public abstract class SkillData
    {
        public string ID { get; set; }

        public string IDName { get; set; }

        public string IDDescription { get; set; }

        public Func<BattleSystem> Function { get; set; }

        public ContextTypeEnum ContextType { get; set; }

        public TargetTypeEnum TargetType { get; set; }

        public int SPCost { get; set; }

        public int Power { get; set; }

        public bool RequiresLineOfSight { get; set; }
    }
}
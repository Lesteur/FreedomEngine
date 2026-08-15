using System;
using System.Collections;

namespace FreedomEngine.Collections.Special.RPG
{
    public abstract class SkillData
    {
        public string ID { get; set; }

        public string IDName { get; set; }

        public string IDDescription { get; set; }

        public Func<IEnumerator> Function { get; set; }

        public int SPCost { get; set; }

        public int Power { get; set; }

        public TargetTypeEnum TargetType { get; set; }

        public bool CanTargetSelf { get; set; }

        public bool RequiresLineOfSight { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Special.RPG.Items
{
    public class ItemData
    {
        public string ID { get; set; }

        public string IDName { get; set; }

        public string IDDescription { get; set; }

        public Func<BattleSystem> FunctionBattle { get; set; }

        public ContextTypeEnum ContextType { get; set; }

        public RPGEnum TargetType { get; set; }

        public int Price { get; set; }

        public bool CanBeSold { get; set; }

        public bool RequiresLineOfSight { get; set; }

        public ItemData ItemResult { get; set; }
    }
}

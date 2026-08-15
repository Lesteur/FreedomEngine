using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Special.RPG
{
    public struct Stats
    {
        public int HP { get; set; }

        public int SP { get; set; }

        public int Attack { get; set; }

        public int Magic { get; set; }

        public int Defense { get; set; }

        public int MagicDefense { get; set; }

        public int Precision { get; set; }

        public int Dodge { get; set; }

        public int Speed { get; set; }

        public float CriticalRate { get; set; }

        public float CriticalPower { get; set; }
    }
}

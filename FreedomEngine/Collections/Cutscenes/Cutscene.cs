using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Cutscenes
{
    public class Cutscene
    {
        public string[] StringTable { get; }

        public Instruction[] Instructions { get; }

        public Cutscene(string[] stringTable, Instruction[] instructions)
        {
            StringTable = stringTable;
            Instructions = instructions;
        }
    }
}

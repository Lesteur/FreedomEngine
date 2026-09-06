using System.Collections.Generic;

namespace FreedomEngine.Content.Pipeline.Cutscenes
{
    public class CutsceneContent
    {
        public List<Instruction> Instructions { get; set; } = [];

        public List<string> StringTable { get; set; } = [];
    }
}
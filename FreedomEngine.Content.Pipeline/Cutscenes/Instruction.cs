using System.Collections.Generic;

namespace FreedomEngine.Content.Pipeline.Cutscenes
{
    public enum OpCode : byte
    {
        EndCutscene = 0,
        PrintText = 1,
        Wait = 2,
        CheckVar = 3,    // var1_index, operator(0:>, 1:<, 2:==, 3:!=, 4:<=, 5:>=), var2_index
        SetVar = 4,      // var_index, raw_value (Used to initialize literal registers)
        CopyVar = 5,     // dest_var_index, src_var_index
        Jump = 6,        // jump_target_instruction_index
        JumpIfNot = 7,   // jump_target_instruction_index
        Add = 8          // dest_var_index, src_var_index
    }

    public struct Instruction
    {
        public OpCode OpCode;
        public List<int> Parameters;
    }
}
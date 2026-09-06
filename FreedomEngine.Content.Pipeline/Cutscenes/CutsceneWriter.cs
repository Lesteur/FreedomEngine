using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.IO;
using System.Text;

namespace FreedomEngine.Content.Pipeline.Cutscenes
{
    [ContentTypeWriter]
    public class CutsceneWriter : ContentTypeWriter<CutsceneContent>
    {
        protected override void Write(ContentWriter writer, CutsceneContent cutscene)
        {
            // Generate a readable text file for debugging purposes
            GenerateDebugFile(cutscene);

            // Write String Table
            writer.Write(cutscene.StringTable.Count);
            foreach (var str in cutscene.StringTable)
            {
                writer.Write(str);
            }

            // Write Instructions
            writer.Write(cutscene.Instructions.Count);
            foreach (var instruction in cutscene.Instructions)
            {
                writer.Write((byte)instruction.OpCode);

                int paramCount = instruction.Parameters?.Count ?? 0;
                writer.Write((byte)paramCount);

                if (instruction.Parameters != null)
                {
                    foreach (var param in instruction.Parameters)
                    {
                        writer.Write(param);
                    }
                }
            }
        }

        private void GenerateDebugFile(CutsceneContent cutscene)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== CUTSCENE DEBUG OUPUT ===");

                sb.AppendLine("\n--- String Table ---");
                for (int i = 0; i < cutscene.StringTable.Count; i++)
                {
                    sb.AppendLine($"[{i}] \"{cutscene.StringTable[i]}\"");
                }

                sb.AppendLine("\n--- Instructions ---");
                for (int i = 0; i < cutscene.Instructions.Count; i++)
                {
                    var inst = cutscene.Instructions[i];
                    string opCodeStr = inst.OpCode.ToString().PadRight(15);
                    string paramsStr = inst.Parameters != null ? string.Join(", ", inst.Parameters) : "None";

                    sb.AppendLine($"[{i:D4}] {opCodeStr} | Params: {paramsStr}");
                }

                string debugPath = Path.Combine(Environment.CurrentDirectory, "cutscene_debug_output.txt");
                File.WriteAllText(debugPath, sb.ToString());
            }
            catch
            {
                // Swallow the exception to prevent crashes if writing permissions are missing
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Collections.Cutscenes.Cutscene, FreedomEngine";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Content.CutsceneReader, FreedomEngine";
        }
    }
}
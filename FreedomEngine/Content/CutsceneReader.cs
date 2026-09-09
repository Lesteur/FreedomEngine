using FreedomEngine.Collections.Cutscenes;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Content
{
    public class CutsceneReader : ContentTypeReader<Cutscene>
    {
        protected override Cutscene Read(ContentReader reader, Cutscene existingInstance)
        {
            var countString = reader.ReadInt32();
            var cutsceneStrings = new string[countString];

            for (int i = 0; i < countString; i++)
            {
                cutsceneStrings[i] = reader.ReadString();
            }

            var countInstructions = reader.ReadInt32();
            Instruction[] cutsceneInstructions = new Instruction[countInstructions];

            for (int i = 0; i < countInstructions; i++)
            {
                var opCode = (OpCode)reader.ReadByte();
                var opCount = reader.ReadByte();
                var parameters = new int[opCount];

                for (int j = 0; j < opCount; j++)
                {
                    parameters[j] = reader.ReadInt32();
                }

                cutsceneInstructions[i] = new Instruction()
                {
                    OpCode = opCode,
                    Parameters = parameters
                };
            }

            return new Cutscene(cutsceneStrings, cutsceneInstructions);
        }
    }
}

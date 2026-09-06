using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FreedomEngine.Content.Pipeline.Cutscenes
{
    [ContentImporter(".cutscene", DisplayName = "Cutscene Importer", DefaultProcessor = "CutsceneProcessor")]
    public class CutsceneImporter : ContentImporter<List<string>>
    {
        public override List<string> Import(string filename, ContentImporterContext context)
        {
            var finalLines = new List<string>();
            var lines = File.ReadAllLines(filename).ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Trim();

                if (!string.IsNullOrEmpty(lines[i]) || !lines[i].StartsWith('#'))
                {
                    finalLines.Add(lines[i]);
                }
            }

            return finalLines;
        }
    }
}

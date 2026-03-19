using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.BitmapFonts
{
    /// <summary>
    /// Processes raw BMFont data into a format ready for serialization.
    /// </summary>
    [ContentProcessor(DisplayName = "Bitmap Font Processor")]
    public class BitmapFontProcessor : ContentProcessor<BitmapFontFileContent, BitmapFontContent>
    {
        public override BitmapFontContent Process(BitmapFontFileContent input, ContentProcessorContext context)
        {
            ArgumentNullException.ThrowIfNull(input);

            // Validate kerning data to avoid runtime issues
            ValidateKernings(input, context);

            var output = new BitmapFontContent
            {
                LineHeight = input.Common.LineHeight,
                BaseLine = input.Common.Base
            };

            // Register texture dependencies
            foreach (var page in input.Pages)
            {
                string fileName = Path.GetFileName(page);

                context.AddDependency(fileName);

                output.TextureNames.Add(Path.GetFileNameWithoutExtension(fileName));
            }

            // Copy characters
            foreach (var c in input.Characters)
            {
                output.Characters.Add(new BitmapFontContent.Character
                {
                    Id = c.Id,
                    BoundsX = c.X,
                    BoundsY = c.Y,
                    Width = c.Width,
                    Height = c.Height,
                    XOffset = c.XOffset,
                    YOffset = c.YOffset,
                    XAdvance = c.XAdvance,
                    Page = c.Page
                });
            }

            // Copy kernings
            foreach (var k in input.Kernings)
            {
                output.Kernings.Add(new BitmapFontContent.Kerning
                {
                    First = k.First,
                    Second = k.Second,
                    Amount = k.Amount
                });
            }

            return output;
        }

        /// <summary>
        /// Detects duplicate kerning pairs and logs warnings.
        /// </summary>
        private void ValidateKernings(BitmapFontFileContent font, ContentProcessorContext context)
        {
            if (font.Kernings.Count == 0)
                return;

            var seen = new Dictionary<(int, int), int>();

            for (int i = 0; i < font.Kernings.Count; i++)
            {
                var k = font.Kernings[i];
                var key = (k.First, k.Second);

                if (seen.TryGetValue(key, out int existingIndex))
                {
                    context.Logger.LogWarning(
                        string.Empty,
                        new ContentIdentity(font.FilePath),
                        $"Duplicate kerning pair ({k.First}, {k.Second}) at index {i} (already defined at index {existingIndex})."
                    );
                }
                else
                {
                    seen[key] = i;
                }
            }
        }
    }

    /// <summary>
    /// Final intermediate content used by the Content Pipeline Writer.
    /// </summary>
    public class BitmapFontContent
    {
        public int LineHeight;
        public int BaseLine;

        public List<string> TextureNames { get; } = [];
        public List<Character> Characters { get; } = [];
        public List<Kerning> Kernings { get; } = [];

        public struct Character
        {
            public int Id;

            public int BoundsX;
            public int BoundsY;
            public int Width;
            public int Height;

            public int XOffset;
            public int YOffset;
            public int XAdvance;

            public int Page;
        }

        public struct Kerning
        {
            public int First;
            public int Second;
            public int Amount;
        }
    }
}
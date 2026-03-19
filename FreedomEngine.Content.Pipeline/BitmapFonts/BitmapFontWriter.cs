using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace FreedomEngine.Content.Pipeline.BitmapFonts
{
    /// <summary>
    /// Writes BitmapFontContent into a compact binary format.
    /// </summary>
    [ContentTypeWriter]
    public class BitmapFontWriter : ContentTypeWriter<BitmapFontContent>
    {
        protected override void Write(ContentWriter writer, BitmapFontContent font)
        {
            // --- Texture pages ---
            writer.Write(font.TextureNames.Count);
            foreach (var texture in font.TextureNames)
                writer.Write(texture);

            // --- Global metrics ---
            writer.Write(font.LineHeight);
            writer.Write(font.BaseLine);

            // --- Characters ---
            writer.Write(font.Characters.Count);

            foreach (var c in font.Characters)
            {
                writer.Write(c.Id);

                writer.Write((ushort)c.BoundsX);
                writer.Write((ushort)c.BoundsY);
                writer.Write((ushort)c.Width);
                writer.Write((ushort)c.Height);

                writer.Write((short)c.XOffset);
                writer.Write((short)c.YOffset);
                writer.Write((short)c.XAdvance);

                writer.Write((byte)c.Page);
            }

            // --- Kernings ---
            writer.Write(font.Kernings.Count);

            foreach (var k in font.Kernings)
            {
                writer.Write(k.First);
                writer.Write(k.Second);
                writer.Write((short)k.Amount);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Graphics.BitmapFont, FreedomEngine";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Content.BitmapFontReader, FreedomEngine";
        }
    }
}
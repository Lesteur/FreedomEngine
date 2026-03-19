using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;
using FreedomEngine.Core;

namespace FreedomEngine.Content
{
    /// <summary>
    /// Reads a BitmapFont from the compiled XNB file.
    /// </summary>
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            // --- Load textures ---
            int textureCount = reader.ReadInt32();
            var textures = new Texture2D[textureCount];

            for (int i = 0; i < textureCount; i++)
            {
                string textureName = reader.ReadString();
                textures[i] = reader.ContentManager.Load<Texture2D>($"Assets/Fonts/{textureName}");
            }

            // --- Global metrics ---
            int lineHeight = reader.ReadInt32();
            int baseLine = reader.ReadInt32(); // Not used directly but kept for future use

            // --- Characters ---
            int characterCount = reader.ReadInt32();

            var characters = new List<BitmapFontCharacter>(characterCount);

            // Temporary lookup for fast kerning assignment
            var characterLookup = new Dictionary<int, BitmapFontCharacter>(characterCount);

            for (int i = 0; i < characterCount; i++)
            {
                int id = reader.ReadInt32();

                ushort x = reader.ReadUInt16();
                ushort y = reader.ReadUInt16();
                ushort width = reader.ReadUInt16();
                ushort height = reader.ReadUInt16();

                short xOffset = reader.ReadInt16();
                short yOffset = reader.ReadInt16();
                short xAdvance = reader.ReadInt16();

                byte page = reader.ReadByte();

                var texture = textures[page];

                var region = new TextureRegion(texture, x, y, width, height);

                var character = new BitmapFontCharacter(
                    id,
                    region,
                    xOffset,
                    yOffset,
                    xAdvance
                );

                characters.Add(character);
                characterLookup[id] = character;
            }

            // --- Kernings ---
            int kerningCount = reader.ReadInt32();

            for (int i = 0; i < kerningCount; i++)
            {
                int first = reader.ReadInt32();
                int second = reader.ReadInt32();
                int amount = reader.ReadInt16();

                if (characterLookup.TryGetValue(first, out var firstChar))
                {
                    // Store kerning in the FIRST character (important for your runtime logic)
                    firstChar.Kernings[second] = amount;
                }
            }

            // --- Create final font ---
            var font = new BitmapFont(
                face: string.Empty,     // Not stored in pipeline (optional)
                size: 0,                // Not stored (can be added later if needed)
                lineHeight: lineHeight,
                characters: characters
            );

            return font;
        }
    }
}
using System.Collections.Generic;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;
using FreedomEngine.Graphics.BitmapFonts;

namespace FreedomEngine.Content
{
    public class BitmapFontReader : ContentTypeReader<BitmapFont>
    {
        protected override BitmapFont Read(ContentReader reader, BitmapFont existingInstance)
        {
            int textureCount = reader.ReadInt32();
            var textures = new Texture2D[textureCount];

            for (int i = 0; i < textureCount; i++)
            {
                string name = reader.ReadString();
                textures[i] = reader.ContentManager.Load<Texture2D>($"Assets/Fonts/{name}");
            }

            int lineHeight = reader.ReadInt32();
            int baseline = reader.ReadInt32();

            int charCount = reader.ReadInt32();

            var characters = new BitmapFontCharacter[charCount];
            var lookup = new Dictionary<int, BitmapFontCharacter>(charCount);

            for (int i = 0; i < charCount; i++)
            {
                int id = reader.ReadInt32();

                ushort x = reader.ReadUInt16();
                ushort y = reader.ReadUInt16();
                ushort w = reader.ReadUInt16();
                ushort h = reader.ReadUInt16();

                short xo = reader.ReadInt16();
                short yo = reader.ReadInt16();
                short xa = reader.ReadInt16();

                byte page = reader.ReadByte();

                var region = new TextureRegion(textures[page], x, y, w, h);

                var c = new BitmapFontCharacter(id, region, xo, yo, xa);

                characters[i] = c;
                lookup[id] = c;
            }

            int kerningCount = reader.ReadInt32();

            for (int i = 0; i < kerningCount; i++)
            {
                int first = reader.ReadInt32();
                int second = reader.ReadInt32();
                int amount = reader.ReadInt16();

                if (lookup.TryGetValue(first, out var c))
                    c.Kernings[second] = amount;
            }

            return new BitmapFont("", 0, lineHeight, characters, baseline);
        }
    }
}
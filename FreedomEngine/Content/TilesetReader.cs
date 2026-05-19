using System;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;

namespace FreedomEngine.Content
{
    public class TilesetReader : ContentTypeReader<Tileset>
    {
        protected override Tileset Read(ContentReader reader, Tileset existingInstance)
        {
            var imageHeight = reader.ReadInt32();
            var imageWidth = reader.ReadInt32();
            var margin = reader.ReadUInt16();
            var name = reader.ReadString();
            var spacing = reader.ReadUInt16();
            var tileHeight = reader.ReadUInt16();
            var tileWidth = reader.ReadUInt16();
            var count = reader.ReadInt32();

            Texture2D texture = reader.ContentManager.Load<Texture2D>($"Assets/Textures/Tilesets/{name}");
            TextureRegion region = new(texture, 0, 0, imageWidth, imageHeight);

            Tileset tileset = new(region, tileWidth, tileHeight, margin, margin, spacing, spacing);

            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadUInt16();
                var countFrames = reader.ReadInt32();

                if (countFrames > 0)
                {
                    bool isMonoDelay = reader.ReadBoolean();
                    ushort[] listFrames = new ushort[countFrames];

                    if (isMonoDelay)
                    {
                        ushort globalDelay = reader.ReadUInt16();
                        TimeSpan sharedDelay = TimeSpan.FromMilliseconds(globalDelay);

                        for (int j = 0; j < countFrames; j++)
                        {
                            listFrames[j] = reader.ReadUInt16();
                        }

                        tileset.AddAnimation(id, listFrames, sharedDelay);
                    }
                    else
                    {
                        TimeSpan[] delays = new TimeSpan[countFrames];

                        for (int j = 0; j < countFrames; j++)
                        {
                            listFrames[j] = reader.ReadUInt16();
                            delays[j] = TimeSpan.FromMilliseconds(reader.ReadUInt16());
                        }

                        tileset.AddAnimation(id, listFrames, delays);
                    }
                }
            }

            return tileset;
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;

namespace FreedomEngine.Content
{
    public class TilesetReader : ContentTypeReader<Tileset>
    {
        protected override Tileset Read(ContentReader reader, Tileset existingInstance)
        {
            reader.ReadInt32();
            reader.ReadInt32();
            var margin = reader.ReadUInt16();
            var name = reader.ReadString();
            var spacing = reader.ReadUInt16();
            var tileHeight = reader.ReadUInt16();
            var tileWidth = reader.ReadUInt16();

            var texture = reader.ContentManager.Load<Texture2D>($"Assets/Textures/Tilesets/{name}");

            int availableWidth = texture.Width - (2 * margin);
            int availableHeight = texture.Height - (2 * margin);

            var columns = (ushort)((availableWidth + spacing) / (tileWidth + spacing));
            var rows = (ushort)((availableHeight + spacing) / (tileHeight + spacing));
            var countGrid = (ushort)(columns * rows);

            var tiles = new Rectangle[countGrid];

            int index = 0;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    int x = margin + col * (tileWidth + spacing);
                    int y = margin + row * (tileHeight + spacing);

                    tiles[index] = new Rectangle(x, y, tileWidth, tileHeight);
                    index++;
                }
            }

            var dictionary = new Dictionary<ushort, TileAnimation>();
            var count = reader.ReadInt32();

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

                        dictionary.Add(id, new TileAnimation(listFrames, sharedDelay));
                    }
                    else
                    {
                        TimeSpan[] delays = new TimeSpan[countFrames];

                        for (int j = 0; j < countFrames; j++)
                        {
                            listFrames[j] = reader.ReadUInt16();
                            delays[j] = TimeSpan.FromMilliseconds(reader.ReadUInt16());
                        }

                        dictionary.Add(id, new TileAnimation(listFrames, delays));
                    }
                }
            }

            // Create the tileset with the loaded data
            var tileset = new Tileset(texture, tiles, dictionary);

            return tileset;
        }
    }
}
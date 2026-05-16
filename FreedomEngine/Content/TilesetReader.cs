using System;
using System.Collections.Generic;

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
            TextureRegion region = new TextureRegion(texture, 0, 0, imageWidth, imageHeight);

            Tileset tileset = new Tileset(region, tileWidth, tileHeight, margin, margin, spacing, spacing);

            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadUInt16();
                var countFrames = reader.ReadInt32();

                ushort[] _listFrames = new ushort[countFrames];
                TimeSpan[] _delays = new TimeSpan[countFrames];

                for (int j = 0; j < countFrames; j++)
                {
                    var frameTileID = reader.ReadUInt16();
                    var delay = reader.ReadUInt16();

                    _listFrames[j] = frameTileID;
                    _delays[j] = TimeSpan.FromMilliseconds(delay);
                }

                tileset.AddAnimation(id, _listFrames, _delays);
            }

            return tileset;
        }
    }
}

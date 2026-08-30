using FreedomEngine.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;

namespace FreedomEngine.Content
{
    public class TextureAtlasReader : ContentTypeReader<TextureAtlas>
    {
        protected override TextureAtlas Read(ContentReader reader, TextureAtlas existingInstance)
        {
            var texturePath = reader.ReadString();
            var texture = reader.ContentManager.Load<Texture2D>(texturePath);

            var textureAtlas = new TextureAtlas(texture);
            var spriteCount = reader.ReadInt32();

            for (int i = 0; i < spriteCount; i++)
            {
                var name = reader.ReadString();
                var xOrigin = reader.ReadSingle();
                var yOrigin = reader.ReadSingle();
                var width = reader.ReadInt32();
                var height = reader.ReadInt32();
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                var frameCount = reader.ReadInt32();
                var delay = reader.ReadSingle();
                var xSpace = reader.ReadInt32();

                var frames = new Rectangle[frameCount];

                for (int j = 0; j < frameCount; j++)
                {
                    var frameX = x + (j * (width + xSpace));
                    var frameY = y;
                    var region = new Rectangle(frameX, frameY, width, height);
                    frames[j] = region;
                }

                var sprite = new Sprite(texture, frames, TimeSpan.FromMilliseconds(delay), new Vector2(xOrigin, yOrigin));
                textureAtlas.AddSprite(name, sprite);
            }

            return textureAtlas;
        }
    }
}

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace FreedomEngine.Content.Pipeline.TextureAtlas
{
    [ContentTypeWriter]
    public class TextureAtlasWriter : ContentTypeWriter<TextureAtlasContent>
    {
        protected override void Write(ContentWriter output, TextureAtlasContent value)
        {
            output.Write(value.TexturePath);
            output.Write(value.Sprites.Count);

            foreach (var sprite in value.Sprites)
            {
                output.Write(sprite.Name);
                output.Write(sprite.XOrigin);
                output.Write(sprite.YOrigin);
                output.Write(sprite.Width);
                output.Write(sprite.Height);
                output.Write(sprite.X);
                output.Write(sprite.Y);
                output.Write(sprite.FrameCount);
                output.Write(sprite.Delay);
                output.Write(sprite.XSpace);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Graphics.TextureAtlas, FreedomEngine";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Content.TextureAtlasReader, FreedomEngine";
        }
    }
}

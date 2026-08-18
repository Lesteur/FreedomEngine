using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.TextureAtlas
{
    [ContentProcessor(DisplayName = "Texture Atlas Processor")]
    public class TextureAtlasProcessor : ContentProcessor<TextureAtlasContent, TextureAtlasContent>
    {
        public override TextureAtlasContent Process(TextureAtlasContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing Texture Atlas");

            return input;
        }
    }
}
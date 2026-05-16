using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    [ContentProcessor(DisplayName = "Tiled Tileset Processor")]
    public class TiledTilesetProcessor : ContentProcessor<TiledTilesetContent, TiledTilesetContent>
    {
        public override TiledTilesetContent Process(TiledTilesetContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing Tiled tileset: {0}", input.Name);

            // Here you can process or validate the tileset content before it gets compiled
            // For example, ensuring positive dimensions or recomputing paths

            return input;
        }
    }
}
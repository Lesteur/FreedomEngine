using Microsoft.Xna.Framework.Content.Pipeline;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    [ContentProcessor(DisplayName = "Tiled Tileset Processor")]
    public class TiledTilesetProcessor : ContentProcessor<TiledTilesetContent, TiledTilesetContent>
    {
        public override TiledTilesetContent Process(TiledTilesetContent input, ContentProcessorContext context)
        {
            context.Logger.LogMessage("Processing Tiled tileset: {0}", input.Name);

            // Check if animations exist and if they have a mono frame delay
            if (input.Tiles != null)
            {
                foreach (var tile in input.Tiles)
                {
                    if (tile.Animation != null && tile.Animation.Count > 0)
                    {
                        bool monoDelay = true;
                        int firstDuration = tile.Animation[0].Duration;

                        for (int i = 1; i < tile.Animation.Count; i++)
                        {
                            if (tile.Animation[i].Duration != firstDuration)
                            {
                                monoDelay = false;
                                break;
                            }
                        }

                        tile.MonoFrameDelay = monoDelay;
                    }
                }
            }

            return input;
        }
    }
}
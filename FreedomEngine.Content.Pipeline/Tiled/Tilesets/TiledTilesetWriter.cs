using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    [ContentTypeWriter]
    public class TiledTilesetWriter : ContentTypeWriter<TiledTilesetContent>
    {
        protected override void Write(ContentWriter output, TiledTilesetContent value)
        {
            // Write basic properties
            //output.Write(value.Columns);
            output.Write(value.ImageHeight);
            output.Write(value.ImageWidth);
            output.Write(value.Margin);
            output.Write(value.Name ?? string.Empty);
            output.Write(value.Spacing);
            //output.Write(value.TileCount);
            output.Write(value.TileHeight);
            output.Write(value.TileWidth);

            // Write tiles array
            if (value.Tiles != null)
            {
                output.Write(value.Tiles.Count);
                foreach (var tile in value.Tiles)
                {
                    output.Write(tile.Id);
                    
                    if (tile.Animation != null)
                    {
                        output.Write(tile.Animation.Count);
                        foreach (var frame in tile.Animation)
                        {
                            output.Write(frame.TileId);
                            output.Write(frame.Duration);
                        }
                    }
                    else
                    {
                        output.Write(0); // 0 animation frames
                    }
                }
            }
            else
            {
                output.Write(0); // 0 tiles
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // You will need to implement a TiledTilesetReader in your game project
            // to read this binary data. Return its fully qualified assembly name here.
            return "FreedomEngine.Content.TilesetReader, FreedomEngine";
        }
    }
}
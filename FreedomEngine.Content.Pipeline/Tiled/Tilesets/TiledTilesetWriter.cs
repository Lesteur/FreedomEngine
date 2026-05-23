using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    [ContentTypeWriter]
    public class TiledTilesetWriter : ContentTypeWriter<TiledTilesetContent>
    {
        protected override void Write(ContentWriter output, TiledTilesetContent value)
        {
            output.Write(value.ImageHeight);
            output.Write(value.ImageWidth);
            output.Write((ushort)value.Margin);
            output.Write(value.Name ?? string.Empty);
            output.Write((ushort)value.Spacing);
            output.Write((ushort)value.TileHeight);
            output.Write((ushort)value.TileWidth);

            // Write tiles array
            if (value.Tiles != null)
            {
                output.Write(value.Tiles.Count);
                foreach (var tile in value.Tiles)
                {
                    output.Write((ushort)tile.Id);

                    if (tile.Animation != null && tile.Animation.Count > 0)
                    {
                        output.Write(tile.Animation.Count);
                        output.Write(tile.MonoFrameDelay);

                        if (tile.MonoFrameDelay)
                        {
                            output.Write((ushort)tile.Animation[0].Duration);
                            foreach (var frame in tile.Animation)
                            {
                                output.Write((ushort)frame.TileId);
                            }
                        }
                        else
                        {
                            foreach (var frame in tile.Animation)
                            {
                                output.Write((ushort)frame.TileId);
                                output.Write((ushort)frame.Duration);
                            }
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

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Graphics.Tileset, FreedomEngine";
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "FreedomEngine.Content.TilesetReader, FreedomEngine";
        }
    }
}
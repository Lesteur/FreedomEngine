using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FreedomEngine.Content.Pipeline.Tiled.Tilesets
{
    public class TiledTilesetContent
    {
        [JsonPropertyName("columns")]
        public int Columns { get; set; }

        [JsonPropertyName("imageheight")]
        public int ImageHeight { get; set; }

        [JsonPropertyName("imagewidth")]
        public int ImageWidth { get; set; }

        [JsonPropertyName("margin")]
        public ushort Margin { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("spacing")]
        public ushort Spacing { get; set; }

        [JsonPropertyName("tilecount")]
        public int TileCount { get; set; }

        [JsonPropertyName("tileheight")]
        public ushort TileHeight { get; set; }

        [JsonPropertyName("tilewidth")]
        public ushort TileWidth { get; set; }

        [JsonPropertyName("tiles")]
        public List<Tile> Tiles { get; set; }
    }

    public class TileAnimation
    {
        [JsonPropertyName("duration")]
        public ushort Duration { get; set; }

        [JsonPropertyName("tileid")]
        public ushort TileId { get; set; }
    }

    public class Tile
    {
        [JsonPropertyName("animation")]
        public List<TileAnimation> Animation { get; set; }

        [JsonPropertyName("id")]
        public ushort Id { get; set; }

        // Determine if all animation frames share the same duration
        [JsonIgnore]
        public bool MonoFrameDelay { get; set; }
    }
}

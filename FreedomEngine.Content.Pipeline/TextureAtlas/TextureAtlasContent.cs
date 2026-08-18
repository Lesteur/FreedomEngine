using System.Collections.Generic;

namespace FreedomEngine.Content.Pipeline.TextureAtlas
{
    public class SpriteContent
    {
        public string Name { get; set; } = string.Empty;

        public float XOrigin { get; set; } = 0;

        public float YOrigin { get; set; } = 0;

        public int Width { get; set; } = 0;

        public int Height { get; set; } = 0;

        public int X { get; set; } = 0;

        public int Y { get; set; } = 0;

        public int FrameCount { get; set; } = 0;

        public float Delay { get; set; } = 0;

        public int XSpace { get; set; } = 0;

        public SpriteContent()
        {
        }
    }

    public class TextureAtlasContent
    {
        public string TexturePath { get; set; } = string.Empty;

        public List<SpriteContent> Sprites { get; set; }

        public TextureAtlasContent()
        {
            Sprites = [];
        }
    }
}

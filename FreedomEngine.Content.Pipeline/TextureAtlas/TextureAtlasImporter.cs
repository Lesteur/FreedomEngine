using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace FreedomEngine.Content.Pipeline.TextureAtlas
{
    [ContentImporter(".xml", DisplayName = "Texture Atlas Importer", DefaultProcessor = "TextureAtlasProcessor")]
    public class TextureAtlasImporter : ContentImporter<TextureAtlasContent>
    {
        public override TextureAtlasContent Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing Tiled tileset: {0}", filename);

            try
            {
                var textureAtlasContent = new TextureAtlasContent();

                Stream stream = File.OpenRead(filename);
                XmlReader reader = XmlReader.Create(stream);
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                string texturePath = root.Element("Texture").Value;
                textureAtlasContent.TexturePath = texturePath;

                var sprites = root.Element("Sprites")?.Elements("Sprite");

                if (sprites != null)
                {
                    foreach (var sprite in sprites)
                    {
                        var spriteContent = new SpriteContent();

                        string name = sprite.Attribute("name").Value;

                        float xOrigin = float.Parse(sprite.Attribute("xorigin")?.Value ?? "0");
                        float yOrigin = float.Parse(sprite.Attribute("yorigin")?.Value ?? "0");

                        int width = int.Parse(sprite.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(sprite.Attribute("height")?.Value ?? "0");

                        int x = int.Parse(sprite.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(sprite.Attribute("y")?.Value ?? "0");

                        int frameCount = int.Parse(sprite.Attribute("framecount")?.Value ?? "0");

                        float delay = float.Parse(sprite.Attribute("delay")?.Value ?? "0");

                        int xspace = int.Parse(sprite.Attribute("xspace")?.Value ?? "0");

                        spriteContent.Name = name;
                        spriteContent.XOrigin = xOrigin;
                        spriteContent.YOrigin = yOrigin;
                        spriteContent.Width = width;
                        spriteContent.Height = height;
                        spriteContent.X = x;
                        spriteContent.Y = y;
                        spriteContent.FrameCount = frameCount;
                        spriteContent.Delay = delay;
                        spriteContent.XSpace = xspace;

                        textureAtlasContent.Sprites.Add(spriteContent);
                    }
                }

                return textureAtlasContent;
            }
            catch (Exception ex)
            {
                context.Logger.LogMessage("Failed to import Tiled tileset: {0}. Error: {1}", filename, ex.Message);
                throw;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using FreedomEngine.Core;
using FreedomEngine.Readers;

using MyGame.Scripts.Scenes;

namespace MyGame.Scripts
{
    public class Core : Application
    {
        protected override void Initialize()
        {
            _pathDefaultFont = "Assets/Fonts/Pixeloid";

            base.Initialize();

            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Readers.TilesetReader, FreedomEngine", () => new TilesetReader());
            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Readers.BitmapFontReader, FreedomEngine", () => new BitmapFontReader());
            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Readers.TextureAtlasReader, FreedomEngine", () => new TextureAtlasReader());
            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Readers.CutsceneReader, FreedomEngine", () => new CutsceneReader());

            // TODO: Add your initialization logic here
            ChangeScene(new Scene1());
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // TODO: Add your update logic here
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            // TODO: Add your drawing code here
        }
    }
}

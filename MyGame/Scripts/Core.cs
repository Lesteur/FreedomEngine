using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Content;

using MyGame.Scripts.Scenes;

namespace MyGame.Scripts
{
    public class Core : Application
    {
        protected override void Initialize()
        {
            base.Initialize();

            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Content.TilesetReader, FreedomEngine", () => new TilesetReader());
            ContentTypeReaderManager.AddTypeCreator("FreedomEngine.Content.BitmapFontReader, FreedomEngine", () => new BitmapFontReader());

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

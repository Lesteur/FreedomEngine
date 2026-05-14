using Microsoft.Xna.Framework;

using FreedomEngine.Core;

using MyGame.Scripts.Scenes;

namespace MyGame.Scripts
{
    public class Core : Application
    {
        protected override void Initialize()
        {
            base.Initialize();

            // TODO: Add your initialization logic here
            //ChangeScene(new MyScene());
            ChangeScene(new SceneShadow());
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

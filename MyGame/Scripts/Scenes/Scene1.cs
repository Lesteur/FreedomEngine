using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Core;

namespace MyGame.Scripts.Scenes
{
    internal class Scene1 : Scene
    {
        public override void Update(GameTime gameTime)
        {

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                //Core.Coroutines.StartCoroutine(TestCoroutine());
                Application.ChangeScene(new MyScene());
            }

            base.Update(gameTime);
        }
    }
}

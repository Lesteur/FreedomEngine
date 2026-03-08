using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;

namespace MyGame.Scripts.Scenes
{
    public class MyScene : Scene
    {
        private Texture _texture;
        private Sprite _sprite;
        private Entity _entity;

        private Matrix _scalingMatrix;

        public override void Initialize()
        {
            // LoadContent is called during base.Initialize().
            base.Initialize();

            _sprite = new Sprite(_texture, 1, 28, 45);
            _entity = new Entity(_sprite, 20, 20);
        }

        public override void LoadContent()
        {
            _texture = new Texture(Content, "Assets/Textures/spr_jonathan");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void DrawWorld(GameTime gameTime)
        {
            _entity.Draw(Application.SpriteBatch);
        }
    }
}

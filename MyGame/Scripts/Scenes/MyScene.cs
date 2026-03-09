using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;

namespace MyGame.Scripts.Scenes
{
    public class MyScene : Scene
    {
        private Texture2D _texture;
        private Animation _animation;
        private Entity _entity;

        public override void Initialize()
        {
            // LoadContent is called during base.Initialize().
            base.Initialize();

            _animation = new Animation(_texture, 14, TimeSpan.FromSeconds(0.05));
            _entity = new Entity(_animation, 20, 20);
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _entity.Update(gameTime);
        }

        public override void DrawWorld(GameTime gameTime)
        {
            _entity.Draw(Application.SpriteBatch);
        }
    }
}
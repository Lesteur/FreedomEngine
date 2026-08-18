using FreedomEngine.Components.Collisions;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Collections.Special.Metroidvania;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using MyGame.Scripts.Metroid;

namespace MyGame.Scripts.Scenes
{
    public class Scene1 : Scene
    {
        private PlayerSamus _entity;

        private CollisionMask _collision1;
        private CollisionMask _collision2;
        private CollisionMask _collision3;

        private MovingPlatform _movingPlatform;

        public override void Initialize()
        {
            base.Initialize();

            _width = 700;
            _height = 400;

            _cameraLimitsMin = new Vector2(640 / 2f, 360 / 2f);
            _cameraLimitsMax = new Vector2(_width - 320, _height - 180);

            var collision = new RectangleCollision(Vector2.Zero, 1, 32, 48);
            _entity = new PlayerSamus(Vector2.Zero, collision);

            _following = _entity;

            _collision1 = new RectangleCollision(new Vector2(220, 200), 1, 50, 50);

            _collision2 = new RectangleCollision(Vector2.Zero, 1, 50, 50, OneWayCollision.Top);
            _movingPlatform = new MovingPlatform(null, new Vector2(220, 250), _collision2);

            _collision3 = new RectangleCollision(new Vector2(0, 350), 1, 700, 50);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            PlayerSamus.LoadContent(Content);
        }

        public override void Update(GameTime gameTime)
        {
            _movingPlatform.Update(gameTime);
            _entity.Update(gameTime);

	        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                Application.ChangeScene(new MyScene());
            }

            base.Update(gameTime);
        }

        public override void DrawWorld(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                transformMatrix: WorldCamera.TransformMatrix * _scalingMatrix
            );

            _entity.Draw(spriteBatch);

            _collision1.Draw(spriteBatch);
            _collision3.Draw(spriteBatch);

            _movingPlatform.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}

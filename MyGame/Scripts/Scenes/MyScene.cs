using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;

namespace MyGame.Scripts.Scenes
{
    public class MyScene : Scene
    {
        private Texture2D _texture;
        private Texture2D _textureTileset;

        private Sprite _animation;
        private Tileset _tileset;

        private Entity _entity;
        private Tilemap _tilemap;

        public override void Initialize()
        {
            // LoadContent is called during base.Initialize().
            base.Initialize();

            _animation = new Sprite(_texture, 14, TimeSpan.FromSeconds(0.05));
            _entity = new Entity(_animation, 0, 0);

            _following = _entity;

            var _textureRegion = new TextureRegion(_textureTileset, 0, 0, 170, 136);

            var _list1 = new List<UInt16> { 0, 1, 2, 3 };
            var _list2 = new List<UInt16> { 5, 5, 5, 5, 6, 7, 8 };
            _tileset = new Tileset(_textureRegion, 16, 16, 1, 1, 1, 1);
            _tileset.AddAnimation(0, _list1);
            _tileset.AddAnimation(5, _list2);

            _tilemap = new Tilemap(_tileset, 10, 10);

            for (UInt16 i = 0; i < _tilemap.Count; i++)
            {
                if (i % 2 == 0)
                    _tilemap.SetTile(i, 0);
                else
                    _tilemap.SetTile(i, 5);
            }
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
            _textureTileset = Content.Load<Texture2D>("Assets/Textures/TilesetMario");
        }

        public override void Update(GameTime gameTime)
        {
            if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                _entity.X -= 1;
            }
            else if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                _entity.X += 1;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
            {
                _entity.Y += 1;
            }
            else if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
            {
                _entity.Y -= 1;
            }

            _tilemap.Update(gameTime);
            _entity.Update(gameTime);

            base.Update(gameTime);
        }

        public override void DrawWorld(GameTime gameTime)
        {
            _tilemap.Draw(Application.SpriteBatch);
            _entity.Draw(Application.SpriteBatch);
        }

        public override void DrawUI(GameTime gameTime)
        {
            //_entity.Draw(Application.SpriteBatch);
        }
    }
}
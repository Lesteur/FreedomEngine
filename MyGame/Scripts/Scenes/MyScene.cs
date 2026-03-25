using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Components;
using FreedomEngine.Graphics.BitmapFonts;

namespace MyGame.Scripts.Scenes
{
    public class MyScene : Scene
    {
        private Texture2D _texture;
        private Texture2D _textureTileset;
        private SoundEffect _soundEffect;

        private Sprite _animation;
        private Tileset _tileset;

        private Entity _entity;
        private Tilemap _tilemap;

        private BitmapFont _font;
        private BitmapText _bitmapText1;
        private BitmapText _bitmapText2;
        private BitmapText _bitmapText3;
        private BitmapText _bitmapText4;

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

            _bitmapText1 = new BitmapText(_font, "Hello, [scale 2]World[\\scale]! ç Êê j[scale 0.5]Ê[\\scale] Salut salut salut salut salut salut salut salut hé !!!", 300, 150);
            //_bitmapText1.DefaultColor = Color.Red;
            _bitmapText1.VerticalAlignment = TextVerticalAlignment.Top;
            _bitmapText1.HorizontalAlignment = TextHorizontalAlignment.Right;
            _bitmapText1.MaxWidth = 200;
            _bitmapText1.JumpHeight = 20;

            _bitmapText2 = new BitmapText(_font, "Hello, [scale 2]World[\\scale]! ç Êê j[scale 0.5]Ê[\\scale] Salut salut salut salut salut salut salut salut hé !!!", 300, 150);
            //_bitmapText1.DefaultColor = Color.Red;
            _bitmapText2.VerticalAlignment = TextVerticalAlignment.Top;
            _bitmapText2.HorizontalAlignment = TextHorizontalAlignment.Left;
            _bitmapText2.MaxWidth = 200;
            _bitmapText2.JumpHeight = 20;
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
            _textureTileset = Content.Load<Texture2D>("Assets/Textures/TilesetMario");
            _soundEffect = Content.Load<SoundEffect>("Assets/Audio/sfx_chest");
            _font = Content.Load<BitmapFont>("Assets/Fonts/Determination");
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

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
            {
                Core.Audio.PlaySoundEffect(_soundEffect);
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.A))
            {
                WorldCamera.Rotation += MathHelper.ToRadians(1);
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.Z))
            {
                WorldCamera.Rotation += MathHelper.ToRadians(-1);
            }

            _tilemap.Update(gameTime);
            _entity.Update(gameTime);

            _bitmapText1.Update(gameTime);
            // _bitmapText1.Rotation += MathHelper.ToRadians(1);

            base.Update(gameTime);
        }

        public override void DrawWorld(GameTime gameTime)
        {
            _tilemap.Draw(Application.SpriteBatch);
            _entity.Draw(Application.SpriteBatch);
        }

        public override void DrawUI(GameTime gameTime)
        {
            _bitmapText1.Draw(Application.SpriteBatch);
            _bitmapText2.Draw(Application.SpriteBatch);
        }
    }
}
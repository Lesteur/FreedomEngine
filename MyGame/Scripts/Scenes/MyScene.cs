using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Components;
using FreedomEngine.Graphics.BitmapFonts;
using FreedomEngine.Collections.Coroutines;
using FreedomEngine.Collections;

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
        private Text _bitmapText1;

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

            _tilemap = new(_tileset, 15, 15)
            {
                X = 400,
                Y = 150
            };

            for (UInt16 i = 0; i < _tilemap.Count; i++)
            {
                if (i % 2 == 0)
                    _tilemap.SetTile(i, 0);
                else
                    _tilemap.SetTile(i, 5);
            }

            _bitmapText1 = new(_font, "Ê Salut, [color red][shake 0.5]tout le monde[\\shake][\\color] !\nJe suis un énorme optimiste qui adore les [color blue]jeux vidéo[\\color] et qui adore en créer. Héhéhéhéhéhéhéhé héhéhéhéhé héhéhhéhéhéhéhhéh " +
                "Je pense également que les chats sont de [rainbow][wave 2]merveilleuses créatures[\\wave][\\rainbow] mais les [rainbow]chiens[\\rainbow] sont également des êtres fabuleux !", 400, 150)
            {
                VerticalAlignment = TextVerticalAlignment.Middle,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                MaxWidth = 400,
                JumpHeight = 25
            };
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
            _textureTileset = Content.Load<Texture2D>("Assets/Textures/TilesetMario");
            _soundEffect = Content.Load<SoundEffect>("Assets/Audio/sfx_chest");
            _font = Content.Load<BitmapFont>("Assets/Fonts/Pixeloid");
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
                Core.Coroutines.StartCoroutine(TestCoroutine());
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
            
            base.Update(gameTime);
        }

        public override void DrawWorld(GameTime gameTime)
        {
            Application.SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                transformMatrix: WorldCamera.TransformMatrix * _scalingMatrix
            );

            _tilemap.Draw(Application.SpriteBatch);
            _entity.Draw(Application.SpriteBatch);

            Application.SpriteBatch.End();
        }

        public override void DrawUI(GameTime gameTime)
        {
            _bitmapText1.Draw(Application.SpriteBatch);
        }


        private IEnumerator TestCoroutine()
        {
            Logger.Info("Coroutine started.");

            yield return new WaitForSeconds(TimeSpan.FromSeconds(1));

            Logger.Info("1 second has passed.");

            yield return new WaitForSeconds(TimeSpan.FromSeconds(2));

            Logger.Info("2 more seconds have passed.");

            yield return new WaitUntil(() => Core.Input.Keyboard.IsKeyDown(Keys.Space));

            Logger.Info("Space key was pressed. Coroutine ending.");
        }
    }
}
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
    public class SceneShadow : Scene
    {
        private Texture2D _texture;
        private Texture2D _textureTileset;
        private BitmapFont _font;

        private Sprite _animation;
        private Tileset _tileset;

        private SoundEffect _soundEffect;

        private Entity _entity;
        private Tilemap _tilemap;
        private Text _bitmapText;

        private Texture2D _lightTexture;
        private RenderTarget2D _lightMap;
        private BlendState _multiplyBlend;

        public override void Initialize()
        {
            base.Initialize();

            _animation = new Sprite(_texture, 14, TimeSpan.FromSeconds(0.05));
            _entity = new Entity(_animation, 0, 0);

            _following = _entity;

            var _textureRegion = new TextureRegion(_textureTileset, 0, 0, 170, 136);
            _tileset = new Tileset(_textureRegion, 16, 16, 1, 1, 1, 1);

            ushort[] _list1 = new ushort[] { 0, 1, 2, 3 };
            ushort[] _list2 = new ushort[] { 5, 6, 7, 8 };
            TimeSpan[] _delays = new TimeSpan[] { TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(0.125), TimeSpan.FromSeconds(0.125), TimeSpan.FromSeconds(0.125) };
            
            _tileset.AddAnimation(0, _list1, TimeSpan.FromSeconds(0.125));
            _tileset.AddAnimation(5, _list2, _delays);

            _tilemap = new(_tileset, 15, 15)
            {
                X = 400,
                Y = 150
            };

            for (ushort i = 0; i < _tilemap.Count; i++)
            {
                if (i % 2 == 0)
                    _tilemap.SetTile(i, 0);
                else
                    _tilemap.SetTile(i, 5);
            }


            _bitmapText = new(_font, "Ê Salut, [color red][shake 0.5]tout le monde[\\shake][\\color] !\nJe suis un énorme optimiste qui adore les [color blue]jeux vidéo[\\color] et qui adore en créer. Héhéhéhéhéhéhéhé héhéhéhéhé héhéhhéhéhéhéhhéh " +
                "Je pense également que les chats sont de [rainbow][wave 2]merveilleuses créatures[\\wave][\\rainbow] mais les [rainbow]chiens[\\rainbow] sont également des êtres fabuleux !", new Vector2(400, 150))
            {
                VerticalAlignment = TextVerticalAlignment.Middle,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                MaxWidth = 400,
                JumpHeight = 25
            };


            _lightTexture = new Texture2D(Application.GraphicsDevice, 1, 1);
            _lightTexture.SetData([Color.White]);

            _lightMap = new RenderTarget2D(Application.GraphicsDevice, _width, _height);

            _multiplyBlend = new BlendState()
            {
                ColorBlendFunction = BlendFunction.Add,
                ColorSourceBlend = Blend.DestinationColor,
                ColorDestinationBlend = Blend.Zero
            };
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
            _textureTileset = Content.Load<Texture2D>("Assets/Textures/TilesetMario");
            _soundEffect = Content.Load<SoundEffect>("Assets/Audio/sfx_chest");
            _font = Content.Load<BitmapFont>("Assets/Fonts/Pixeloid");

            base.LoadContent();
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

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                //Core.Coroutines.StartCoroutine(TestCoroutine());
                Application.ChangeScene(new MyScene());
            }

            _tilemap.Update(gameTime);
            _entity.Update(gameTime);

            _bitmapText.Update(gameTime);

            base.Update(gameTime);
        }

        public override void DrawWorld(SpriteBatch spriteBatch)
        {
            // Set the render target to draw our lights and shadows
            Application.GraphicsDevice.SetRenderTarget(_lightMap);
            // Clear with an ambient dark color (e.g., dark gray/blue).
            Application.GraphicsDevice.Clear(new Color(20, 20, 30, 255));

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.Additive, // Additive blending is used to combine overlapping lights
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: WorldCamera.TransformMatrix
            );

            spriteBatch.Draw(_lightTexture, _entity.Position, new Rectangle(0, 0, 100, 100), Color.White, 0f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_lightTexture, new Vector2(400, 100), new Rectangle(0, 0, 100, 100), new Color(255, 100, 100), 0f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_lightTexture, new Vector2(500, 300), new Rectangle(0, 0, 100, 100), new Color(100, 255, 100), 0f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_lightTexture, new Vector2(400, 200), new Rectangle(0, 0, 100, 100), new Color(100, 100, 255), 0f, new Vector2(0.5f, 0.5f), 1f, SpriteEffects.None, 0f);

            spriteBatch.End();
            Application.GraphicsDevice.SetRenderTarget(null);


            Application.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                transformMatrix: WorldCamera.TransformMatrix * _scalingMatrix
            );

            _tilemap.Draw(Application.SpriteBatch);
            _entity.Draw(Application.SpriteBatch);

            spriteBatch.End();


            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: _multiplyBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: _scalingMatrix
            );

            // Draw the generated light mask on top of everything
            spriteBatch.Draw(_lightMap, Vector2.Zero, Color.White);

            spriteBatch.End();
        }

        public override void DrawUI(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: UICamera.TransformMatrix * _scalingMatrix
            );

            _bitmapText.Draw(spriteBatch);

            spriteBatch.End();
        }

        public override void UnloadContent()
        {
            _lightTexture?.Dispose();
            _lightMap?.Dispose();
            _multiplyBlend?.Dispose();

            base.UnloadContent();
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

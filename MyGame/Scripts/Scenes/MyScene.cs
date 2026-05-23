using System;
using System.Collections;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections;
using FreedomEngine.Collections.Coroutines;
using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Graphics.BitmapFonts;
using FreedomEngine.UI;
using FreedomEngine.Graphics.Particles;
using FreedomEngine.Components.Collisions;

namespace MyGame.Scripts.Scenes
{
    public class MyScene : Scene
    {
        private Texture2D _texture;
        private BitmapFont _font;

        private Sprite _animation;
        private Tileset _tileset;

        private Entity _entity;
        private Tilemap _tilemap;
        private Text _bitmapText;
        private ParticleEmitter<ParticleDefault> _particleEmitter;
        private CollisionMask _collision;

        // The texture used for the background pattern.
        private Texture2D _backgroundPattern;
        // The destination rectangle for the background pattern to fill.
        private Rectangle _backgroundDestination;
        // The offset to apply when drawing the background pattern so it appears to
        // be scrolling.
        private Vector2 _backgroundOffset;
        // The speed that the background pattern scrolls.
        private readonly float _scrollSpeed = 50.0f;

        // Variable to hold the reference to our active tween
        private ITween _tween;

        public override void Initialize()
        {
            base.Initialize();

            _particleEmitter = new ParticleEmitter<ParticleDefault>(Core.PixelTexture, 100, new Vector2(200, 200), texture => new ParticleDefault(texture));

            _animation = new Sprite(_texture, 14, TimeSpan.FromSeconds(0.05));
            _entity = new Entity(_animation, 0, 0);
            
            _following = _entity;

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

            Tilemap.Camera = WorldCamera;
            Entity.Camera = WorldCamera;

            _bitmapText = new(_font, "Ê Salut, [color red][shake 0.5]tout le monde[\\shake][\\color] !\nJe suis un énorme optimiste qui adore les [color blue]jeux vidéo[\\color] et qui adore en créer. Héhéhéhéhéhéhéhé héhéhéhéhé héhéhhéhéhéhéhhéh " +
                "Je pense également que les chats sont de [rainbow][wave 2]merveilleuses créatures[\\wave][\\rainbow] mais les [rainbow]chiens[\\rainbow] sont également des êtres fabuleux !", new Vector2(400, 150))
            {
                VerticalAlignment = TextVerticalAlignment.Middle,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                MaxWidth = 400,
                JumpHeight = 25
            };

            // Initialize the offset of the background pattern at zero.
            _backgroundOffset = Vector2.Zero;

            // Set the background pattern destination rectangle to fill the entire
            // screen background.
            _backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

            //_collision = new RectangleCollision(new Vector2(100, 100), 50, 50);
            //_collision = new LineCollision(new Vector2(100, 100), new Vector2(200, 150));
            _collision = new CircleCollision(new Vector2(400, 150), 25);
        }

        public override void LoadContent()
        {
            _texture = Content.Load<Texture2D>("Assets/Textures/spr_jonathan");
            _tileset = Content.Load<Tileset>("Assets/Tilesets/TilesetMario");
            _font = Content.Load<BitmapFont>("Assets/Fonts/Pixeloid");
            _backgroundPattern = Content.Load<Texture2D>("Assets/Textures/background-pattern");

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 movement = Vector2.Zero;

            if (Core.Input.Keyboard.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
            }
            else if (Core.Input.Keyboard.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
            }

            if (Core.Input.Keyboard.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
            }
            else if (Core.Input.Keyboard.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
            }

            if (!_entity.CollidesWith(_collision, movement))
            {
                _entity.Position += movement;
            }

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Space))
            {
                // Core.Audio.PlaySoundEffect(_soundEffect);

                if (_tween != null && !_tween.IsComplete)
                {
                    _tween.Kill();
                }

                // Core.Tweens.TweenColor(_entity, Color.White, Color.Red, TimeSpan.FromSeconds(2));//, TweenEasing.Linear, TweenLoopType.PingPong);
                //_tween = Core.Tweens.TweenScale(_entity, Vector2.One, Vector2.One * 2, TimeSpan.FromSeconds(2));//, TweenEasing.BounceOut);//, TweenLoopType.PingPong);
                //_tween = Core.Tweens.TweenPosition(_entity, _entity.Position, _entity.Position + new Vector2(100, 0), TimeSpan.FromSeconds(1));
                _tween = Core.Tweens.TweenRotation(_entity, 0, MathHelper.ToRadians(360), TimeSpan.FromSeconds(1));//, TweenLoopType.PingPong);
            }

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            {
                // Core.Coroutines.StartCoroutine(TestCoroutine());
                Application.ChangeScene(new SceneShadow());
            }

            _tilemap.Update(gameTime);
            _entity.Update(gameTime);

            _bitmapText.Update(gameTime);

            // Update the offsets for the background pattern wrapping so that it
            // scrolls down and to the right.
            float offset = _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _backgroundOffset.X -= offset;
            _backgroundOffset.Y -= offset;

            // Ensure that the offsets do not go beyond the texture bounds so it is
            // a seamless wrap.
            _backgroundOffset.X %= _backgroundPattern.Width;
            _backgroundOffset.Y %= _backgroundPattern.Height;

            _particleEmitter.Position = _entity.Position;
            _particleEmitter.Update(gameTime);

            base.Update(gameTime);
        }

        public override void DrawWorld(SpriteBatch spriteBatch)
        {
            // Draw the background pattern first using the PointWrap sampler state.
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);
            spriteBatch.Draw(_backgroundPattern, _backgroundDestination, new Rectangle(_backgroundOffset.ToPoint(), _backgroundDestination.Size), Color.White * 0.5f);
            spriteBatch.End();

            spriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                transformMatrix: WorldCamera.TransformMatrix * _scalingMatrix
            );

            _tilemap.Draw(spriteBatch);
            _entity.Draw(spriteBatch);

            _particleEmitter.Draw(spriteBatch);

            _collision.Draw(spriteBatch);

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
            Application.Coroutines.Clear();
            Application.Tweens.Clear();

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

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Core
{
    public class Application : Game
    {
        private static Application _instance;

        private static GraphicsDeviceManager _graphics;
        private static GraphicsDevice _graphicsDevice;
        private static SpriteBatch _spriteBatch;
        private static ContentManager _content;

        private static Scene _currentScene;
        private static Scene _nextScene;

        public static Application Instance => _instance;

        public static GraphicsDeviceManager Graphics => _graphics;
        public static new GraphicsDevice GraphicsDevice => _graphicsDevice;
        public static SpriteBatch SpriteBatch => _spriteBatch;
        public static new ContentManager Content => _content;

        public static Scene CurrentScene => _currentScene;
        public static Scene NextScene => _nextScene;

        public Application()
        {
            _instance = this;

            _graphics = new GraphicsDeviceManager(this);
            ConfigureGraphics();

            _content = base.Content;

            _content.RootDirectory = "Content";
            IsMouseVisible = true;

            _currentScene = null;
            _nextScene = null;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Set the core's graphics device to a reference of the base Game's
            // graphics device.
            _graphicsDevice = base.GraphicsDevice;

            // Create the sprite batch instance.
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (_nextScene != null)
                TransitionScene();

            _currentScene?.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _currentScene?.Draw(gameTime);

            base.Draw(gameTime);
        }

        private void ConfigureGraphics()
        {
            _graphics.PreferredBackBufferWidth = EngineConfig.WindowWidth;
            _graphics.PreferredBackBufferHeight = EngineConfig.WindowHeight;
            _graphics.SynchronizeWithVerticalRetrace = EngineConfig.VSync;

            IsFixedTimeStep = EngineConfig.IsFixedTimeStep;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / EngineConfig.TargetFPS);
        }

        public static void ChangeScene(Scene next)
        {
            // Only set the next scene value if it is not the same
            // instance as the currently active scene.
            if (_nextScene != next)
                _nextScene = next;
        }

        private static void TransitionScene()
        {
            // If there is an active scene, dispose of it.
            _currentScene?.Dispose();

            // Force the garbage collector to collect to ensure memory is cleared.
            GC.Collect();

            // Change the currently active scene to the new scene.
            _currentScene = _nextScene;

            // Null out the next scene value so it does not trigger a change over and over.
            _nextScene = null;

            // If the active scene now is not null, initialize it.
            // Remember, just like with Game, the Initialize call also calls the
            // Scene.LoadContent
            _currentScene?.Initialize();
        }
    }
}

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Collections;
using FreedomEngine.Collections.Coroutines;
using FreedomEngine.Audio;
using FreedomEngine.Input;
using FreedomEngine.UI;

namespace FreedomEngine.Core
{
    /// <summary>
    /// Core engine class that sits atop the main framework game loop, handling
    /// the global state properties, inputs, and active scenes over execution time.
    /// </summary>
    public class Application : Game
    {
        #region Properties

        /// <summary>
        /// Gets the singleton instance of the executing application core.
        /// </summary>
        public static Application Instance { get; private set; }

        /// <summary>
        /// Gets the principal graphics device settings manager of the engine.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Overrides the framework-supplied GraphicsDevice cache globally.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets the core SpriteBatch utilized dynamically within the application structure.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Retrieves the root system ContentManager bound globally.
        /// </summary>
        public static new ContentManager Content { get; private set; }

        /// <summary>
        /// Indicates if hitting the escape key terminates the application execution instance.
        /// </summary>
        public static bool ExitOnEscape { get; private set; }

        /// <summary>
        /// Gets the input handler globally orchestrating physical events mapped into the engine.
        /// </summary>
        public static InputManager Input { get; private set; }

        /// <summary>
        /// Gets a reference to the audio control system.
        /// </summary>
        public static AudioController Audio { get; private set; }

        /// <summary>
        /// Gets the global coroutine controller responsible for managing all active coroutines across scenes.
        /// </summary>
        public static CoroutineController Coroutines { get; private set; }

        /// <summary>
        /// Gets the global manager for handling tween animations.
        /// </summary>
        public static TweenManager Tweens { get; private set; }

        /// <summary>
        /// Gets the currently executing scene.
        /// </summary>
        public static Scene CurrentScene { get; private set; }

        /// <summary>
        /// Gets the upcoming pending Scene transition request.
        /// </summary>
        public static Scene NextScene { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Application"/> class, initializing core properties and configurations.
        /// </summary>
        public Application()
        {
            Instance = this;

            Graphics = new GraphicsDeviceManager(this);
            ConfigureGraphics();

            Content = base.Content;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            ExitOnEscape = true;
            CurrentScene = null;
            NextScene = null;
        }

        #endregion

        #region Lifecycle Methods

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDevice = base.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Input = new InputManager();
            Audio = new AudioController();
            Coroutines = new CoroutineController();
            Tweens = new TweenManager();
        }

        /// <inheritdoc/>
        protected override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Audio.Update();
            Coroutines.Update(gameTime);
            Tweens.Update(gameTime);

            if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
                Exit();

            if (NextScene != null)
                TransitionScene();

            CurrentScene?.Update(gameTime);

            base.Update(gameTime);
        }

        /// <inheritdoc/>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            CurrentScene?.Draw(SpriteBatch);
            base.Draw(gameTime);
        }

        /// <inheritdoc/>
        protected override void UnloadContent()
        {
            Audio.Dispose();
            base.UnloadContent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepares the engine for a level/scene switching over upon hitting the next consecutive logic tick.
        /// </summary>
        public static void ChangeScene(Scene next)
        {
            if (NextScene != next)
                NextScene = next;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates core graphics definitions bound to screen size constraints from settings.
        /// </summary>
        private void ConfigureGraphics()
        {
            Graphics.PreferredBackBufferWidth = EngineConfig.WindowWidth;
            Graphics.PreferredBackBufferHeight = EngineConfig.WindowHeight;
            Graphics.SynchronizeWithVerticalRetrace = EngineConfig.VSync;

            IsFixedTimeStep = EngineConfig.IsFixedTimeStep;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / EngineConfig.TargetFPS);
        }

        /// <summary>
        /// Purges references of any previous active scene safely ensuring proper memory clearance
        /// then assigns initialization bindings internally applying new runtime environments.
        /// </summary>
        private static void TransitionScene()
        {
            CurrentScene?.Dispose();

            // GC.Collect(); // Recommended: Avoid manual GC calls unless profiling proves it necessary.

            CurrentScene = NextScene;
            NextScene = null;
            CurrentScene?.Initialize();
        }

        #endregion
    }
}
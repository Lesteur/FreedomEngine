using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Audio;
using FreedomEngine.Collections;
using FreedomEngine.Collections.Coroutines;
using FreedomEngine.Collections.Tweens;
using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Input;

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
        /// Gets the singleton instance of the running application.
        /// </summary>
        public static Application Instance { get; private set; }

        /// <summary>
        /// Gets the graphics device manager that controls the presentation parameters and back buffer.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Gets the graphics device used for rendering.
        /// </summary>
        /// <remarks>
        /// This hides the instance-level <see cref="Game.GraphicsDevice"/> with a static copy, cached
        /// once available, for convenient engine-wide access.
        /// </remarks>
        public static new GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets the shared <c>SpriteBatch</c> used for 2D rendering throughout the engine.
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }

        /// <summary>
        /// Gets the root content manager.
        /// </summary>
        /// <remarks>
        /// This hides the instance-level <see cref="Game.Content"/> with a static copy, cached once
        /// available, for convenient engine-wide access.
        /// </remarks>
        public static new ContentManager Content { get; private set; }

        /// <summary>
        /// Gets or sets whether pressing the Escape key exits the application.
        /// </summary>
        public static bool ExitOnEscape { get; set; }

        /// <summary>
        /// Gets the input manager that reads and exposes physical input state each frame.
        /// </summary>
        public static InputManager Input { get; private set; }

        /// <summary>
        /// Gets the audio manager used to play and control music and sound effects.
        /// </summary>
        public static AudioManager Audio { get; private set; }

        /// <summary>
        /// Gets the scene that is currently active and receiving updates and draw calls.
        /// </summary>
        public static Scene CurrentScene { get; private set; }

        /// <summary>
        /// Gets the scene queued to become active at the start of the next update, or
        /// <see langword="null"/> if no transition is pending.
        /// </summary>
        public static Scene NextScene { get; private set; }

        /// <summary>
        /// Gets a single white 1x1 pixel texture, useful for drawing primitives and debugging.
        /// </summary>
        public static Texture2D PixelTexture { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Application"/> class, initializing core properties and configurations.
        /// </summary>
        /// <exception cref="InvalidOperationException">An <see cref="Application"/> instance already exists.</exception>
        public Application()
        {
            if (Instance != null)
                throw new InvalidOperationException($"Only one instance of {nameof(Application)} may exist at a time.");

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
            Audio = new AudioManager();

            PixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            PixelTexture.SetData([Color.White]);
        }

        /// <inheritdoc/>
        protected override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);
            Audio.Update(gameTime);

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
            PixelTexture.Dispose();
            Audio.Dispose();

            base.UnloadContent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Queues a scene to become active at the start of the next update, replacing the current scene.
        /// </summary>
        /// <param name="next">The scene to transition to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="next"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The transition is deferred to the start of the next <see cref="Update"/> call, so the
        /// current scene keeps receiving updates and draw calls until then. The scene-scoped static
        /// controllers (<see cref="GameObject.Scene"/>, <see cref="Coroutine.Controller"/>,
        /// <see cref="Tween.Controller"/>, and <see cref="CollisionMask.Controller"/>) are rebound to
        /// <paramref name="next"/> only when the transition actually happens, not when this method is
        /// called.
        /// </remarks>
        public static void ChangeScene(Scene next)
        {
            ArgumentNullException.ThrowIfNull(next);

            NextScene = next;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Applies the window, back buffer, and timing settings from <see cref="EngineConfig"/> to the
        /// graphics device manager and the game loop.
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
        /// Disposes the current scene (if any), makes <see cref="NextScene"/> the current scene,
        /// rebinds the scene-scoped static controllers to it, and initializes it.
        /// </summary>
        private static void TransitionScene()
        {
            CurrentScene?.Dispose();

            CurrentScene = NextScene;
            NextScene = null;

            GameObject.Scene = CurrentScene;
            Coroutine.Controller = CurrentScene?.Coroutines;
            Tween.Controller = CurrentScene?.Tweens;
            CollisionMask.Controller = CurrentScene?.Collisions;

            // Note: avoid calling GC.Collect() here unless profiling proves it necessary.

            CurrentScene?.Initialize();
        }

        #endregion
    }
}
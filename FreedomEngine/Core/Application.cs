using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FreedomEngine.Input;

namespace FreedomEngine.Core
{
    /// <summary>
    /// Core engine class that sits atop the main framework game loop, handling the global state 
    /// properties, inputs, and active scenes over execution time.
    /// </summary>
    public class Application : Game
    {
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
        /// Gets the input handler globally orchestrating physical events mapped into the engine.
        /// </summary>
        public static InputManager Input { get; private set; }

        /// <summary>
        /// Indicates if hitting the escape key terminates the application execution instance.
        /// </summary>
        public static bool ExitOnEscape { get; private set; }

        /// <summary>
        /// Gets the currently executing Scene.
        /// </summary>
        public static Scene CurrentScene { get; private set; }

        /// <summary>
        /// Gets the upcoming pending Scene transition request.
        /// </summary>
        public static Scene NextScene { get; private set; }

        /// <summary>
        /// Initializes a new instance of the global application setup, pre-configuring critical 
        /// components like the GraphicsDeviceManager to base states.
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
        /// Bootstraps custom internal components initializing rendering configurations 
        /// right before beginning main loops.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Set the core's graphics device to a reference of the base Game's graphics device.
            GraphicsDevice = base.GraphicsDevice;

            // Create the sprite batch instance.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a new input manager.
            Input = new InputManager();
        }

        /// <summary>
        /// Automatically disposes cached memory properties internally mapping rendering loops.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Main application execution ticker evaluating controls, managing 
        /// requested transitions, and piping frames logically to active scenes.
        /// </summary>
        /// <param name="gameTime">Contains metrics about global running time scales.</param>
        protected override void Update(GameTime gameTime)
        {
            Input.Update(gameTime);

            if (ExitOnEscape && Input.Keyboard.WasKeyJustPressed(Keys.Escape))
                Exit();

            if (NextScene != null)
                TransitionScene();

            CurrentScene?.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// Paints the background environment invoking specialized layer calls 
        /// into the active scene environment natively.
        /// </summary>
        /// <param name="gameTime">Contains metrics reflecting timeframes tied to execution intervals.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            CurrentScene?.Draw(gameTime);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Prepares the engine for a level/scene switching over upon hitting the next consecutive logic tick.
        /// </summary>
        /// <param name="next">The incoming Scene instance ready to be switched dynamically over the execution state.</param>
        public static void ChangeScene(Scene next)
        {
            // Only set the next scene value if it is not the same
            // instance as the currently active scene.
            if (NextScene != next)
                NextScene = next;
        }

        /// <summary>
        /// Purges references of any previous active scene safely ensuring proper memory clearance
        /// then assigns initialization bindings internally applying new runtime environments.
        /// </summary>
        private static void TransitionScene()
        {
            // If there is an active scene, dispose of it.
            CurrentScene?.Dispose();

            // Force the garbage collector to collect to ensure memory is cleared.
            GC.Collect();

            // Change the currently active scene to the new scene.
            CurrentScene = NextScene;

            // Null out the next scene value so it does not trigger a change over and over.
            NextScene = null;

            // If the active scene now is not null, initialize it.
            // Remember, just like with Game, the Initialize call also calls the
            // Scene.LoadContent
            CurrentScene?.Initialize();
        }
    }
}

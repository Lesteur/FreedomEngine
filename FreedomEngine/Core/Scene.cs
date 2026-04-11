using FreedomEngine.Collections;
using FreedomEngine.Components;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace FreedomEngine.Core
{
    /// <summary>
    /// Represents a base scene or game state within the engine.
    /// Handles updates, rendering of the world and UI, and manages cameras.
    /// </summary>
    public abstract class Scene : IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the width of the scene's world in pixels. This is used to constrain camera movement when following an entity.
        /// </summary>
        protected int _width;

        /// <summary>
        /// Represents the height of the scene's world in pixels. This is used to constrain camera movement when following an entity.
        /// </summary>
        protected int _height;
        
        /// <summary>
        /// Represents the entity that is currently being followed.
        /// </summary>
        protected Entity _following;

        /// <summary>
        /// Pre-calculated scaling matrix to adapt the virtual resolution to the actual window size, maintaining aspect ratio.
        /// </summary>
        protected Matrix _scalingMatrix;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Scene"/> class, initializing content management and camera setup.
        /// Computes base scaling and sets up internal cameras.
        /// </summary>
        public Scene()
        {
            // Create a content manager for the scene
            Content = new(Application.Content.ServiceProvider)
            {
                // Set the root directory for content to the same as the root directory
                // for the game's content.
                RootDirectory = Application.Content.RootDirectory
            };

            // World camera centered at origin
            WorldCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            // UI camera positioned at center of virtual screen
            // This ensures (0, 0) in UI space is at the top-left corner
            UICamera = new Camera(
                EngineConfig.VirtualWidth / 2,
                EngineConfig.VirtualHeight / 2,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            float scaleX = (float)EngineConfig.WindowWidth / EngineConfig.VirtualWidth;
            float scaleY = (float)EngineConfig.WindowHeight / EngineConfig.VirtualHeight;

            // Use the smaller scale to maintain aspect ratio (letterbox mode)
            var scale = MathHelper.Min(scaleX, scaleY);
            _scalingMatrix = Matrix.CreateScale(scale, scale, 1f);

            _width = EngineConfig.VirtualWidth * 2;
            _height = EngineConfig.VirtualHeight * 2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ContentManager used for loading scene-specific assets.
        /// </summary>
        /// <remarks>
        /// Assets loaded through this ContentManager will be automatically unloaded when this scene ends.
        /// </remarks>
        public ContentManager Content { get; protected set; }

        /// <summary>
        /// Gets the world camera for rendering entities and backgrounds.
        /// </summary>
        public Camera WorldCamera { get; protected set; }

        /// <summary>
        /// Gets the UI camera for rendering menus, HUD elements.
        /// </summary>
        public Camera UICamera { get; protected set; }

        /// <summary>
        /// Gets a value that indicates if the scene has been disposed of.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the scene.
        /// </summary>
        /// <remarks>
        /// When overriding this in a derived class, ensure that base.Initialize()
        /// still called as this is when LoadContent is called.
        /// </remarks>
        public virtual void Initialize()
        {
            LoadContent();
        }

        /// <summary>
        /// Override to provide logic to load content for the scene.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Updates this scene.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (_following != null)
            {
                float halfViewportWidth = WorldCamera.ViewportWidth / 2f;
                float halfViewportHeight = WorldCamera.ViewportHeight / 2f;

                var x = Math.Clamp(_following.X, halfViewportWidth, _width - halfViewportWidth);
                var y = Math.Clamp(_following.Y, halfViewportHeight, _height - halfViewportHeight);
                WorldCamera.Position = new Vector2(x, y);
            }
        }

        /// <summary>
        /// Draws this scene.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        public virtual void Draw(GameTime gameTime)
        {
            DrawWorld(gameTime);

            DrawUI(gameTime);
        }

        /// <summary>
        /// Unloads scene-specific content.
        /// </summary>
        public virtual void UnloadContent()
        {
            Content.Unload();
        }

        #endregion

        #region Protected Virtual Methods (API for Subclasses)

        /// <summary>
        /// Override this to render all components composing your background and world entities.
        /// </summary>
        /// <param name="gameTime">Rendering time context.</param>
        public virtual void DrawWorld(GameTime gameTime)
        {
        }

        /// <summary>
        /// Override this to render all canvas screens, HUD data, or purely screen-based UI coordinates.
        /// </summary>
        /// <param name="gameTime">Rendering time context.</param>
        public virtual void DrawUI(GameTime gameTime)
        {
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this scene.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of this scene.
        /// </summary>
        /// <param name="disposing">'
        /// Indicates whether managed resources should be disposed. This value is only true when called from the main
        /// Dispose method. When called from the finalizer, this will be false.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                UnloadContent();
                Content.Dispose();
            }

            IsDisposed = true;
        }

        #endregion
    }
}
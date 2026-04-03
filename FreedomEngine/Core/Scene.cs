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
        private Matrix _scalingMatrix;

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
        /// Gets the content manager specifically associated with this scene.
        /// </summary>
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
        /// Indicates whether this scene instance has been disposed.
        /// </summary>
        public bool IsDisposed { get; protected set; }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the scene's core resources.
        /// </summary>
        public virtual void Initialize()
        {
            LoadContent();
        }

        /// <summary>
        /// Called to load all assets and elements required by the scene.
        /// </summary>
        public virtual void LoadContent()
        {
        }

        /// <summary>
        /// Updates logic, positional updates of the world camera, moving entities, etc.
        /// </summary>
        /// <param name="gameTime">A snapshot of the current engine timing values.</param>
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
        /// Core rendering loop. First draws the world via WorldCamera, then the UI via UICamera.
        /// </summary>
        /// <param name="gameTime">Time passed since the last call to Draw.</param>
        public virtual void Draw(GameTime gameTime)
        {
            Application.SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: WorldCamera.TransformMatrix * _scalingMatrix
            );

            DrawWorld(gameTime);

            Application.SpriteBatch.End();

            Application.SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: UICamera.TransformMatrix * _scalingMatrix
            );

            DrawUI(gameTime);

            Application.SpriteBatch.End();
        }

        /// <summary>
        /// Called to unload all specific resources for this scene.
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
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of the Dispose pattern.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources.</param>
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
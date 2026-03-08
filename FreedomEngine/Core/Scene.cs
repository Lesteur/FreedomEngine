using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Core
{
    public abstract class Scene : IDisposable
    {
        protected ContentManager _content;

        protected bool _disposed;

        protected Camera _worldCamera;

        protected Camera _uiCamera;

        private Matrix _scalingMatrix;


        public ContentManager Content => _content;

        public bool IsDisposed => _disposed;

        public Camera WorldCamera => _worldCamera;

        public Camera UICamera => _uiCamera;


        public Scene()
        {
            // Create a content manager for the scene
            _content = new ContentManager(Application.Content.ServiceProvider);

            // Set the root directory for content to the same as the root directory
            // for the game's content.
            Content.RootDirectory = Application.Content.RootDirectory;


            // World camera centered at origin
            _worldCamera = new Camera(
                0,
                0,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            // UI camera positioned at center of virtual screen
            // This ensures (0, 0) in UI space is at the top-left corner
            _uiCamera = new Camera(
                EngineConfig.VirtualWidth / 2,
                EngineConfig.VirtualHeight / 2,
                EngineConfig.VirtualWidth,
                EngineConfig.VirtualHeight
            );

            float scaleX = (float) EngineConfig.WindowWidth / EngineConfig.VirtualWidth;
            float scaleY = (float) EngineConfig.WindowHeight / EngineConfig.VirtualHeight;

            // Use the smaller scale to maintain aspect ratio (letterbox mode)
            float scale = MathHelper.Min(scaleX, scaleY);

            _scalingMatrix = Matrix.CreateScale(scale, scale, 1f);
        }


        public virtual void Initialize()
        {
            LoadContent();
        }

        public virtual void LoadContent()
        {
        }

        public virtual void UnloadContent()
        {
            Content.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime)
        {
            Application.SpriteBatch.Begin(
                sortMode: SpriteSortMode.Deferred,
                blendState: BlendState.AlphaBlend,
                samplerState: SamplerState.PointClamp,
                depthStencilState: null,
                rasterizerState: null,
                effect: null,
                transformMatrix: _worldCamera.TransformMatrix * _scalingMatrix
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
                transformMatrix: _uiCamera.TransformMatrix * _scalingMatrix
            );

            DrawUI(gameTime);

            Application.SpriteBatch.End();
        }

        public virtual void DrawWorld(GameTime gameTime)
        {
        }

        public virtual void DrawUI(GameTime gameTime)
        {
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


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

            _disposed = true;
        }
    }
}

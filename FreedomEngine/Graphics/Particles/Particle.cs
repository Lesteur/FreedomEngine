using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Graphics.Particles
{
    public class Particle : IDraw
    {
        #region Properties

        public Texture2D Texture;

        public Vector2 Position;
        
        public Vector2 Velocity; // Added for movement
        
        public float Orientation;
        
        public float AngularVelocity; // Added for rotation
        
        public Vector2 Scale = Vector2.One;
        
        public Color Tint = Color.White;
        
        public TimeSpan Duration;
        
        public float PercentLife = 1f;

        #endregion

        #region Constructors

        public Particle(Texture2D texture)
        {
            Texture = texture;
        }

        #endregion

        #region Lifecycle Methods

        // Added this method to help re-initialize the particle from the pool
        //public virtual void Initialize(Vector2 position, Vector2 velocity, float orientation, float angularVelocity, Color tint, TimeSpan duration, Vector2 scale)
        public virtual void Initialize(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            Orientation = 0f;
            AngularVelocity = 0f;
            Tint = Color.White;
            Duration = TimeSpan.FromSeconds(1);
            Scale = Vector2.One;
            PercentLife = 1f;
        }

        public virtual void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply movement and rotation
            Position += Velocity * dt;
            Orientation += AngularVelocity * dt;

            // Reduce lifespan
            if (Duration.TotalSeconds > 0)
            {
                PercentLife -= dt / (float)Duration.TotalSeconds;
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null || PercentLife <= 0f)
                return;

            // Center the texture on the position
            Vector2 origin = new(Texture.Width / 2f, Texture.Height / 2f);

            // Fade out particle based on its remaining life
            Color colorToDraw = Tint * PercentLife;

            spriteBatch.Draw(Texture, Position, null, colorToDraw, Orientation, origin, Scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
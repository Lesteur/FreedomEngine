using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections;

namespace FreedomEngine.Particles
{
    public class ParticleDefault : Particle
    {
        #region Fields

        private static readonly Random _rand = new();

        #endregion

        #region Constructors

        public ParticleDefault(Texture2D texture) : base(texture)
        {
            // Default constructor for serialization and pooling purposes.
        }

        #endregion

        #region Lifecycle Methods

        public override void Initialize(Vector2 position)
        {
            var angle = (float)(_rand.NextDouble() * Math.PI * 2);
            var _time = _rand.NextInt64(500, 1500); // Random duration between 0.5 and 1.5 seconds
            var _force = (float)(_rand.NextDouble() * 10f + 10f);

            Position = position;

            Velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * _force;
            Orientation = angle;
            AngularVelocity = 0f;
            Tint = Color.Red;
            Duration = TimeSpan.FromMilliseconds(_time);
            Scale = Vector2.One * 4;
            PercentLife = 2f;

            //Logger.Info($"Initialized particle at position {Position} with velocity {Velocity}, orientation {Orientation}, and duration {Duration.TotalSeconds} seconds");
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Move the particle based on its velocity
            Velocity += new Vector2(0, 20f) * deltaTime; // Apply gravity

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null || PercentLife <= 0f)
                return;

            // Center the texture on the position
            Vector2 origin = new(Texture.Width / 2f, Texture.Height / 2f);

            // Fade out particle based on its remaining life
            Color colorToDraw = Color.Lerp(Tint, Color.Yellow, PercentLife) * PercentLife;

            spriteBatch.Draw(Texture, Position, null, colorToDraw, Orientation, origin, Scale, SpriteEffects.None, 0f);
        }

        #endregion
    }
}
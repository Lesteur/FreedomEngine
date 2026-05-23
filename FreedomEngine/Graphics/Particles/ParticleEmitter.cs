using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections;
using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Graphics.Particles
{
    public class ParticleEmitter<T> : IDraw where T : Particle
    {
        #region Fields

        private readonly T[] _particles;

        private int _activeCount;

        private TimeSpan _elapsed;

        private readonly Func<Texture2D, T> _particleFactory;

        #endregion

        #region Properties

        public Vector2 Position { get; set; }

        public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(0.2);

        public bool Looping { get; set; } = true;

        public int ParticlesPerEmission { get; set; } = 10;

        // Exposing the number of active particles for debugging purposes
        public int ActiveParticlesCount => _activeCount;

        public int Capacity => _particles.Length;

        public bool IsEmitting => _activeCount > 0;

        #endregion

        #region Constructors

        public ParticleEmitter(Texture2D texture, int capacity, Vector2 position, Func<Texture2D, T> particleFactory)
        {
            _particles = new T[capacity];
            Position = position;
            _activeCount = 0;
            _elapsed = TimeSpan.Zero;
            _particleFactory = particleFactory;

            // Populate the array with dead particle objects, for pooling.
            for (int i = 0; i < capacity; i++)
            {
                var particle = _particleFactory(texture);
                particle.PercentLife = -1f; // Force particle to be considered dead
                _particles[i] = particle;
            }
        }

        #endregion

        #region Lifecycle Methods

        public void Update(GameTime gameTime)
        {
            _elapsed += gameTime.ElapsedGameTime;

            for (int i = 0; i < _activeCount; i++)
            {
                var particle = _particles[i];

                particle.Update(gameTime);

                // If the particle is dead, swap it with the last active one
                if (particle.PercentLife <= 0)
                {
                    _particles[i] = _particles[_activeCount - 1];
                    _particles[_activeCount - 1] = particle;

                    _activeCount--;
                    i--; // Decrement i to evaluate the newly swapped particle
                }
            }

            if (Looping && _elapsed >= Interval)
            {
                Emit(ParticlesPerEmission);
                _elapsed -= Interval;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < _activeCount; i++)
            {
                _particles[i].Draw(spriteBatch);
            }
        }

        #endregion

        #region Public Methods

        public void Emit(int number)
        {
            if (number <= 0)
                throw new ArgumentException("Number of particles to emit must be greater than zero.", nameof(number));

            if (_activeCount + number > _particles.Length)
                number = _particles.Length - _activeCount; // Emit only as many as we have capacity for

            for (int i = 0; i < number; i++)
            {
                var particle = _particles[_activeCount + i];
                particle.Initialize(Position);
            }

            _activeCount += number;
        }

        public void Clear()
        {
            // Instantly kill all particles by resetting the active counter
            _activeCount = 0;
        }

        #endregion
    }
}
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Provides the base functionality for an entity that renders a <see cref="Graphics.Sprite"/> and
    /// advances its animation over time.
    /// </summary>
    public abstract class DrawableEntity : IDraw
    {
        #region Fields

        /// <summary>
        /// Represents the elapsed time since the last frame update, used for animation timing.
        /// </summary>
        protected TimeSpan _elapsed = TimeSpan.Zero;

        /// <summary>
        /// The sprite rendered by this entity. May be <see langword="null"/>, in which case the entity
        /// neither animates nor draws.
        /// </summary>
        protected Sprite _sprite;

        /// <summary>
        /// The index of the frame of <see cref="_sprite"/> currently being displayed.
        /// </summary>
        private int _currentFrame;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the sprite rendered by this entity.
        /// </summary>
        /// <remarks>
        /// Assigning a different sprite resets <see cref="CurrentFrame"/> and the accumulated animation
        /// time. Assigning the sprite that is already set has no effect, so the current animation is
        /// not interrupted.
        /// </remarks>
        public virtual Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (ReferenceEquals(_sprite, value))
                    return;

                _sprite = value;
                _currentFrame = 0;
                _elapsed = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets or sets the color mask to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Color.White
        /// </remarks>
        public virtual Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the amount of rotation, in radians, to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public virtual float Rotation { get; set; } = 0.0f;

        /// <summary>
        /// Gets or sets the scale factor to apply to the x- and y-axes when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Vector2.One
        /// </remarks>
        public virtual Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Gets or sets the sprite effects to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is SpriteEffects.None
        /// </remarks>
        public virtual SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Gets or sets the layer depth to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public virtual float LayerDepth { get; set; } = 0.0f;

        /// <summary>
        /// Gets or sets a value indicating whether the entity should be drawn.
        /// </summary>
        /// <remarks>
        /// Default value is true
        /// </remarks>
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Gets the width, in pixels, of this entity as currently rendered, or 0 if no sprite is set.
        /// </summary>
        /// <remarks>
        /// Width is calculated by multiplying the width of the source texture region by the x-axis scale factor.
        /// </remarks>
        public virtual float Width => _sprite == null ? 0f : _sprite.Frames[_currentFrame].Width * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, of this entity as currently rendered, or 0 if no sprite is set.
        /// </summary>
        /// <remarks>
        /// Height is calculated by multiplying the height of the source texture region by the y-axis scale factor.
        /// </remarks>
        public virtual float Height => _sprite == null ? 0f : _sprite.Frames[_currentFrame].Height * Scale.Y;

        /// <summary>
        /// Gets or sets the index of the animation frame currently being displayed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value is negative, or is not less than the frame count of <see cref="Sprite"/>.
        /// </exception>
        public virtual int CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Frame index cannot be negative.");

                if (_sprite != null && value >= _sprite.Length)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Frame index must be less than the sprite frame count ({_sprite.Length}).");

                _currentFrame = value;
            }
        }

        /// <summary>
        /// Gets or sets the position of the entity in 2D space.
        /// </summary>
        public virtual Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate of the entity's position.
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the entity's position.
        /// </summary>
        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="DrawableEntity"/> class with the specified sprite and initial position.
        /// </summary>
        /// <param name="sprite">
        /// The sprite associated with the entity. May be <see langword="null"/> for an entity that
        /// does not render until a sprite is assigned.
        /// </param>
        /// <param name="position">The initial position of the entity in 2D space.</param>
        protected DrawableEntity(Sprite sprite, Vector2 position)
        {
            // Assign the backing fields directly: the Sprite and Position properties are virtual, and
            // calling a virtual member from a constructor would run a derived override before the
            // derived type has finished initializing.
            _sprite = sprite;
            _currentFrame = 0;

            Position = position;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the entity's state and advances its sprite animation.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public virtual void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            if (_sprite == null)
                return;

            _elapsed += gameTime.ElapsedGameTime;

            _currentFrame = _sprite.GetNextFrame(_currentFrame, _elapsed, out TimeSpan newElapsedTime);
            _elapsed = newElapsedTime;
        }

        /// <summary>
        /// Draws the entity using the given sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        /// <remarks>Nothing is drawn when <see cref="Visible"/> is false or no sprite is set.</remarks>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            if (!Visible || _sprite == null)
                return;

            spriteBatch.Draw(_sprite.Texture, Position, _sprite.Frames[_currentFrame], Color, Rotation, _sprite.Origin, Scale, Effects, LayerDepth);
        }

        #endregion
    }
}
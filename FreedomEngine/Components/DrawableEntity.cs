using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Components
{
    public abstract class DrawableEntity : IDraw
    {
        #region Fields

        /// <summary>
        /// Represents the elapsed time since the last frame update, used for animation timing.
        /// </summary>
        protected TimeSpan _elapsed = TimeSpan.Zero;

        protected Sprite _sprite;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the sprite of the entity.
        /// </summary>
        public virtual Sprite Sprite
        {
            get => _sprite;
            set
            {
                if (Sprite != null || Sprite != value)
                {
                    _sprite = value;
                    CurrentFrame = 0;
                }
            }
        }

        /// <summary>
        /// Gets or Sets the color mask to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Color.White
        /// </remarks>
        public virtual Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or Sets the amount of rotation, in radians, to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public virtual float Rotation { get; set; } = 0.0f;

        /// <summary>
        /// Gets or Sets the scale factor to apply to the x- and y-axes when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Vector2.One
        /// </remarks>
        public virtual Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Gets or Sets the xy-coordinate origin point, relative to the top-left corner, of this entity when rendering.
        /// </summary>
        /// <remarks>
        /// Default value is Vector2.Zero
        /// </remarks>
        public virtual Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets or Sets the sprite effects to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is SpriteEffects.None
        /// </remarks>
        public virtual SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Gets or Sets the layer depth to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public virtual float LayerDepth { get; set; } = 0.0f;

        /// <summary>
        /// Gets or Sets a value indicating whether the entity should be drawn.
        /// </summary>
        /// <remarks>
        /// Default value is true
        /// </remarks>
        public virtual bool Visible { get; set; } = true;

        /// <summary>
        /// Gets the width, in pixels, of this sprite.
        /// </summary>
        /// <remarks>
        /// Width is calculated by multiplying the width of the source texture region by the x-axis scale factor.
        /// </remarks>
        public virtual float Width => Sprite.Animation.Frames[CurrentFrame].Width * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, of this sprite.
        /// </summary>
        /// <remarks>
        /// Height is calculated by multiplying the height of the source texture region by the y-axis scale factor.
        /// </remarks>
        public virtual float Height => Sprite.Animation.Frames[CurrentFrame].Height * Scale.Y;

        /// <summary>
        /// Gets or Sets the current animation frame index.
        /// </summary>
        public virtual int CurrentFrame { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the X position of the entity.
        /// </summary>
        public virtual Vector2 Position { get; set; }

        /// <summary>
        /// Gets or Sets the X position of the entity.
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        /// <summary>
        /// Gets or Sets the Y position of the entity.
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
        /// <param name="sprite">The sprite associated with the entity.</param>
        /// <param name="position">The initial position of the entity in 2D space.</param>
        public DrawableEntity(Sprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the entity's state and animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (Sprite?.Animation == null)
                return;

            var animation = Sprite.Animation;

            CurrentFrame = animation.GetNextFrame(CurrentFrame, _elapsed, out TimeSpan newElapsedTime);
            _elapsed = newElapsedTime;

            _elapsed += gameTime.ElapsedGameTime;
        }

        /// <summary>
        /// Draws the entity using the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The rendering context.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Sprite?.Animation.Frames == null)
                return;

            spriteBatch.Draw(Sprite.Texture, Position, Sprite.Animation.Frames[CurrentFrame], Color, Rotation, Sprite.Origin, Scale, Effects, LayerDepth);
        }

        #endregion
    }
}

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;
using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a base entity in the game.
    /// </summary>
    public class Entity : IDraw
    {
        #region Fields

        /// <summary>
        /// Represents the elapsed time since the last frame update, used for animation timing.
        /// </summary>
        private TimeSpan _elapsed = TimeSpan.Zero;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the sprite of the entity.
        /// </summary>
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// Gets or Sets the color mask to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Color.White
        /// </remarks>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or Sets the amount of rotation, in radians, to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public int Rotation { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the scale factor to apply to the x- and y-axes when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is Vector2.One
        /// </remarks>
        public Vector2 Scale { get; set; } = Vector2.One;

        /// <summary>
        /// Gets or Sets the xy-coordinate origin point, relative to the top-left corner, of this entity when rendering.
        /// </summary>
        /// <remarks>
        /// Default value is Vector2.Zero
        /// </remarks>
        public Vector2 Origin { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets or Sets the sprite effects to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is SpriteEffects.None
        /// </remarks>
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Gets or Sets the layer depth to apply when rendering this entity.
        /// </summary>
        /// <remarks>
        /// Default value is 0.0f
        /// </remarks>
        public float LayerDepth { get; set; } = 0.0f;

        /// <summary>
        /// Gets or Sets a value indicating whether the entity should be drawn.
        /// </summary>
        /// <remarks>
        /// Default value is true
        /// </remarks>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets the width, in pixels, of this sprite.
        /// </summary>
        /// <remarks>
        /// Width is calculated by multiplying the width of the source texture region by the x-axis scale factor.
        /// </remarks>
        //public float Width => Region.Width * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, of this sprite.
        /// </summary>
        /// <remarks>
        /// Height is calculated by multiplying the height of the source texture region by the y-axis scale factor.
        /// </remarks>
        //public float Height => Sprite.Region.Height * Scale.Y;

        /// <summary>
        /// Gets or Sets the current animation frame index.
        /// </summary>
        public int CurrentFrame { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the X position of the entity.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

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
        /// Creates a new instance of the <see cref="Entity"/> class with the specified sprite and initial position.
        /// </summary>
        /// <param name="sprite">The sprite associated with the entity.</param>
        /// <param name="x">The starting X position.</param>
        /// <param name="y">The starting Y position.</param>
        public Entity(Sprite sprite, int x = 0, int y = 0)
        {
            Sprite = sprite;
            X = x;
            Y = y;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the entity's state and animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (Sprite?.Frames == null || Sprite.Frames.Length <= 1 || Sprite.Delay == TimeSpan.Zero)
                return;

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= Sprite.Delay)
            {
                _elapsed -= Sprite.Delay;
                
                // Modulo allows naturally looping back to the start of the animation
                CurrentFrame = (CurrentFrame + 1) % Sprite.Frames.Length;
            }
        }

        /// <summary>
        /// Draws the entity using the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The rendering context.</param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Sprite?.Frames == null || CurrentFrame >= Sprite.Frames.Length)
                return;

            // Factorize recurring calculations
            var origin = new Vector2(Sprite.XOrigin, Sprite.YOrigin);
            var position = new Vector2(X + origin.X, Y + origin.Y);
            var scale = new Vector2(Scale.X, Scale.Y);
            float rotationRadians = MathHelper.ToRadians(Rotation);

            Sprite.Frames[CurrentFrame].Draw(
                spriteBatch,
                position,
                Color,
                rotationRadians,
                origin,
                scale,
                Effects,
                LayerDepth
            );
        }

        #endregion

        #region Public Methods

        public void ChangeSprite(Sprite sprite)
        {
            if (Sprite != null || Sprite != sprite)
            {
                Sprite = sprite;
                CurrentFrame = 0;
            }
        }

        #endregion
    }
}
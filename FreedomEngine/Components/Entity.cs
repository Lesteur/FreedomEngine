using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a base entity in the game.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Represents the elapsed time since the last frame update, used for animation timing.
        /// </summary>
        private TimeSpan _elapsed = TimeSpan.Zero;

        /// <summary>
        /// Gets or Sets the sprite of the entity.
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Gets or Sets the current animation frame index.
        /// </summary>
        public int CurrentFrame { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the X position of the entity.
        /// </summary>
        public int X { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the Y position of the entity.
        /// </summary>
        public int Y { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the rotation of the entity in degrees.
        /// </summary>
        public int Rotation { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the horizontal scale.
        /// </summary>
        public float ScaleX { get; set; } = 1f;

        /// <summary>
        /// Gets or Sets the vertical scale.
        /// </summary>
        public float ScaleY { get; set; } = 1f;

        /// <summary>
        /// Gets or Sets the effects applied to the sprite rendering.
        /// </summary>
        public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;

        /// <summary>
        /// Gets or Sets the draw depth of the entity.
        /// </summary>
        public float LayerDepth { get; set; } = 0f;

        /// <summary>
        /// Gets or Sets the tint color of the sprite.
        /// </summary>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or Sets a value indicating whether the entity should be drawn.
        /// </summary>
        public bool Visible { get; set; } = true;

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

        /// <summary>
        /// Updates the entity's state and animation.
        /// </summary>
        /// <param name="gameTime">The time elapsed since the last update.</param>
        public virtual void Update(GameTime gameTime)
        {
            if (Sprite?.Frames == null || Sprite.Frames.Length == 0 || Sprite.Delay == TimeSpan.Zero)
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
            var scale = new Vector2(ScaleX, ScaleY);
            float rotationRadians = MathHelper.ToRadians(Rotation);

            Sprite.Frames[CurrentFrame].Draw(
                spriteBatch,
                position,
                Color,
                rotationRadians,
                origin,
                scale,
                SpriteEffects,
                LayerDepth
            );
        }
    }
}
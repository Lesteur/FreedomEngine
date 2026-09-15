using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Represents a collision mask shaped as a single point in world space.
    /// </summary>
    public class PointCollision : CollisionMask
    {
        #region Properties

        /// <inheritdoc/>
        public override float BBoxLeft => Position.X;

        /// <inheritdoc/>
        public override float BBoxRight => Position.X;

        /// <inheritdoc/>
        public override float BBoxTop => Position.Y;

        /// <inheritdoc/>
        public override float BBoxBottom => Position.Y;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollision"/> class.
        /// </summary>
        /// <param name="position">The position of the point.</param>
        /// <param name="tag">The tag bits identifying which category or categories this mask belongs to.</param>
        /// <param name="oneWayCollision">The direction from which this mask may be collided with.</param>
        /// <exception cref="InvalidOperationException"><see cref="CollisionMask.Controller"/> has not been assigned.</exception>
        public PointCollision(Vector2 position, uint tag, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws a debug visualization of this point as a small cross.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float worldX = Position.X;
            float worldY = Position.Y;
            const int size = 3;

            // Draw a cross
            DrawDebugLine(spriteBatch, new Vector2(worldX - size, worldY), new Vector2(worldX + size, worldY));
            DrawDebugLine(spriteBatch, new Vector2(worldX, worldY - size), new Vector2(worldX, worldY + size));
        }

        #endregion

        #region Internal Methods

        /// <inheritdoc/>
        /// <remarks>Two points overlap when their coordinates match within a small tolerance.</remarks>
        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            Vector2 a = Position + thisPosition;
            Vector2 b = point.Position;

            return MathF.Abs(a.X - b.X) < Epsilon && MathF.Abs(a.Y - b.Y) < Epsilon;
        }

        /// <inheritdoc/>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            // Delegate to the line's point test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return line.IntersectsPoint(this, -thisPosition);
        }

        /// <inheritdoc/>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            // Delegate to the rectangle's point test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return rectangle.IntersectsPoint(this, -thisPosition);
        }

        /// <inheritdoc/>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            // Delegate to the circle's point test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return circle.IntersectsPoint(this, -thisPosition);
        }

        #endregion
    }
}
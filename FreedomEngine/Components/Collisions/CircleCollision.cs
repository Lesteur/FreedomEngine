using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Represents a collision mask shaped as a circle, positioned by its center.
    /// </summary>
    public class CircleCollision : CollisionMask
    {
        #region Fields

        /// <summary>
        /// The number of line segments used to approximate the circle when drawing it for debugging.
        /// </summary>
        private const int DebugSegments = 32;

        /// <summary>
        /// The radius of the circle, in pixels.
        /// </summary>
        private float _radius;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the radius of the circle, in pixels.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        public float Radius
        {
            get => _radius;
            set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Radius cannot be negative.");

                _radius = value;
            }
        }

        /// <summary>
        /// Gets the diameter of the circle (twice the radius), in pixels.
        /// </summary>
        public float Diameter => Radius * 2f;

        /// <inheritdoc/>
        public override float BBoxLeft => Position.X - Radius;

        /// <inheritdoc/>
        public override float BBoxRight => Position.X + Radius;

        /// <inheritdoc/>
        public override float BBoxTop => Position.Y - Radius;

        /// <inheritdoc/>
        public override float BBoxBottom => Position.Y + Radius;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleCollision"/> class.
        /// </summary>
        /// <param name="position">The position of the circle's center.</param>
        /// <param name="tag">The tag bits identifying which category or categories this mask belongs to.</param>
        /// <param name="radius">The radius of the circle, in pixels.</param>
        /// <param name="oneWayCollision">The direction from which this mask may be collided with.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="radius"/> is negative.</exception>
        /// <exception cref="InvalidOperationException"><see cref="CollisionMask.Controller"/> has not been assigned.</exception>
        public CircleCollision(Vector2 position, uint tag, float radius, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
            Radius = radius;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws a debug visualization of this circle, approximated with line segments.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float centerX = Position.X;
            float centerY = Position.Y;
            float radius = Radius;

            // Draw circle using line segments
            for (int i = 0; i < DebugSegments; i++)
            {
                float angle1 = MathF.Tau * i / DebugSegments;
                float angle2 = MathF.Tau * (i + 1) / DebugSegments;

                float x1 = centerX + radius * MathF.Cos(angle1);
                float y1 = centerY + radius * MathF.Sin(angle1);
                float x2 = centerX + radius * MathF.Cos(angle2);
                float y2 = centerY + radius * MathF.Sin(angle2);

                DrawDebugLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2));
            }
        }

        #endregion

        #region Internal Methods

        /// <inheritdoc/>
        /// <remarks>The test is inclusive of the circle's edge.</remarks>
        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            Vector2 center = Position + thisPosition;

            float dx = point.Position.X - center.X;
            float dy = point.Position.Y - center.Y;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= Radius * Radius;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Finds the point on the segment closest to the circle's center and compares its distance
        /// against the radius.
        /// </remarks>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            Vector2 center = Position + thisPosition;

            Vector2 start = line.Position;
            Vector2 end = line.PositionEnd;

            // Line direction vector
            float ldx = end.X - start.X;
            float ldy = end.Y - start.Y;

            // Line length squared
            float lineLengthSquared = ldx * ldx + ldy * ldy;

            float closestX;
            float closestY;

            if (lineLengthSquared < Epsilon)
            {
                // Degenerate segment: both endpoints coincide, so treat it as a single point.
                closestX = start.X;
                closestY = start.Y;
            }
            else
            {
                // Vector from line start to circle center
                float dx = center.X - start.X;
                float dy = center.Y - start.Y;

                // Find closest point on line to circle center (clamped to segment)
                float t = Math.Clamp((dx * ldx + dy * ldy) / lineLengthSquared, 0f, 1f);

                closestX = start.X + t * ldx;
                closestY = start.Y + t * ldy;
            }

            // Distance from closest point to circle center
            float distX = center.X - closestX;
            float distY = center.Y - closestY;
            float distanceSquared = distX * distX + distY * distY;

            return distanceSquared <= Radius * Radius;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Clamps the circle's center to the rectangle to find the closest point, then compares its
        /// distance against the radius.
        /// </remarks>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            Vector2 center = Position + thisPosition;

            float left = rectangle.Position.X;
            float top = rectangle.Position.Y;
            float right = left + rectangle.Width;
            float bottom = top + rectangle.Height;

            // Find the closest point on the rectangle to the circle center
            float closestX = Math.Clamp(center.X, left, right);
            float closestY = Math.Clamp(center.Y, top, bottom);

            // Calculate distance from circle center to closest point
            float dx = center.X - closestX;
            float dy = center.Y - closestY;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= Radius * Radius;
        }

        /// <inheritdoc/>
        /// <remarks>Two circles overlap when the distance between their centers is at most the sum of their radii.</remarks>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            Vector2 center1 = Position + thisPosition;
            Vector2 center2 = circle.Position;

            float dx = center2.X - center1.X;
            float dy = center2.Y - center1.Y;
            float distanceSquared = dx * dx + dy * dy;

            float radiusSum = Radius + circle.Radius;
            return distanceSquared <= radiusSum * radiusSum;
        }

        #endregion
    }
}
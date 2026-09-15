using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Represents a collision mask shaped as a line segment running between two world positions.
    /// </summary>
    public class LineCollision : CollisionMask
    {
        #region Properties

        /// <summary>
        /// Gets or sets the position at which the line segment ends.
        /// </summary>
        /// <remarks>
        /// This is an endpoint expressed in the same coordinate space as <see cref="CollisionMask.Position"/>,
        /// not a direction or a length offset from it.
        /// </remarks>
        public Vector2 PositionEnd { get; set; }

        /// <inheritdoc/>
        public override float BBoxLeft => MathF.Min(Position.X, PositionEnd.X);

        /// <inheritdoc/>
        public override float BBoxRight => MathF.Max(Position.X, PositionEnd.X);

        /// <inheritdoc/>
        public override float BBoxTop => MathF.Min(Position.Y, PositionEnd.Y);

        /// <inheritdoc/>
        public override float BBoxBottom => MathF.Max(Position.Y, PositionEnd.Y);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineCollision"/> class.
        /// </summary>
        /// <param name="position">The position at which the line segment starts.</param>
        /// <param name="tag">The tag bits identifying which category or categories this mask belongs to.</param>
        /// <param name="positionEnd">The position at which the line segment ends.</param>
        /// <param name="oneWayCollision">The direction from which this mask may be collided with.</param>
        /// <exception cref="InvalidOperationException"><see cref="CollisionMask.Controller"/> has not been assigned.</exception>
        public LineCollision(Vector2 position, uint tag, Vector2 positionEnd, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
            PositionEnd = positionEnd;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws a debug visualization of this line segment.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawDebugLine(spriteBatch, Position, PositionEnd);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Determines whether two line segments intersect.
        /// </summary>
        /// <param name="a1">The position at which the first segment starts.</param>
        /// <param name="a2">The position at which the first segment ends.</param>
        /// <param name="b1">The position at which the second segment starts.</param>
        /// <param name="b2">The position at which the second segment ends.</param>
        /// <returns><see langword="true"/> if the segments cross; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Collinear overlapping segments are reported as not intersecting, since the denominator of
        /// the parametric form degenerates to zero for parallel segments.
        /// </remarks>
        internal static bool SegmentsIntersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
        {
            // Calculate the denominator for the intersection formula
            float denominator = ((a1.X - a2.X) * (b1.Y - b2.Y)) - ((a1.Y - a2.Y) * (b1.X - b2.X));

            // Segments are parallel (or degenerate) if the denominator is zero
            if (MathF.Abs(denominator) < Epsilon)
                return false;

            // Calculate intersection parameters
            float t = (((a1.X - b1.X) * (b1.Y - b2.Y)) - ((a1.Y - b1.Y) * (b1.X - b2.X))) / denominator;
            float u = -(((a1.X - a2.X) * (a1.Y - b1.Y)) - ((a1.Y - a2.Y) * (a1.X - b1.X))) / denominator;

            // Check if the intersection point falls within both segments
            return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// A point is considered on the segment when the sum of its distances to both endpoints
        /// matches the segment length within a small tolerance.
        /// </remarks>
        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            Vector2 start = Position + thisPosition;
            Vector2 end = PositionEnd + thisPosition;
            Vector2 p = point.Position;

            float lineLength = Vector2.Distance(start, end);
            float d1 = Vector2.Distance(p, start);
            float d2 = Vector2.Distance(p, end);

            // Check if sum of distances equals line length (within tolerance)
            return MathF.Abs((d1 + d2) - lineLength) < 0.01f;
        }

        /// <inheritdoc/>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            return SegmentsIntersect(
                Position + thisPosition,
                PositionEnd + thisPosition,
                line.Position,
                line.PositionEnd);
        }

        /// <inheritdoc/>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            // Delegate to the rectangle's line test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return rectangle.IntersectsLine(this, -thisPosition);
        }

        /// <inheritdoc/>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            // Delegate to the circle's line test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return circle.IntersectsLine(this, -thisPosition);
        }

        #endregion
    }
}
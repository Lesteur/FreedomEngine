using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Represents a collision mask shaped as an axis-aligned rectangle, positioned by its top-left corner.
    /// </summary>
    public class RectangleCollision : CollisionMask
    {
        #region Fields

        /// <summary>
        /// The width of the rectangle, in pixels.
        /// </summary>
        private float _width;

        /// <summary>
        /// The height of the rectangle, in pixels.
        /// </summary>
        private float _height;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the width of the rectangle, in pixels, extending right from <see cref="CollisionMask.Position"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        public float Width
        {
            get => _width;
            set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Width cannot be negative.");

                _width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the rectangle, in pixels, extending down from <see cref="CollisionMask.Position"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        public float Height
        {
            get => _height;
            set
            {
                if (value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Height cannot be negative.");

                _height = value;
            }
        }

        /// <inheritdoc/>
        public override float BBoxLeft => Position.X;

        /// <inheritdoc/>
        public override float BBoxRight => Position.X + Width;

        /// <inheritdoc/>
        public override float BBoxTop => Position.Y;

        /// <inheritdoc/>
        public override float BBoxBottom => Position.Y + Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleCollision"/> class.
        /// </summary>
        /// <param name="position">The position of the rectangle's top-left corner.</param>
        /// <param name="tag">The tag bits identifying which category or categories this mask belongs to.</param>
        /// <param name="width">The width of the rectangle, in pixels.</param>
        /// <param name="height">The height of the rectangle, in pixels.</param>
        /// <param name="oneWayCollision">The direction from which this mask may be collided with.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> or <paramref name="height"/> is negative.
        /// </exception>
        /// <exception cref="InvalidOperationException"><see cref="CollisionMask.Controller"/> has not been assigned.</exception>
        public RectangleCollision(Vector2 position, uint tag, float width, float height, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws a debug visualization of this rectangle's four edges.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            float left = Position.X;
            float top = Position.Y;
            float right = left + Width;
            float bottom = top + Height;

            // Draw four edges
            DrawDebugLine(spriteBatch, new Vector2(left, top), new Vector2(right, top));        // Top
            DrawDebugLine(spriteBatch, new Vector2(right, top), new Vector2(right, bottom));    // Right
            DrawDebugLine(spriteBatch, new Vector2(right, bottom), new Vector2(left, bottom));  // Bottom
            DrawDebugLine(spriteBatch, new Vector2(left, bottom), new Vector2(left, top));      // Left
        }

        #endregion

        #region Internal Methods

        /// <inheritdoc/>
        /// <remarks>The test is inclusive of the rectangle's edges.</remarks>
        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            float left = Position.X + thisPosition.X;
            float top = Position.Y + thisPosition.Y;
            float right = left + Width;
            float bottom = top + Height;

            float px = point.Position.X;
            float py = point.Position.Y;

            return px >= left && px <= right && py >= top && py <= bottom;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Reports an overlap when either endpoint of the segment lies inside the rectangle, or when
        /// the segment crosses any of its four edges.
        /// </remarks>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            float left = Position.X + thisPosition.X;
            float top = Position.Y + thisPosition.Y;
            float right = left + Width;
            float bottom = top + Height;

            Vector2 start = line.Position;
            Vector2 end = line.PositionEnd;

            // Check if either endpoint is inside the rectangle
            if ((start.X >= left && start.X <= right && start.Y >= top && start.Y <= bottom) ||
                (end.X >= left && end.X <= right && end.Y >= top && end.Y <= bottom))
                return true;

            var topLeft = new Vector2(left, top);
            var topRight = new Vector2(right, top);
            var bottomLeft = new Vector2(left, bottom);
            var bottomRight = new Vector2(right, bottom);

            // Check intersection with each edge of the rectangle
            return LineCollision.SegmentsIntersect(start, end, topLeft, topRight) ||
                   LineCollision.SegmentsIntersect(start, end, bottomLeft, bottomRight) ||
                   LineCollision.SegmentsIntersect(start, end, topLeft, bottomLeft) ||
                   LineCollision.SegmentsIntersect(start, end, topRight, bottomRight);
        }

        /// <inheritdoc/>
        /// <remarks>Uses an axis-aligned bounding box (AABB) overlap test.</remarks>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            float left1 = Position.X + thisPosition.X;
            float top1 = Position.Y + thisPosition.Y;
            float right1 = left1 + Width;
            float bottom1 = top1 + Height;

            float left2 = rectangle.Position.X;
            float top2 = rectangle.Position.Y;
            float right2 = left2 + rectangle.Width;
            float bottom2 = top2 + rectangle.Height;

            // AABB collision detection
            return !(right1 <= left2 || left1 >= right2 || bottom1 <= top2 || top1 >= bottom2);
        }

        /// <inheritdoc/>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            // Delegate to the circle's rectangle test. The roles of the two shapes are swapped, so the
            // offset must be negated to keep their relative displacement identical.
            return circle.IntersectsRectangle(this, -thisPosition);
        }

        #endregion
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Drawing;

namespace FreedomEngine.Components.Collisions
{
    public class RectangleCollision : CollisionMask
    {
        #region Properties

        public float Width { get; set; }

        public float Height { get; set; }

        #endregion

        #region Constructors

        public RectangleCollision(Vector2 position, float width, float height) : base(position)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Public Methods

        public override bool Intersects(CollisionMask other, Vector2 thisPosition, Vector2 otherPosition)
        {
            return other switch
            {
                PointCollision point            => IntersectsPoint(point, thisPosition, otherPosition),
                LineCollision line              => IntersectsLine(line, thisPosition, otherPosition),
                RectangleCollision rectangle    => IntersectsRectangle(rectangle, thisPosition, otherPosition),
                CircleCollision circle          => IntersectsCircle(circle, thisPosition, otherPosition),
                _ => false
            };
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            float left = position.X + Position.X;
            float top = position.Y + Position.Y;
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

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition, Vector2 otherPosition)
        {
            float left = thisPosition.X + Position.X;
            float top = thisPosition.Y + Position.Y;
            float right = left + Width;
            float bottom = top + Height;

            float px = otherPosition.X + point.Position.X;
            float py = otherPosition.Y + point.Position.Y;

            return px >= left && px <= right && py >= top && py <= bottom;
        }

        /// <summary>
        /// Checks if a line segment intersects this rectangle.
        /// Tests if either endpoint is inside, or if the line crosses any edge.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition, Vector2 otherPosition)
        {
            float left = thisPosition.X + Position.X;
            float top = thisPosition.Y + Position.Y;
            float right = left + Width;
            float bottom = top + Height;

            float x1 = otherPosition.X + line.Position.X;
            float y1 = otherPosition.Y + line.Position.Y;
            float x2 = x1 + line.PositionEnd.X;
            float y2 = y1 + line.PositionEnd.Y;

            // Check if either endpoint is inside the rectangle
            if ((x1 >= left && x1 <= right && y1 >= top && y1 <= bottom) ||
                (x2 >= left && x2 <= right && y2 >= top && y2 <= bottom))
                return true;

            // Check intersection with each edge of the rectangle
            LineCollision topEdge       = new(new Vector2(left, top), new Vector2(right, top));
            LineCollision bottomEdge    = new(new Vector2(left, bottom), new Vector2(right, bottom));
            LineCollision leftEdge      = new(new Vector2(left, top), new Vector2(left, bottom));
            LineCollision rightEdge     = new(new Vector2(right, top), new Vector2(right, bottom));

            return line.IntersectsLine(topEdge, otherPosition, Vector2.Zero) ||
                   line.IntersectsLine(bottomEdge, otherPosition, Vector2.Zero) ||
                   line.IntersectsLine(leftEdge, otherPosition, Vector2.Zero) ||
                   line.IntersectsLine(rightEdge, otherPosition, Vector2.Zero);
        }

        /// <summary>
        /// Checks if this rectangle intersects with another rectangle.
        /// Uses axis-aligned bounding box (AABB) collision detection for optimal performance.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition, Vector2 otherPosition)
        {
            float left1 = thisPosition.X + Position.X;
            float top1 = thisPosition.Y + Position.Y;
            float right1 = left1 + Width;
            float bottom1 = top1 + Height;

            float left2 = otherPosition.X + rectangle.Position.X;
            float top2 = otherPosition.Y + rectangle.Position.Y;
            float right2 = left2 + rectangle.Width;
            float bottom2 = top2 + rectangle.Height;

            // AABB collision detection
            return !(right1 < left2 || left1 > right2 || bottom1 < top2 || top1 > bottom2);
        }

        /// <summary>
        /// Checks if this rectangle intersects with a circle.
        /// Uses clamping to find the closest point on the rectangle to the circle center.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to circle's rectangle collision detection
            return circle.IntersectsRectangle(this, thisPosition, otherPosition);
        }

        #endregion
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    public class RectangleCollision : CollisionMask
    {
        #region Properties

        public float Width { get; set; }

        public float Height { get; set; }

        public override float BBoxLeft => Position.X;

        public override float BBoxRight => Position.X + Width;

        public override float BBoxTop => Position.Y;

        public override float BBoxBottom => Position.Y + Height;

        #endregion

        #region Constructors

        public RectangleCollision(Vector2 position, uint tag, float width, float height, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
            Width = width;
            Height = height;
        }

        #endregion

        #region Public Methods

        public override bool Intersects(CollisionMask other, Vector2 thisPosition)
        {
            if (!CheckOneWay(other, thisPosition))
                return false;

            return other switch
            {
                PointCollision point            => IntersectsPoint(point, thisPosition),
                LineCollision line              => IntersectsLine(line, thisPosition),
                RectangleCollision rectangle    => IntersectsRectangle(rectangle, thisPosition),
                CircleCollision circle          => IntersectsCircle(circle, thisPosition),
                _ => false
            };
        }

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

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            float left = thisPosition.X + Position.X;
            float top = thisPosition.Y + Position.Y;
            float right = left + Width;
            float bottom = top + Height;

            float px = point.Position.X;
            float py = point.Position.Y;

            return px >= left && px <= right && py >= top && py <= bottom;
        }

        /// <summary>
        /// Checks if a line segment intersects this rectangle.
        /// Tests if either endpoint is inside, or if the line crosses any edge.
        /// </summary>
        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            float left = thisPosition.X + Position.X;
            float top = thisPosition.Y + Position.Y;
            float right = left + Width;
            float bottom = top + Height;

            float x1 = line.Position.X;
            float y1 = line.Position.Y;
            float x2 = x1 + line.PositionEnd.X;
            float y2 = y1 + line.PositionEnd.Y;

            // Check if either endpoint is inside the rectangle
            if ((x1 >= left && x1 <= right && y1 >= top && y1 <= bottom) ||
                (x2 >= left && x2 <= right && y2 >= top && y2 <= bottom))
                return true;

            // Check intersection with each edge of the rectangle
            LineCollision topEdge       = new(new Vector2(left, top), Tag, new Vector2(right, top));
            LineCollision bottomEdge    = new(new Vector2(left, bottom), Tag, new Vector2(right, bottom));
            LineCollision leftEdge      = new(new Vector2(left, top), Tag, new Vector2(left, bottom));
            LineCollision rightEdge     = new(new Vector2(right, top), Tag, new Vector2(right, bottom));

            return line.IntersectsLine(topEdge, Vector2.Zero) ||
                   line.IntersectsLine(bottomEdge, Vector2.Zero) ||
                   line.IntersectsLine(leftEdge, Vector2.Zero) ||
                   line.IntersectsLine(rightEdge, Vector2.Zero);
        }

        /// <summary>
        /// Checks if this rectangle intersects with another rectangle.
        /// Uses axis-aligned bounding box (AABB) collision detection for optimal performance.
        /// </summary>
        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            float left1 = thisPosition.X + Position.X;
            float top1 = thisPosition.Y + Position.Y;
            float right1 = left1 + Width;
            float bottom1 = top1 + Height;

            float left2 = rectangle.Position.X;
            float top2 = rectangle.Position.Y;
            float right2 = left2 + rectangle.Width;
            float bottom2 = top2 + rectangle.Height;

            // AABB collision detection
            return !(right1 <= left2 || left1 >= right2 || bottom1 <= top2 || top1 >= bottom2);
        }

        /// <summary>
        /// Checks if this rectangle intersects with a circle.
        /// Uses clamping to find the closest point on the rectangle to the circle center.
        /// </summary>
        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            // Delegate to circle's rectangle collision detection
            return circle.IntersectsRectangle(this, thisPosition);
        }

        #endregion
    }
}

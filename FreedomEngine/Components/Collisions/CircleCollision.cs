using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Components.Collisions
{
    public class CircleCollision : CollisionMask
    {
        #region Properties

        /// <summary>
        /// Gets the radius of the circle.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Gets the diameter of the circle (twice the radius).
        /// </summary>
        public float Diameter => Radius * 2f;

        #endregion

        #region Constructors

        public CircleCollision(Vector2 position, float radius) : base(position)
        {
            Radius = radius;
        }

        #endregion

        #region Public Methods

        public override bool Intersects(CollisionMask other, Vector2 thisPosition)
        {
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
            float centerX = Position.X;
            float centerY = Position.Y;
            float radius = Radius;
            int segments = 32;

            // Draw circle using line segments
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (float)(2 * Math.PI * i / segments);
                float angle2 = (float)(2 * Math.PI * (i + 1) / segments);

                float x1 = centerX + radius * MathF.Cos(angle1);
                float y1 = centerY + radius * MathF.Sin(angle1);
                float x2 = centerX + radius * MathF.Cos(angle2);
                float y2 = centerY + radius * MathF.Sin(angle2);

                DrawDebugLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2));
            }
        }

        #endregion

        #region Internal Methods

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            float centerX = thisPosition.X + Position.X;
            float centerY = thisPosition.Y + Position.Y;
            float px = point.Position.X;
            float py = point.Position.Y;

            float dx = px - centerX;
            float dy = py - centerY;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= Radius * Radius;
        }

        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            float centerX = thisPosition.X + Position.X;
            float centerY = thisPosition.Y + Position.Y;

            float x1 = line.Position.X;
            float y1 = line.Position.Y;
            float x2 = x1 + line.PositionEnd.X;
            float y2 = y1 + line.PositionEnd.Y;

            // Vector from line start to circle center
            float dx = centerX - x1;
            float dy = centerY - y1;

            // Line direction vector
            float ldx = x2 - x1;
            float ldy = y2 - y1;

            // Line length squared
            float lineLengthSquared = ldx * ldx + ldy * ldy;

            // Find closest point on line to circle center (clamped to segment)
            float t = Math.Clamp((dx * ldx + dy * ldy) / lineLengthSquared, 0f, 1f);

            // Closest point coordinates
            float closestX = x1 + t * ldx;
            float closestY = y1 + t * ldy;

            // Distance from closest point to circle center
            float distX = centerX - closestX;
            float distY = centerY - closestY;
            float distanceSquared = distX * distX + distY * distY;

            return distanceSquared <= Radius * Radius;
        }

        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            float centerX = thisPosition.X + Position.X;
            float centerY = thisPosition.Y + Position.Y;

            float left = rectangle.Position.X;
            float top = rectangle.Position.Y;
            float right = left + rectangle.Width;
            float bottom = top + rectangle.Height;

            // Find the closest point on the rectangle to the circle center
            float closestX = Math.Clamp(centerX, left, right);
            float closestY = Math.Clamp(centerY, top, bottom);

            // Calculate distance from circle center to closest point
            float dx = centerX - closestX;
            float dy = centerY - closestY;
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= Radius * Radius;
        }

        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            float center1X = thisPosition.X + Position.X;
            float center1Y = thisPosition.Y + Position.Y;
            float center2X = circle.Position.X;
            float center2Y = circle.Position.Y;

            float dx = center2X - center1X;
            float dy = center2Y - center1Y;
            float distanceSquared = dx * dx + dy * dy;

            float radiusSum = Radius + circle.Radius;
            return distanceSquared <= radiusSum * radiusSum;
        }

        #endregion
    }
}

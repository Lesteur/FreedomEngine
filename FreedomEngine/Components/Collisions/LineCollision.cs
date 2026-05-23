using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.Components.Collisions
{
    public class LineCollision : CollisionMask
    {
        #region Properties

        public Vector2 PositionEnd { get; set; }

        #endregion

        #region Constructors

        public LineCollision(Vector2 position, Vector2 positionEnd) : base(position)
        {
            PositionEnd = positionEnd;
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
            float x1 = position.X + Position.X;
            float y1 = position.Y + Position.Y;
            float x2 = position.X + PositionEnd.X;
            float y2 = position.Y + PositionEnd.Y;

            DrawDebugLine(spriteBatch, new Vector2(x1, y1), new Vector2(x2, y2));
        }

        #endregion

        #region Internal Methods

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition, Vector2 otherPosition)
        {
            float x1 = thisPosition.X + Position.X;
            float y1 = thisPosition.Y + Position.Y;
            float x2 = thisPosition.X + PositionEnd.X;
            float y2 = thisPosition.Y + PositionEnd.Y;

            float px = otherPosition.X + point.Position.X;
            float py = otherPosition.Y + point.Position.Y;

            // Calculate distances
            float lineLength = MathF.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
            float d1 = MathF.Sqrt((px - x1) * (px - x1) + (py - y1) * (py - y1));
            float d2 = MathF.Sqrt((px - x2) * (px - x2) + (py - y2) * (py - y2));

            // Check if sum of distances equals line length (within epsilon)
            return Math.Abs((d1 + d2) - lineLength) < 0.01f;
        }

        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition, Vector2 otherPosition)
        {
            float x1 = thisPosition.X + Position.X;
            float y1 = thisPosition.Y + Position.Y;
            float x2 = thisPosition.X + PositionEnd.X;
            float y2 = thisPosition.Y + PositionEnd.Y;

            float x3 = otherPosition.X + line.Position.X;
            float y3 = otherPosition.Y + line.Position.Y;
            float x4 = otherPosition.X + line.PositionEnd.X;
            float y4 = otherPosition.Y + line.PositionEnd.Y;

            // Calculate the denominator for the intersection formula
            float denominator = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4));

            // Lines are parallel if denominator is zero
            if (Math.Abs(denominator) < float.Epsilon)
                return false;

            // Calculate intersection parameters
            float t = (((x1 - x3) * (y3 - y4)) - ((y1 - y3) * (x3 - x4))) / denominator;
            float u = -(((x1 - x2) * (y1 - y3)) - ((y1 - y2) * (x1 - x3))) / denominator;

            // Check if intersection point is within both line segments
            return t >= 0f && t <= 1f && u >= 0f && u <= 1f;
        }

        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to rectangle's line collision detection
            return rectangle.IntersectsLine(this, thisPosition, otherPosition);
        }

        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to circle's line collision detection
            return circle.IntersectsLine(this, thisPosition, otherPosition);
        }

        #endregion
    }
}

using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    public class PointCollision : CollisionMask
    {
        #region Constructors

        public PointCollision(Vector2 position) : base(position)
        {
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
            float worldX = position.X + Position.X;
            float worldY = position.Y + Position.Y;
            int size = 3;

            // Draw a cross
            DrawDebugLine(spriteBatch, new Vector2(worldX - size, worldY), new Vector2(worldX + size, worldY));
            DrawDebugLine(spriteBatch, new Vector2(worldX, worldY - size), new Vector2(worldX, worldY + size));
        }

        #endregion

        #region Internal Methods

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition, Vector2 otherPosition)
        {
            float x1 = thisPosition.X + Position.X;
            float y1 = thisPosition.Y + Position.Y;
            float x2 = otherPosition.X + point.Position.X;
            float y2 = otherPosition.Y + point.Position.Y;

            return Math.Abs(x1 - x2) < float.Epsilon && Math.Abs(y1 - y2) < float.Epsilon;
        }

        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to line's point collision detection
            return line.IntersectsPoint(this, thisPosition, otherPosition);
        }

        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to rectangle's point collision detection
            return rectangle.IntersectsPoint(this, thisPosition, otherPosition);
        }

        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition, Vector2 otherPosition)
        {
            // Delegate to circle's point collision detection
            return circle.IntersectsPoint(this, thisPosition, otherPosition);
        }

        #endregion
    }
}

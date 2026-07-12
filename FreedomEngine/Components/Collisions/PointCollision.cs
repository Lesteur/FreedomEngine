using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Components.Collisions
{
    public class PointCollision : CollisionMask
    {
        #region Properties

        public override float BBoxLeft => Position.X;

        public override float BBoxRight => Position.X;

        public override float BBoxTop => Position.Y;

        public override float BBoxBottom => Position.Y;

        #endregion

        #region Constructors

        public PointCollision(Vector2 position, uint tag, OneWayCollision oneWayCollision = OneWayCollision.None) : base(position, tag, oneWayCollision)
        {
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
            float worldX = Position.X;
            float worldY = Position.Y;
            int size = 3;

            // Draw a cross
            DrawDebugLine(spriteBatch, new Vector2(worldX - size, worldY), new Vector2(worldX + size, worldY));
            DrawDebugLine(spriteBatch, new Vector2(worldX, worldY - size), new Vector2(worldX, worldY + size));
        }

        #endregion

        #region Internal Methods

        internal override bool IntersectsPoint(PointCollision point, Vector2 thisPosition)
        {
            float x1 = thisPosition.X + Position.X;
            float y1 = thisPosition.Y + Position.Y;
            float x2 = point.Position.X;
            float y2 = point.Position.Y;

            return Math.Abs(x1 - x2) < float.Epsilon && Math.Abs(y1 - y2) < float.Epsilon;
        }

        internal override bool IntersectsLine(LineCollision line, Vector2 thisPosition)
        {
            // Delegate to line's point collision detection
            return line.IntersectsPoint(this, thisPosition);
        }

        internal override bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition)
        {
            // Delegate to rectangle's point collision detection
            return rectangle.IntersectsPoint(this, thisPosition);
        }

        internal override bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition)
        {
            // Delegate to circle's point collision detection
            return circle.IntersectsPoint(this, thisPosition);
        }

        #endregion
    }
}

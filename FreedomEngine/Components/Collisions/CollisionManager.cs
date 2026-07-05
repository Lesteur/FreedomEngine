using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Components.Collisions
{
    public class CollisionManager
    {
        #region Fields

        private readonly List<CollisionMask> _collisionMasks = [];

        #endregion

        #region Constructors

        public CollisionManager()
        {
        }

        #endregion

        #region Public Methods

        public RectangleCollision AddRectangleCollision(Vector2 position, ushort tag, float width, float height)
        {
            var rectangleCollision = new RectangleCollision(position, tag, width, height);
            _collisionMasks.Add(rectangleCollision);
            return rectangleCollision;
        }

        public CircleCollision AddCircleCollision(Vector2 position, ushort tag, float radius)
        {
            var circleCollision = new CircleCollision(position, tag, radius);
            _collisionMasks.Add(circleCollision);
            return circleCollision;
        }

        public LineCollision AddLineCollision(Vector2 position, ushort tag, Vector2 positionEnd)
        {
            var lineCollision = new LineCollision(position, tag, positionEnd);
            _collisionMasks.Add(lineCollision);
            return lineCollision;
        }

        public PointCollision AddPointCollision(Vector2 position, ushort tag)
        {
            var pointCollision = new PointCollision(position, tag);
            _collisionMasks.Add(pointCollision);
            return pointCollision;
        }

        public void Remove(CollisionMask mask)
        {
            _collisionMasks.Remove(mask);
        }

        public bool CheckCollisions(CollisionMask mask, ushort tag, Vector2 offset)
        {
            foreach (var otherMask in _collisionMasks)
            {
                if ((mask != otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of this tween manager and cleans up resources.
        /// </summary>
        public void Dispose()
        {
            _collisionMasks.Clear();
        }

        #endregion
    }
}

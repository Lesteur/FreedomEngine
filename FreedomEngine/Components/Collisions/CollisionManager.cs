using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Components.Collisions
{
    public class CollisionManager
    {
        #region Fields

        private readonly List<CollisionMask> _collisionMasks;

        #endregion

        #region Constructors

        public CollisionManager()
        {
            _collisionMasks = [];
        }

        #endregion

        #region Public Methods

        public void Remove(CollisionMask mask)
        {
            _collisionMasks.Remove(mask);
        }

        public bool CheckCollisions(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            foreach (var otherMask in _collisionMasks)
            {
                if ((mask != otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    return true;
                }
            }

            return false;
        }

        public CollisionMask CheckCollisionsInstance(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            foreach (var otherMask in _collisionMasks)
            {
                if ((mask != otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    return otherMask;
                }
            }
            return null;
        }

        public List<CollisionMask> GetCollisionsInstances(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            List<CollisionMask> collisions = [];

            foreach (var otherMask in _collisionMasks)
            {
                if ((mask != otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    collisions.Add(otherMask);
                }
            }

            return collisions;
        }

        #endregion

        #region Internal Methods

        internal void Add(CollisionMask mask)
        {
            _collisionMasks.Add(mask);
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

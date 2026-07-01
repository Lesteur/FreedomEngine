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

        public void Add(CollisionMask mask)
        {
            _collisionMasks.Add(mask);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this tween manager and cleans up resources.
        /// </summary>
        /// <param name="disposing">Indicates whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            _collisionMasks.Clear();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Components.Collisions
{
    public class CollisionManager
    {
        #region Properties

        List<CollisionMask> _collisionMasks = [];

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

        public void Clear()
        {
            _collisionMasks.Clear();
        }

        #endregion
    }
}

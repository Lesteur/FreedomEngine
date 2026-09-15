using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Tracks the collision masks belonging to a scene and resolves collision queries against them.
    /// </summary>
    /// <remarks>
    /// A <see cref="CollisionMask"/> registers itself with the manager assigned to
    /// <see cref="CollisionMask.Controller"/> when it is constructed. Masks are never removed
    /// automatically, so an entity that owns a mask should call <see cref="Remove"/> when it is
    /// destroyed, or the whole set should be discarded with <see cref="Clear"/>.
    /// </remarks>
    public class CollisionManager
    {
        #region Fields

        /// <summary>
        /// The collision masks currently registered with this manager.
        /// </summary>
        private readonly List<CollisionMask> _collisionMasks;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of collision masks currently registered with this manager.
        /// </summary>
        public int Count => _collisionMasks.Count;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionManager"/> class.
        /// </summary>
        public CollisionManager()
        {
            _collisionMasks = [];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Removes a collision mask from this manager, so it no longer takes part in collision queries.
        /// </summary>
        /// <param name="mask">The mask to remove.</param>
        /// <returns><see langword="true"/> if the mask was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool Remove(CollisionMask mask)
        {
            return _collisionMasks.Remove(mask);
        }

        /// <summary>
        /// Removes every collision mask from this manager.
        /// </summary>
        public void Clear()
        {
            _collisionMasks.Clear();
        }

        /// <summary>
        /// Determines whether the given mask, translated by the given offset, overlaps any other
        /// registered mask carrying at least one of the queried tag bits.
        /// </summary>
        /// <param name="mask">The mask to test. It is excluded from its own query.</param>
        /// <param name="tag">The tag bits identifying which masks to test against.</param>
        /// <param name="offset">The offset applied to <paramref name="mask"/> before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision rules should be ignored.</param>
        /// <returns><see langword="true"/> if an overlap is found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mask"/> is <see langword="null"/>.</exception>
        public bool CheckCollisions(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            ArgumentNullException.ThrowIfNull(mask);

            foreach (var otherMask in _collisionMasks)
            {
                if (!ReferenceEquals(mask, otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the first registered mask carrying at least one of the queried tag bits that the given
        /// mask overlaps, when translated by the given offset.
        /// </summary>
        /// <param name="mask">The mask to test. It is excluded from its own query.</param>
        /// <param name="tag">The tag bits identifying which masks to test against.</param>
        /// <param name="offset">The offset applied to <paramref name="mask"/> before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision rules should be ignored.</param>
        /// <returns>The first overlapping mask, or <see langword="null"/> if none is found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mask"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Which mask is returned first depends on registration order, which is not guaranteed to be
        /// stable across scenes.
        /// </remarks>
        public CollisionMask CheckCollisionsInstance(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            ArgumentNullException.ThrowIfNull(mask);

            foreach (var otherMask in _collisionMasks)
            {
                if (!ReferenceEquals(mask, otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    return otherMask;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets every registered mask carrying at least one of the queried tag bits that the given
        /// mask overlaps, when translated by the given offset.
        /// </summary>
        /// <param name="mask">The mask to test. It is excluded from its own query.</param>
        /// <param name="tag">The tag bits identifying which masks to test against.</param>
        /// <param name="offset">The offset applied to <paramref name="mask"/> before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision rules should be ignored.</param>
        /// <returns>The overlapping masks, or an empty list if none is found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="mask"/> is <see langword="null"/>.</exception>
        public List<CollisionMask> GetCollisionsInstances(CollisionMask mask, uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            ArgumentNullException.ThrowIfNull(mask);

            List<CollisionMask> collisions = null;

            foreach (var otherMask in _collisionMasks)
            {
                if (!ReferenceEquals(mask, otherMask) && (otherMask.Tag & tag) != 0 && mask.Intersects(otherMask, offset, ignoreOneWayCollisions))
                {
                    collisions ??= [];
                    collisions.Add(otherMask);
                }
            }

            return collisions ?? [];
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Registers a collision mask with this manager, so it takes part in collision queries.
        /// </summary>
        /// <param name="mask">The mask to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="mask"/> is <see langword="null"/>.</exception>
        internal void Add(CollisionMask mask)
        {
            ArgumentNullException.ThrowIfNull(mask);

            _collisionMasks.Add(mask);
        }

        #endregion
    }
}
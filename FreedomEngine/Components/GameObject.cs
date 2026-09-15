using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Components.Collisions;
using FreedomEngine.Core;
using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a base entity in the game: a drawable entity that additionally carries an optional
    /// collision mask and participates in the scene's collision queries.
    /// </summary>
    public abstract class GameObject : DrawableEntity
    {
        #region Fields

        /// <summary>
        /// The collision mask associated with this entity, or <see langword="null"/> if it does not collide.
        /// </summary>
        private CollisionMask _collision;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the scene that game objects query for collisions.
        /// </summary>
        /// <remarks>
        /// The engine assigns this as part of a scene transition. See
        /// <see cref="Application.ChangeScene"/>.
        /// </remarks>
        public static Scene Scene { get; set; }

        /// <summary>
        /// Gets or sets the camera used to cull game objects that fall outside the view.
        /// </summary>
        /// <remarks>
        /// When <see langword="null"/>, no culling is performed and every visible game object is drawn.
        /// </remarks>
        public static Camera Camera { get; set; }

        /// <summary>
        /// Gets or sets the collision mask associated with this entity.
        /// </summary>
        /// <remarks>
        /// Assigning a non-null mask also binds that mask's collider back to this entity.
        /// </remarks>
        public CollisionMask Collision
        {
            get => _collision;
            set
            {
                _collision = value;

                if (_collision != null)
                    _collision.Collider = this;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="GameObject"/> class with the specified sprite and initial position.
        /// </summary>
        /// <param name="sprite">
        /// The sprite associated with the entity. May be <see langword="null"/> for an entity that
        /// does not render until a sprite is assigned.
        /// </param>
        /// <param name="position">The initial position of the entity in 2D space.</param>
        /// <param name="collisionMask">
        /// The collision mask associated with the entity, or <see langword="null"/> for an entity that
        /// does not collide.
        /// </param>
        protected GameObject(Sprite sprite, Vector2 position, CollisionMask collisionMask = null) : base(sprite, position)
        {
            // The Collision setter binds the mask's Collider back to this entity, so no separate
            // assignment is needed here.
            Collision = collisionMask;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws the entity using the given sprite batch, skipping it entirely when it falls outside
        /// the view of <see cref="Camera"/>.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            if (!Visible || Sprite == null)
                return;

            if (Camera != null)
            {
                var origin = Sprite.Origin;
                var cullPosition = new Vector2(X + origin.X, Y + origin.Y);

                if (!Camera.IsInView(cullPosition, Width, Height))
                    return;
            }

            Collision?.Draw(spriteBatch);

            base.Draw(spriteBatch);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether this entity's collision mask overlaps any mask with the given tag, when
        /// offset by the given amount.
        /// </summary>
        /// <param name="tag">The tag identifying which collision masks to test against.</param>
        /// <param name="offset">The offset applied to this entity's mask before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision masks should be ignored.</param>
        /// <returns>
        /// <see langword="true"/> if an overlap is found; otherwise, <see langword="false"/>. Always
        /// <see langword="false"/> when this entity has no <see cref="Collision"/> mask.
        /// </returns>
        /// <exception cref="InvalidOperationException"><see cref="Scene"/> has not been assigned.</exception>
        public bool CollidesWith(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return false;

            return RequireScene().Collisions.CheckCollisions(Collision, tag, offset, ignoreOneWayCollisions);
        }

        /// <summary>
        /// Gets the first collision mask with the given tag that this entity's mask overlaps, when
        /// offset by the given amount.
        /// </summary>
        /// <param name="tag">The tag identifying which collision masks to test against.</param>
        /// <param name="offset">The offset applied to this entity's mask before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision masks should be ignored.</param>
        /// <returns>
        /// The first overlapping collision mask, or <see langword="null"/> if none is found or this
        /// entity has no <see cref="Collision"/> mask.
        /// </returns>
        /// <exception cref="InvalidOperationException"><see cref="Scene"/> has not been assigned.</exception>
        public CollisionMask CollidesWithInstance(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return null;

            return RequireScene().Collisions.CheckCollisionsInstance(Collision, tag, offset, ignoreOneWayCollisions);
        }

        /// <summary>
        /// Gets every collision mask with the given tag that this entity's mask overlaps, when offset
        /// by the given amount.
        /// </summary>
        /// <param name="tag">The tag identifying which collision masks to test against.</param>
        /// <param name="offset">The offset applied to this entity's mask before testing.</param>
        /// <param name="ignoreOneWayCollisions">Whether one-way collision masks should be ignored.</param>
        /// <returns>
        /// The overlapping collision masks. Returns an empty list when none is found or this entity
        /// has no <see cref="Collision"/> mask.
        /// </returns>
        /// <exception cref="InvalidOperationException"><see cref="Scene"/> has not been assigned.</exception>
        public List<CollisionMask> CollidesWithInstances(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return [];

            return RequireScene().Collisions.GetCollisionsInstances(Collision, tag, offset, ignoreOneWayCollisions);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns the active <see cref="Scene"/>, throwing a descriptive exception if none is assigned.
        /// </summary>
        /// <returns>The active scene.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Scene"/> has not been assigned.</exception>
        private static Scene RequireScene()
        {
            return Scene ?? throw new InvalidOperationException($"{nameof(GameObject)}.{nameof(Scene)} must be assigned before performing collision queries.");
        }

        #endregion
    }
}
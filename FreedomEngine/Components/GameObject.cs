using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a base entity in the game.
    /// </summary>
    public class GameObject : DrawableEntity
    {
        #region Fields

        private CollisionMask _collision;

        #endregion

        #region Properties

        public static Scene Scene { get; set; }

        public static Camera Camera { get; set; }

        /// <summary>
        /// Gets or Sets the collision mask associated with this entity.
        /// </summary>
        public CollisionMask Collision
        {
            get => _collision;
            set
            {
                _collision = value;
                if (_collision != null)
                {
                    _collision.Collider = this;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="GameObject"/> class with the specified sprite and initial position.
        /// </summary>
        /// <param name="sprite">The sprite associated with the entity.</param>
        /// <param name="position">The initial position of the entity in 2D space.</param>
        /// <param name="collisionMask">The collision mask associated with the entity.</param>
        public GameObject(Sprite sprite, Vector2 position, CollisionMask collisionMask = null) : base(sprite, position)
        {
            if (collisionMask != null)
            {
                Collision = collisionMask;
                Collision.Collider = this;
            }
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws the entity using the SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">The rendering context.</param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible || Sprite?.Animation.Frames == null)
                return;

            Collision?.Draw(spriteBatch);

            var origin = Sprite.Origin;
            var position = new Vector2(X + origin.X, Y + origin.Y);

            if (Camera != null)
            {
                if (!Camera.IsInView(position, Width, Height))
                    return;
            }

            Sprite.Animation.Frames[CurrentFrame].Draw(
                spriteBatch,
                Position,
                Color,
                Rotation,
                origin,
                Scale,
                Effects,
                LayerDepth
            );
        }

        #endregion

        #region Public Methods

        public bool CollidesWith(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return false;

            return Scene.Collisions.CheckCollisions(Collision, tag, offset, ignoreOneWayCollisions);
        }

        public CollisionMask CollidesWithInstance(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return null;

            return Scene.Collisions.CheckCollisionsInstance(Collision, tag, offset, ignoreOneWayCollisions);
        }

        public List<CollisionMask> CollidesWithInstances(uint tag, Vector2 offset, bool ignoreOneWayCollisions = false)
        {
            if (Collision == null)
                return [];

            return Scene.Collisions.GetCollisionsInstances(Collision, tag, offset, ignoreOneWayCollisions);
        }

        #endregion
    }
}
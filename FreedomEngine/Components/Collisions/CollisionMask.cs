using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Core;

namespace FreedomEngine.Components.Collisions
{
    /// <summary>
    /// Identifies the single direction from which a one-way collision mask may be collided with.
    /// </summary>
    public enum OneWayCollision
    {
        /// <summary>The mask is solid from every direction.</summary>
        None,

        /// <summary>The mask may only be collided with when approached from its left side.</summary>
        Left,

        /// <summary>The mask may only be collided with when approached from its right side.</summary>
        Right,

        /// <summary>The mask may only be collided with when approached from above.</summary>
        Top,

        /// <summary>The mask may only be collided with when approached from below.</summary>
        Bottom
    }

    /// <summary>
    /// Provides the base functionality for a collision shape that can be tested for overlap against
    /// other shapes through the scene's <see cref="CollisionManager"/>.
    /// </summary>
    /// <remarks>
    /// A mask registers itself with <see cref="Controller"/> when constructed, so a mask must not be
    /// created before the controller has been assigned. Intersection tests are dispatched through
    /// <see cref="Intersects"/>, which resolves the concrete type of the other shape and calls the
    /// matching <c>Intersects*</c> overload.
    /// </remarks>
    public abstract class CollisionMask
    {
        #region Fields

        /// <summary>
        /// The position of this mask. When <see cref="Collider"/> is set, this is an offset relative
        /// to that entity; otherwise it is an absolute world position.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// The tolerance used for floating-point comparisons in intersection tests.
        /// </summary>
        /// <remarks>
        /// Deliberately larger than <see cref="float.Epsilon"/>, which is the smallest representable
        /// denormal and is far too small to absorb accumulated rounding error.
        /// </remarks>
        protected const float Epsilon = 1e-5f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the manager that newly created collision masks are registered with.
        /// </summary>
        /// <remarks>
        /// Must be assigned before constructing a <see cref="CollisionMask"/>; the engine assigns this
        /// as part of a scene transition. See <see cref="Application.ChangeScene"/>.
        /// </remarks>
        public static CollisionManager Controller { get; set; }

        /// <summary>
        /// Gets the world position of this mask, or sets its position relative to <see cref="Collider"/>.
        /// </summary>
        /// <remarks>
        /// This property is deliberately asymmetric. The setter stores the value as-is, but the getter
        /// adds the position of <see cref="Collider"/> when one is attached, so a mask follows its
        /// entity automatically. As a result, reading this property does not necessarily return the
        /// value that was last assigned to it.
        /// </remarks>
        public Vector2 Position
        {
            get => Collider != null ? Collider.Position + _position : _position;
            set => _position = value;
        }

        /// <summary>
        /// Gets or sets the tag bits identifying which category or categories this mask belongs to.
        /// </summary>
        /// <remarks>
        /// Tags are treated as a bit field: a collision query matches a mask when the bitwise AND of
        /// the queried tag and this value is non-zero.
        /// </remarks>
        public uint Tag { get; set; }

        /// <summary>
        /// Gets or sets the entity this mask is attached to, or <see langword="null"/> if the mask is
        /// positioned in absolute world coordinates.
        /// </summary>
        public GameObject Collider { get; set; }

        /// <summary>
        /// Gets or sets the direction from which this mask may be collided with.
        /// </summary>
        public OneWayCollision OneWayCollision { get; set; }

        /// <summary>
        /// Gets the world-space X coordinate of the left edge of this mask's bounding box.
        /// </summary>
        public abstract float BBoxLeft { get; }

        /// <summary>
        /// Gets the world-space X coordinate of the right edge of this mask's bounding box.
        /// </summary>
        public abstract float BBoxRight { get; }

        /// <summary>
        /// Gets the world-space Y coordinate of the top edge of this mask's bounding box.
        /// </summary>
        public abstract float BBoxTop { get; }

        /// <summary>
        /// Gets the world-space Y coordinate of the bottom edge of this mask's bounding box.
        /// </summary>
        public abstract float BBoxBottom { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionMask"/> class and registers it with
        /// <see cref="Controller"/>.
        /// </summary>
        /// <param name="position">
        /// The position of the mask. Interpreted relative to <see cref="Collider"/> once one is
        /// attached; see <see cref="Position"/>.
        /// </param>
        /// <param name="tag">The tag bits identifying which category or categories this mask belongs to.</param>
        /// <param name="oneWayCollision">The direction from which this mask may be collided with.</param>
        /// <exception cref="InvalidOperationException"><see cref="Controller"/> has not been assigned.</exception>
        protected CollisionMask(Vector2 position, uint tag, OneWayCollision oneWayCollision = OneWayCollision.None)
        {
            if (Controller == null)
                throw new InvalidOperationException($"{nameof(CollisionMask)}.{nameof(Controller)} must be assigned a {nameof(CollisionManager)} before creating a collision mask.");

            Position = position;
            Tag = tag;
            Collider = null;
            OneWayCollision = oneWayCollision;

            // Register last: the manager must never observe a partially initialized mask.
            Controller.Add(this);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Draws a debug visualization of this mask's shape.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        public abstract void Draw(SpriteBatch spriteBatch);

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws a single-pixel-thick debug line between two world positions.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <param name="positionStart">The world position the line starts at.</param>
        /// <param name="positionEnd">The world position the line ends at.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public void DrawDebugLine(SpriteBatch spriteBatch, Vector2 positionStart, Vector2 positionEnd)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            float dx = positionEnd.X - positionStart.X;
            float dy = positionEnd.Y - positionStart.Y;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            float angle = MathF.Atan2(dy, dx);

            spriteBatch.Draw(
                Application.PixelTexture,
                positionStart,
                null,
                Color.Red,
                angle,
                Vector2.Zero,
                new Vector2(length, 1f),
                SpriteEffects.None,
                0f
            );
        }

        /// <summary>
        /// Determines whether this mask, translated by the given offset, overlaps another mask.
        /// </summary>
        /// <param name="other">The mask to test against. It is tested at its current position, untranslated.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <param name="ignoreOneWay">
        /// Whether to skip the one-way direction check and treat both masks as solid from every side.
        /// </param>
        /// <returns><see langword="true"/> if the two shapes overlap; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Returns <see langword="false"/> for any shape type this dispatcher does not recognize, so a
        /// new <see cref="CollisionMask"/> subclass must also be added to the switch below and to the
        /// <c>Intersects*</c> overloads of every existing subclass.
        /// </remarks>
        public bool Intersects(CollisionMask other, Vector2 thisPosition, bool ignoreOneWay = false)
        {
            ArgumentNullException.ThrowIfNull(other);

            if (!ignoreOneWay && !CheckOneWay(other, thisPosition))
                return false;

            return other switch
            {
                PointCollision point => IntersectsPoint(point, thisPosition),
                LineCollision line => IntersectsLine(line, thisPosition),
                RectangleCollision rectangle => IntersectsRectangle(rectangle, thisPosition),
                CircleCollision circle => IntersectsCircle(circle, thisPosition),
                _ => false
            };
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Determines whether the one-way settings of this mask and the other mask allow a collision
        /// to be reported for the given movement offset.
        /// </summary>
        /// <param name="other">The mask being tested against.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <returns>
        /// <see langword="true"/> if the collision is allowed to be reported; <see langword="false"/>
        /// to suppress it.
        /// </returns>
        /// <remarks>
        /// A collision is suppressed when the two bounding boxes already overlap before the offset is
        /// applied, so an entity that is already inside a one-way mask passes back out of it freely.
        /// Otherwise the movement direction is checked against the one-way side.
        /// </remarks>
        protected bool CheckOneWay(CollisionMask other, Vector2 thisPosition)
        {
            if (OneWayCollision == OneWayCollision.None && other.OneWayCollision == OneWayCollision.None)
                return true;

            float thisL = BBoxLeft;
            float thisR = BBoxRight;
            float thisT = BBoxTop;
            float thisB = BBoxBottom;

            float otherL = other.BBoxLeft;
            float otherR = other.BBoxRight;
            float otherT = other.BBoxTop;
            float otherB = other.BBoxBottom;

            bool isOverlapping = !(thisR <= otherL || thisL >= otherR || thisB <= otherT || thisT >= otherB);

            if (isOverlapping)
                return false;

            // Check this one-way
            if (OneWayCollision == OneWayCollision.None)
            {
                if (!IsOneWayValid(other.OneWayCollision, thisPosition))
                    return false;
            }

            // Check other one-way
            if (other.OneWayCollision == OneWayCollision.None)
            {
                if (!IsOneWayValid(OneWayCollision, thisPosition))
                    return false;
            }

            return true;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Determines whether this mask, translated by the given offset, overlaps a point.
        /// </summary>
        /// <param name="point">The point mask to test against, at its current position.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <returns><see langword="true"/> if the shapes overlap; otherwise, <see langword="false"/>.</returns>
        internal abstract bool IntersectsPoint(PointCollision point, Vector2 thisPosition);

        /// <summary>
        /// Determines whether this mask, translated by the given offset, overlaps a line segment.
        /// </summary>
        /// <param name="line">The line mask to test against, at its current position.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <returns><see langword="true"/> if the shapes overlap; otherwise, <see langword="false"/>.</returns>
        internal abstract bool IntersectsLine(LineCollision line, Vector2 thisPosition);

        /// <summary>
        /// Determines whether this mask, translated by the given offset, overlaps a rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle mask to test against, at its current position.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <returns><see langword="true"/> if the shapes overlap; otherwise, <see langword="false"/>.</returns>
        internal abstract bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition);

        /// <summary>
        /// Determines whether this mask, translated by the given offset, overlaps a circle.
        /// </summary>
        /// <param name="circle">The circle mask to test against, at its current position.</param>
        /// <param name="thisPosition">The offset applied to this mask before testing.</param>
        /// <returns><see langword="true"/> if the shapes overlap; otherwise, <see langword="false"/>.</returns>
        internal abstract bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition);

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines whether a movement offset approaches a one-way mask from its permitted side.
        /// </summary>
        /// <param name="oneWay">The one-way side being tested.</param>
        /// <param name="thisPosition">The movement offset applied to the moving mask.</param>
        /// <returns>
        /// <see langword="true"/> if the movement approaches from the permitted side, or if
        /// <paramref name="oneWay"/> is <see cref="OneWayCollision.None"/>; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        private static bool IsOneWayValid(OneWayCollision oneWay, Vector2 thisPosition)
        {
            return oneWay switch
            {
                OneWayCollision.Top => thisPosition.Y > 0f,
                OneWayCollision.Bottom => thisPosition.Y < 0f,
                OneWayCollision.Left => thisPosition.X > 0f,
                OneWayCollision.Right => thisPosition.X < 0f,
                _ => true
            };
        }

        #endregion
    }
}
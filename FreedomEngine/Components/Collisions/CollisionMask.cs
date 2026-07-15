using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Core;
using FreedomEngine.Collections;

namespace FreedomEngine.Components.Collisions
{
    public abstract class CollisionMask
    {
        #region Fields

        private static CollisionManager Controller => Application.Collisions;

        private Vector2 _position;

        #endregion

        #region Properties

        public Vector2 Position
        {
            set => _position = value;
            get
            {
                if (Collider != null)
                {
                    return Collider.Position + _position;
                }
                else
                {
                    return _position;
                }
            }
        }

        public uint Tag { get; set; }

        public OneWayCollision OneWayCollision { get; set; }

        public Entity Collider { get; set; }

        public abstract float BBoxLeft { get; }

        public abstract float BBoxRight { get; }

        public abstract float BBoxTop { get; }

        public abstract float BBoxBottom { get; }

        #endregion

        #region Constructors

        public CollisionMask(Vector2 position, uint tag, OneWayCollision oneWayCollision = OneWayCollision.None)
        {
            Position = position;
            Tag = tag;
            OneWayCollision = oneWayCollision;
            Collider = null;

            Controller.Add(this);
        }

        #endregion

        #region Public Methods

        public void DrawDebugLine(SpriteBatch spriteBatch, Vector2 positionStart, Vector2 positionEnd)
        {
            float dx = positionEnd.X - positionStart.X;
            float dy = positionEnd.Y - positionStart.Y;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            float angle = MathF.Atan2(dy, dx);

            spriteBatch.Draw(
                Application.PixelTexture,
                new Vector2(positionStart.X, positionStart.Y),
                null,
                Color.Red,
                angle,
                Vector2.Zero,
                new Vector2(length, 1f),
                SpriteEffects.None,
                0f
            );
        }

        #endregion

        #region Lifecycle Methods

        public abstract void Draw(SpriteBatch spriteBatch);

        #endregion

        #region Public Methods

        public bool Intersects(CollisionMask other, Vector2 thisPosition, bool ignoreOneWay = false)
        {
            if (!ignoreOneWay && !CheckOneWay(other, thisPosition))
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

        #endregion

        #region Internal Methods

        /// <summary>
        /// Checks if a one-way collision allows the intersection between this mask and the other mask.
        /// It assumes an intersection has already been detected. We check if the overlap indicates 
        /// that the other object is fully entering from the allowed direction.
        /// </summary>
        protected bool CheckOneWay(CollisionMask other, Vector2 thisPosition)
        {
            if (this.OneWayCollision == OneWayCollision.None && other.OneWayCollision == OneWayCollision.None)
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
            if (this.OneWayCollision == OneWayCollision.None)
            {
                if (!IsOneWayValid(other.OneWayCollision, thisPosition))
                    return false;
            }

            // Check other one-way
            if (other.OneWayCollision == OneWayCollision.None)
            {
                if (!IsOneWayValid(this.OneWayCollision, thisPosition))
                    return false;
            }

            return true;
        }

        private bool IsOneWayValid(OneWayCollision oneWay, Vector2 thisPosition)
        {
            switch (oneWay)
            {
                case OneWayCollision.Top:
                    return (thisPosition.Y > 0f);

                case OneWayCollision.Bottom:
                    return (thisPosition.Y < 0f);

                case OneWayCollision.Left:
                    return (thisPosition.X > 0f);
                    
                case OneWayCollision.Right:
                    return (thisPosition.X < 0f);
            }

            return true;
        }

        internal abstract bool IntersectsPoint(PointCollision point, Vector2 thisPosition);

        internal abstract bool IntersectsLine(LineCollision line, Vector2 thisPosition);

        internal abstract bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition);

        internal abstract bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition);

        #endregion
    }
}

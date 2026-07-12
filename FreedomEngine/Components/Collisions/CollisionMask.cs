
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Core;

namespace FreedomEngine.Components.Collisions
{
    public abstract class CollisionMask
    {
        #region Fields

        private static CollisionManager Controller => Application.Collisions;

        #endregion

        #region Properties

        public Vector2 Position { get; set; }

        public uint Tag { get; set; }

        #endregion

        #region Constructors

        public CollisionMask(Vector2 position, uint tag)
        {
            Position = position;
            Tag = tag;

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

        #region Public Methods

        public abstract bool Intersects(CollisionMask other, Vector2 thisPosition);

        public abstract void Draw(SpriteBatch spriteBatch);

        #endregion

        #region Internal Methods

        internal abstract bool IntersectsPoint(PointCollision point, Vector2 thisPosition);

        internal abstract bool IntersectsLine(LineCollision line, Vector2 thisPosition);

        internal abstract bool IntersectsRectangle(RectangleCollision rectangle, Vector2 thisPosition);

        internal abstract bool IntersectsCircle(CircleCollision circle, Vector2 thisPosition);

        #endregion
    }
}

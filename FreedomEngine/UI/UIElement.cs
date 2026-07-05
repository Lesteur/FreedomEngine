using FreedomEngine.Collections.Tweens;
using FreedomEngine.Collections.Utilities;
using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.UI
{
    public class UIElement : Entity
    {
        #region Fields

        private Vector2 _startPosition;

        private Tween _hoverTween;

        #endregion

        #region Properties

        public bool IsFocused { get; set; } = false;

        public bool IsHovered { get; set; } = false;

        public bool IsEnabled { get; set; } = true;

        public bool IsPressed { get; set; } = false;

        #endregion

        #region Constructors

        public UIElement(Sprite sprite, Vector2 position) : base(sprite, position)
        {
            CollisionMask collision = Application.Collisions.AddRectangleCollision(new Vector2(Position.X, Position.Y), 0, Width, Height);
            Collision = collision;

            //_hoverTween = new Tween(0.2f, 0.0f, 1.0f, EaseInOut);
            _startPosition = new Vector2(position.X, position.Y);
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouse = Application.Input.Mouse;
            var mousePosition = new Vector2(mouse.X, mouse.Y);

            //if (Collision.Intersects(new PointCollision(mousePosition, 0), new Vector2(X, Y)))
            if (Collision.Intersects(new PointCollision(mousePosition, 0), new Vector2(X, Y)))
            {
                if (!IsHovered)
                {
                    OnHovered(true);
                }
            }
            else
            {
                if (IsHovered)
                {
                    OnHovered(false);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        #endregion

        #region Public Methods

        protected virtual void OnFocus(bool focus)
        {
            IsFocused = focus;
        }

        protected virtual void OnHovered(bool hovered)
        {
            IsHovered = hovered;

            if (_hoverTween != null && !_hoverTween.IsFinished)
            {
                _hoverTween.Stop();
            }

            if (hovered)
            {
                _hoverTween = Application.Tweens.TweenPosition(this, Position, _startPosition + new Vector2(20, 0), TimeSpan.FromSeconds(0.15), EasingFunctions.SineInOut);
            }
            else
            {
                _hoverTween = Application.Tweens.TweenPosition(this, Position, _startPosition, TimeSpan.FromSeconds(0.15), EasingFunctions.SineInOut);
            }
        }

        protected virtual void OnEnabled(bool enabled)
        {
            IsEnabled = enabled;
        }

        protected virtual void OnPressed(bool pressed)
        {
            IsPressed = pressed;
        }

        #endregion
    }
}

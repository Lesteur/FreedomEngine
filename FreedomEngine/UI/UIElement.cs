using FreedomEngine.Collections;
using FreedomEngine.Collections.Tweens;
using FreedomEngine.Collections.Utilities;
using FreedomEngine.Components;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace FreedomEngine.UI
{
    public class UIElement : Entity
    {
        #region Fields

        private Vector2 _startPosition;

        private Tween _hoverTween;

        private Rectangle _hoverRectangle;

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
            _hoverRectangle = new Rectangle((int)position.X, (int)position.Y, (int)Width, (int)Height);

            //_hoverTween = new Tween(0.2f, 0.0f, 1.0f, EaseInOut);
            _startPosition = new Vector2(position.X, position.Y);
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouse = Application.Input.Mouse;
            var mousePosition = new Vector2(mouse.Position.X / mouse.UIScale.X, mouse.Position.Y / mouse.UIScale.Y);

            if (_hoverRectangle.Contains(mousePosition))
            {
                if (!IsHovered)
                {
                    OnHovered(true);
                }

                if (mouse.WasButtonJustPressed(MouseButton.Left))
                {
                    OnPressed(true);
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

            Logger.Info($"UIElement {(pressed ? "Pressed" : "Released")} at Position: {Position}");
        }

        #endregion
    }
}

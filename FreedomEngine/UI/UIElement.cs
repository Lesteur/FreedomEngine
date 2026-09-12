using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.UI
{
    public abstract class UIElement : DrawableEntity, IUIElement
    {
        #region Properties

        public UIElement Parent { get; set; } = null;

        public Vector2 PositionDraw { get; set; } = Vector2.Zero;

        public Vector2 PositionTotal => Position + (Parent?.PositionTotal ?? Vector2.Zero);

        public Vector2 PositionTotalDraw => PositionTotal + PositionDraw + (Parent?.PositionDraw ?? Vector2.Zero);

        public bool IsFocused { get; protected set; } = false;

        public bool IsHovered { get; protected set; } = false;

        public bool IsEnabled { get; protected set; } = true;

        public bool IsPressed { get; protected set; } = false;

        #endregion

        #region Events

        /// <summary>
        /// Triggered when the button is successfully clicked and released while hovered.
        /// </summary>
        public event Action OnClick;

        #endregion

        #region Constructors

        public UIElement(Sprite sprite, Vector2 position) : base(sprite, position)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouse = Application.Input.Mouse;
            var mousePosition = new Vector2(mouse.Position.X / mouse.UIScale.X, mouse.Position.Y / mouse.UIScale.Y);
            var hoverRectangle = new Rectangle((int)(PositionTotal.X), (int)(PositionTotal.Y), (int)Width, (int)Height);

            if (hoverRectangle.Contains(mousePosition))
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
            if (!Visible || Sprite?.Frames == null)
                return;

            var origin = Sprite.Origin;
            var position = PositionTotal + PositionDraw;

            spriteBatch.Draw(Sprite.Texture, position, Sprite.Frames[CurrentFrame], Color, Rotation, origin, Scale, Effects, LayerDepth);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnFocus(bool focus)
        {
            IsFocused = focus;
        }

        protected virtual void OnHovered(bool hovered)
        {
            IsHovered = hovered;
        }

        protected virtual void OnEnabled(bool enabled)
        {
            IsEnabled = enabled;
        }

        protected virtual void OnPressed(bool pressed)
        {
            IsPressed = pressed;

            if (pressed)
            {
                OnClick?.Invoke();
            }
        }

        #endregion
    }
}

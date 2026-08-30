using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.UI
{
    public class UIElement : DrawableEntity
    {
        #region Properties

        public UIElement Parent { get; set; } = null;

        public Vector2 PositionDraw { get; set; } = Vector2.Zero;

        public bool IsFocused { get; set; } = false;

        public bool IsHovered { get; set; } = false;

        public bool IsEnabled { get; set; } = true;

        public bool IsPressed { get; set; } = false;

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
            var hoverRectangle = new Rectangle((int)(X + (Parent?.Position.X ?? 0)), (int)(Y + (Parent?.Position.Y ?? 0)), (int)Width, (int)Height);

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
            var position = new Vector2(X + PositionDraw.X + (Parent?.Position.X ?? 0), Y + PositionDraw.Y + (Parent?.Position.Y ?? 0));

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

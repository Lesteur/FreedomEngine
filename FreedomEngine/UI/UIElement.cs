using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Components;
using FreedomEngine.Core;
using FreedomEngine.Graphics;
using FreedomEngine.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FreedomEngine.UI
{
    /// <summary>
    /// Provides the base functionality for an interactive, positioned element of a user interface:
    /// hover and press detection, parent-relative layout, and click notification.
    /// </summary>
    public abstract class UIElement : DrawableEntity, IUIElement
    {
        #region Fields

        /// <summary>
        /// The element this element is nested under.
        /// </summary>
        private UIElement _parent;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the element this element is nested under, or <see langword="null"/> for none.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// The assigned value is this element itself, or assigning it would create a cycle (an
        /// ancestor of the assigned value is this element).
        /// </exception>
        public UIElement Parent
        {
            get => _parent;
            set
            {
                if (ReferenceEquals(value, this))
                    throw new ArgumentException("An element cannot be its own parent.", nameof(value));

                for (var ancestor = value; ancestor != null; ancestor = ancestor.Parent)
                {
                    if (ReferenceEquals(ancestor, this))
                        throw new ArgumentException("Assigning this parent would create a cycle.", nameof(value));
                }

                _parent = value;
            }
        }

        /// <summary>
        /// Gets or sets the draw-only offset applied on top of <see cref="DrawableEntity.Position"/>.
        /// </summary>
        /// <remarks>
        /// Used to offset the position of the element when drawing, allowing for effects like shaking
        /// or sliding without changing the actual logical position (and therefore without disturbing
        /// hit-testing done against <see cref="PositionTotal"/>).
        /// </remarks>
        public Vector2 PositionDraw { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets the absolute position of this element, with every ancestor's
        /// <see cref="DrawableEntity.Position"/> applied.
        /// </summary>
        public Vector2 PositionTotal => Position + (Parent?.PositionTotal ?? Vector2.Zero);

        /// <summary>
        /// Gets the absolute position this element is rendered at.
        /// </summary>
        /// <remarks>
        /// Includes every ancestor's <see cref="DrawableEntity.Position"/> (via <see cref="PositionTotal"/>),
        /// this element's own <see cref="PositionDraw"/>, and its immediate parent's
        /// <see cref="PositionDraw"/>. Deeper ancestors' draw offsets are not included.
        /// </remarks>
        public Vector2 PositionTotalDraw => PositionTotal + PositionDraw + (Parent?.PositionDraw ?? Vector2.Zero);

        /// <summary>
        /// Gets a value indicating whether this element currently has input focus.
        /// </summary>
        public bool IsFocused { get; protected set; } = false;

        /// <summary>
        /// Gets a value indicating whether the pointer is currently over this element.
        /// </summary>
        public bool IsHovered { get; protected set; } = false;

        /// <summary>
        /// Gets a value indicating whether this element accepts input.
        /// </summary>
        public bool IsEnabled { get; protected set; } = true;

        /// <summary>
        /// Gets a value indicating whether the left mouse button is currently held down on this element.
        /// </summary>
        public bool IsPressed { get; protected set; } = false;

        #endregion

        #region Events

        /// <summary>
        /// Raised when the left mouse button is pressed down and later released while still hovering
        /// this element.
        /// </summary>
        /// <remarks>
        /// Not raised if the pointer leaves the element before the button is released, even if it
        /// returns afterward — the release must happen while <see cref="IsHovered"/> is still true.
        /// </remarks>
        public event Action OnClick;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="UIElement"/> class with the specified sprite and initial position.
        /// </summary>
        /// <param name="sprite">
        /// The sprite associated with the element. May be <see langword="null"/> for an element with
        /// no visual of its own, such as a pure layout container.
        /// </param>
        /// <param name="position">The initial position of the element in 2D space.</param>
        protected UIElement(Sprite sprite, Vector2 position) : base(sprite, position)
        {
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the element's animation, and its hover, press, and click state against the current
        /// mouse position.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouse = Application.Input.Mouse;
            var mousePosition = new Vector2(mouse.Position.X / mouse.UIScale.X, mouse.Position.Y / mouse.UIScale.Y);

            // Hit-test against the same position the element is actually rendered at, so the
            // clickable area never drifts away from what the player sees (e.g. while PositionDraw is
            // animating a hover slide or shake).
            var hitPosition = PositionTotal;
            var hitRectangle = new Rectangle((int)hitPosition.X, (int)hitPosition.Y, (int)Width, (int)Height);

            bool isHovering = hitRectangle.Contains(mousePosition);

            if (isHovering != IsHovered)
                OnHovered(isHovering);

            if (isHovering && mouse.WasButtonJustPressed(MouseButton.Left))
                OnPressed(true);

            if (IsPressed && mouse.WasButtonJustReleased(MouseButton.Left))
            {
                OnPressed(false);

                if (isHovering)
                    OnClick?.Invoke();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Gets the position this element is actually rendered at: <see cref="PositionTotalDraw"/>.
        /// </summary>
        protected override Vector2 RenderPosition => PositionTotalDraw;

        /// <summary>
        /// Called when this element gains or loses input focus.
        /// </summary>
        /// <param name="focus"><see langword="true"/> if the element gained focus; <see langword="false"/> if it lost it.</param>
        /// <remarks>
        /// Nothing in <see cref="UIElement"/> calls this yet; it is a hook for a derived class or a
        /// focus manager to drive.
        /// </remarks>
        protected virtual void OnFocus(bool focus)
        {
            IsFocused = focus;
        }

        /// <summary>
        /// Called when the pointer starts or stops hovering this element.
        /// </summary>
        /// <param name="hovered"><see langword="true"/> if the pointer just entered the element; <see langword="false"/> if it just left.</param>
        protected virtual void OnHovered(bool hovered)
        {
            IsHovered = hovered;
        }

        /// <summary>
        /// Called when this element is enabled or disabled.
        /// </summary>
        /// <param name="enabled"><see langword="true"/> to accept input; <see langword="false"/> to ignore it.</param>
        /// <remarks>
        /// Nothing in <see cref="UIElement"/> calls this yet; it is a hook for a derived class to
        /// drive, and <see cref="Update"/> does not currently check <see cref="IsEnabled"/> before
        /// processing hover and press.
        /// </remarks>
        protected virtual void OnEnabled(bool enabled)
        {
            IsEnabled = enabled;
        }

        /// <summary>
        /// Called when the left mouse button is pressed down or released on this element.
        /// </summary>
        /// <param name="pressed"><see langword="true"/> on press-down; <see langword="false"/> on release.</param>
        /// <remarks>
        /// Only updates <see cref="IsPressed"/>. <see cref="OnClick"/> is raised separately by
        /// <see cref="Update"/>, only on release and only while still hovering.
        /// </remarks>
        protected virtual void OnPressed(bool pressed)
        {
            IsPressed = pressed;
        }

        #endregion
    }
}
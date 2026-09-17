using Microsoft.Xna.Framework;

using FreedomEngine.UI;

namespace FreedomEngine.Collections.Interfaces
{
    /// <summary>
    /// Represents an interactive, positioned element of a user interface.
    /// </summary>
    public interface IUIElement : IDraw
    {
        #region Properties

        /// <summary>
        /// Gets the element this element is nested under, or <see langword="null"/> if it has none.
        /// </summary>
        public UIElement Parent { get; }

        /// <summary>
        /// Gets the logical position of this element, relative to <see cref="Parent"/> if it has one.
        /// </summary>
        public Vector2 Position { get; }

        /// <summary>
        /// Gets the draw-only offset applied on top of <see cref="Position"/>, without affecting layout.
        /// </summary>
        public Vector2 PositionDraw { get; }

        /// <summary>
        /// Gets the absolute position of this element, with every ancestor's <see cref="Position"/> applied.
        /// </summary>
        public Vector2 PositionTotal { get; }

        /// <summary>
        /// Gets the absolute position this element is rendered at, including every ancestor's
        /// <see cref="Position"/> and its own and its immediate parent's <see cref="PositionDraw"/>.
        /// </summary>
        public Vector2 PositionTotalDraw { get; }

        /// <summary>
        /// Gets a value indicating whether this element currently has input focus.
        /// </summary>
        public bool IsFocused { get; }

        /// <summary>
        /// Gets a value indicating whether the pointer is currently over this element.
        /// </summary>
        public bool IsHovered { get; }

        /// <summary>
        /// Gets a value indicating whether this element accepts input.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this element is currently being pressed.
        /// </summary>
        public bool IsPressed { get; }

        #endregion
    }
}
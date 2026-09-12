using FreedomEngine.UI;
using Microsoft.Xna.Framework;

namespace FreedomEngine.Collections.Interfaces
{
    public interface IUIElement : IDraw
    {
        #region Properties

        public UIElement Parent { get; }

        public Vector2 Position { get; }

        public Vector2 PositionDraw { get; }

        public Vector2 PositionTotal { get; }

        public Vector2 PositionTotalDraw { get; }

        public bool IsFocused { get; }

        public bool IsHovered { get; }

        public bool IsEnabled { get; }

        public bool IsPressed { get; }

        #endregion
    }
}

using FreedomEngine.Components;
using FreedomEngine.Graphics;

namespace FreedomEngine.UI
{
    public class UIElement : Entity
    {
        #region Properties

        public bool IsFocused { get; set; }

        public bool IsHovered { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsPressed { get; set; }

        #endregion

        #region Constructors

        public UIElement(Sprite sprite, int x = 0, int y = 0) : base(sprite, x, y)
        {
        }

        #endregion
    }
}

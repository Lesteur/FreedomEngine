using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.UI
{
    public class UIList : UIElement
    {
        #region Fields

        private readonly List<UIElement> _items;

        private float _spacing;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the vertical space between each item in the list.
        /// </summary>
        public float Spacing
        {
            get => _spacing;
            set
            {
                if (_spacing != value)
                {
                    _spacing = value;
                    RefreshLayout();
                }
            }
        }

        #endregion

        #region Constructors

        // Note: Assumes you added a UIElement constructor that doesn't strictly require a Sprite.
        // If not, you can pass null and handle it gracefully in the base class.
        public UIList(Vector2 position, float spacing = 10f) : base(null, position)
        {
            _items = [];
            _spacing = spacing;
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            foreach (var item in _items)
            {
                if (item.IsEnabled)
                {
                    item.Update(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            foreach (var item in _items)
            {
                if (item.Visible)
                {
                    item.Draw(spriteBatch);
                }
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(UIElement item)
        {
            if (item == null || _items.Contains(item))
                return;

            _items.Add(item);
            item.Parent = this; // Set the parent to this list for relative positioning
            RefreshLayout();
        }

        public void RemoveItem(UIElement item)
        {
            if (_items.Remove(item))
            {
                RefreshLayout();
            }
        }

        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Recalculates the position of all child items based on the list's position and spacing.
        /// </summary>
        public void RefreshLayout()
        {
            float currentY = 0;

            foreach (var item in _items)
            {
                // Align item to the list's X position, and stack vertically on Y
                item.Position = new Vector2(0, currentY);

                // Advance the Y position by the item's height and the defined spacing
                // Note: Make sure UIElement has a virtual or calculated Height property!
                currentY += item.Height + _spacing;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.UI
{
    /// <summary>
    /// Represents a purely logical container that stacks child <see cref="UIElement"/> instances
    /// vertically, spaced apart by <see cref="Spacing"/>.
    /// </summary>
    /// <remarks>
    /// A list has no visual of its own — it is constructed with no sprite and never renders a
    /// background — and does not process hover, press, or click for itself; only its children do.
    /// Its own <see cref="Width"/> and <see cref="Height"/> are computed from its children, so a list
    /// nested inside another list lays out correctly.
    /// </remarks>
    public class UIList : UIElement
    {
        #region Fields

        /// <summary>
        /// The child elements of this list, in display order.
        /// </summary>
        private readonly List<UIElement> _items;

        /// <summary>
        /// The vertical space between each item in the list.
        /// </summary>
        private float _spacing;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the vertical space between each item in the list.
        /// </summary>
        /// <remarks>Changing this immediately recomputes every item's position.</remarks>
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

        /// <summary>
        /// Gets the number of items in this list.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Gets the width of this list, computed as the width of its widest item.
        /// </summary>
        public override float Width
        {
            get
            {
                float max = 0f;

                foreach (var item in _items)
                {
                    if (item.Width > max)
                        max = item.Width;
                }

                return max;
            }
        }

        /// <summary>
        /// Gets the height of this list, computed as the sum of every item's height plus the spacing
        /// between them.
        /// </summary>
        public override float Height
        {
            get
            {
                if (_items.Count == 0)
                    return 0f;

                float total = 0f;

                foreach (var item in _items)
                    total += item.Height;

                total += Spacing * (_items.Count - 1);

                return total;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIList"/> class.
        /// </summary>
        /// <param name="position">The initial position of the list.</param>
        /// <param name="spacing">The vertical space between each item in the list.</param>
        public UIList(Vector2 position, float spacing = 10f) : base(null, position)
        {
            _items = [];
            _spacing = spacing;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates every enabled item in this list.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Deliberately does not call the base <see cref="UIElement.Update"/>: a list has no bounds of
        /// its own to hover or click, only its items do.
        /// </remarks>
        public override void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            foreach (var item in _items)
            {
                if (item.IsEnabled)
                    item.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws every visible item in this list.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Deliberately does not call the base <see cref="DrawableEntity.Draw"/>: a list is
        /// constructed with no sprite and has nothing of its own to draw.
        /// </remarks>
        public override void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            if (!Visible)
                return;

            foreach (var item in _items)
            {
                if (item.Visible)
                    item.Draw(spriteBatch);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an item to the end of this list and immediately recomputes the layout.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Adding <paramref name="item"/> would create a parent cycle (see <see cref="UIElement.Parent"/>).</exception>
        /// <remarks>Adding an item that is already in this list has no effect.</remarks>
        public void AddItem(UIElement item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (_items.Contains(item))
                return;

            _items.Add(item);
            item.Parent = this; // Set the parent to this list for relative positioning
            RefreshLayout();
        }

        /// <summary>
        /// Removes an item from this list and immediately recomputes the layout.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <remarks>
        /// Clears the removed item's <see cref="UIElement.Parent"/>, so its position no longer factors
        /// in this list's position. Has no effect if <paramref name="item"/> is not in this list.
        /// </remarks>
        public void RemoveItem(UIElement item)
        {
            if (_items.Remove(item))
            {
                item.Parent = null;
                RefreshLayout();
            }
        }

        /// <summary>
        /// Removes every item from this list.
        /// </summary>
        /// <remarks>Clears each removed item's <see cref="UIElement.Parent"/>, same as <see cref="RemoveItem"/>.</remarks>
        public void Clear()
        {
            foreach (var item in _items)
                item.Parent = null;

            _items.Clear();
        }

        /// <summary>
        /// Recalculates the position of all child items based on the list's spacing.
        /// </summary>
        /// <remarks>
        /// Called automatically whenever the item set or <see cref="Spacing"/> changes; call it
        /// manually after changing an item's own height in a way this list cannot observe.
        /// </remarks>
        public void RefreshLayout()
        {
            float currentY = 0;

            foreach (var item in _items)
            {
                // Align item to the list's X position, and stack vertically on Y
                item.Position = new Vector2(0, currentY);

                // Advance the Y position by the item's height and the defined spacing
                currentY += item.Height + _spacing;
            }
        }

        #endregion
    }
}
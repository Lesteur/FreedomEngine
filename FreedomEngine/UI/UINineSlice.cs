using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Collections.Structures;

namespace FreedomEngine.UI
{
    /// <summary>
    /// Represents a nine-slice (also known as 9-patch) scalable graphic: a source texture region split
    /// into a 3x3 grid whose corners stay fixed size, whose edges stretch along one axis, and whose
    /// center stretches along both, allowing a small source image to scale to any size without
    /// distorting its border.
    /// </summary>
    public class UINineSlice : IUIElement
    {
        #region Fields

        /// <summary>
        /// The nine source regions of the slice, in row-major order (top-left, top, top-right,
        /// left, center, right, bottom-left, bottom, bottom-right).
        /// </summary>
        private readonly Rectangle[] _rectangles;

        /// <summary>
        /// The rendered size of the nine-slice. Backing field for <see cref="Dimensions"/>.
        /// </summary>
        private Vector2Int _dimensions;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the source texture the nine slices are drawn from.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the element this nine-slice is nested under, or <see langword="null"/> for none.
        /// </summary>
        public IUIElement Parent { get; }

        /// <summary>
        /// Gets or sets the position of the nine-slice's top-left corner.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the draw-only offset applied on top of <see cref="Position"/>.
        /// </summary>
        /// <remarks>
        public Vector2 PositionDraw { get; set; } = Vector2.Zero;

        /// <summary>
        /// Gets the absolute position of this element, with every ancestor's
        /// <see cref="Position"/> applied.
        /// </summary>
        public Vector2 PositionTotal => Position + (Parent?.PositionTotal ?? Vector2.Zero);

        /// <summary>
        /// Gets the absolute position this element is rendered at.
        /// </summary>
        public Vector2 PositionTotalDraw => PositionTotal + PositionDraw + (Parent?.PositionDraw ?? Vector2.Zero);

        /// <summary>
        /// Gets or sets the color mask to apply when rendering every slice.
        /// </summary>
        /// <remarks>Default value is Color.White</remarks>
        public Color Color { get; set; } = Color.White;

        /// <summary>
        /// Gets or sets the rendered size of the nine-slice.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The value is smaller than the combined size of the fixed corner slices along either axis,
        /// which would make the stretched edges and center collapse to a negative size.
        /// </exception>
        public Vector2Int Dimensions
        {
            get => _dimensions;
            set
            {
                int minWidth = _rectangles[0].Width + _rectangles[2].Width;
                int minHeight = _rectangles[0].Height + _rectangles[6].Height;

                if (value.X < minWidth || value.Y < minHeight)
                    throw new ArgumentOutOfRangeException(nameof(value), value, $"Dimensions must be at least ({minWidth}, {minHeight}) to fit the corner slices.");

                _dimensions = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this nine-slice currently has input focus.
        /// </summary>
        public bool IsFocused { get; }

        /// <summary>
        /// Gets a value indicating whether the pointer is currently over this nine-slice.
        /// </summary>
        public bool IsHovered { get; }

        /// <summary>
        /// Gets a value indicating whether this nine-slice accepts input.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether this nine-slice is currently being pressed.
        /// </summary>
        public bool IsPressed { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UINineSlice"/> class.
        /// </summary>
        /// <param name="texture">The source texture the nine slices are drawn from.</param>
        /// <param name="rectangle">
        /// The region within <paramref name="texture"/> to split into a 3x3 grid. Its width and height
        /// must each be evenly divisible by 3.
        /// </param>
        /// <param name="position">The initial position of the nine-slice's top-left corner.</param>
        /// <exception cref="ArgumentNullException"><paramref name="texture"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rectangle"/> has a width or height that is not greater than zero, or is not
        /// evenly divisible by 3.
        /// </exception>
        /// <remarks>
        /// <see cref="Dimensions"/> starts equal to <paramref name="rectangle"/>'s own size, so the
        /// nine-slice renders at its source size until <see cref="Dimensions"/> is changed.
        /// </remarks>
        public UINineSlice(Texture2D texture, Rectangle rectangle, Vector2 position)
        {
            ArgumentNullException.ThrowIfNull(texture);

            if (rectangle.Width <= 0 || rectangle.Height <= 0)
                throw new ArgumentException("Rectangle width and height must be greater than zero.", nameof(rectangle));

            if (rectangle.Width % 3 != 0 || rectangle.Height % 3 != 0)
                throw new ArgumentException("Rectangle width and height must be divisible by 3 for nine-slice scaling.", nameof(rectangle));

            Texture = texture;
            Position = position;

            _rectangles = new Rectangle[9];

            int cellWidth = rectangle.Width / 3;
            int cellHeight = rectangle.Height / 3;

            for (int i = 0; i < 9; i++)
            {
                int x = rectangle.X + (i % 3) * cellWidth;
                int y = rectangle.Y + (i / 3) * cellHeight;
                _rectangles[i] = new Rectangle(x, y, cellWidth, cellHeight);
            }

            // Assigned last: the Dimensions setter validates against _rectangles, which must already
            // be populated.
            Dimensions = new Vector2Int(rectangle.Width, rectangle.Height);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the nine-slice.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Currently a no-op: a plain nine-slice has no time-based state. It exists as a hook for a
        /// derived class that animates its slices (a pulsing border, for instance).
        /// </remarks>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);
        }

        /// <summary>
        /// Draws the nine slices, stretched to fit <see cref="Dimensions"/>.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            int leftWidth = _rectangles[0].Width;
            int rightWidth = _rectangles[2].Width;
            int topHeight = _rectangles[0].Height;
            int bottomHeight = _rectangles[6].Height;

            int middleWidth = Dimensions.X - leftWidth - rightWidth;
            int middleHeight = Dimensions.Y - topHeight - bottomHeight;

            // Column widths and row heights, in the same left-to-right, top-to-bottom order as the
            // 3x3 grid in _rectangles. The Dimensions setter guarantees these are never negative.
            Span<int> columnWidths = [ leftWidth, middleWidth, rightWidth ];
            Span<int> rowHeights = [ topHeight, middleHeight, bottomHeight ];

            int destY = (int)Position.Y;

            for (int row = 0; row < 3; row++)
            {
                int destX = (int)Position.X;

                for (int col = 0; col < 3; col++)
                {
                    Rectangle source = _rectangles[row * 3 + col];
                    Rectangle destination = new(destX, destY, columnWidths[col], rowHeights[row]);

                    spriteBatch.Draw(Texture, destination, source, Color);

                    destX += columnWidths[col];
                }

                destY += rowHeights[row];
            }
        }

        #endregion
    }
}
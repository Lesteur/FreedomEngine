using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Core;
using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a grid of tiles for rendering in a 2D space.
    /// </summary>
    /// <remarks>
    /// The map stores one tile ID per cell, resolved against its <see cref="Tileset"/> at draw time.
    /// Animated tiles are resolved through a remap table rebuilt once per frame in <see cref="Update"/>,
    /// so animation cost is proportional to the number of distinct animations rather than to the
    /// number of cells.
    /// </remarks>
    public class Tilemap : IDraw
    {
        #region Fields

        /// <summary>
        /// Represents the tileset associated with this instance.
        /// </summary>
        private readonly Tileset _tileset;

        /// <summary>
        /// Stores the tile IDs for each position in the tilemap grid. Each ID corresponds to an index in the tileset.
        /// </summary>
        private readonly ushort[] _tiles;

        /// <summary>
        /// A lookup table that maps original tile IDs to their current animation frame IDs.
        /// </summary>
        /// <remarks>
        /// Indexed by tile ID, and sized to the tileset's tile count. A non-animated tile maps to
        /// itself, so the table can be applied unconditionally while drawing.
        /// </remarks>
        private readonly ushort[] _animationRemap;

        /// <summary>
        /// Tracks the elapsed time within the current frame of each animated tile ID.
        /// </summary>
        private readonly Dictionary<ushort, TimeSpan> _animationElapsedStates;

        /// <summary>
        /// Tracks the current frame index of each animated tile ID.
        /// </summary>
        private readonly Dictionary<ushort, int> _animationFrameStates;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the camera used to cull tiles that fall outside the view.
        /// </summary>
        /// <remarks>
        /// When <see langword="null"/>, no culling is performed and every tile is drawn.
        /// </remarks>
        public static Camera Camera { get; set; }

        /// <summary>
        /// Gets the total number of rows in this tilemap.
        /// </summary>
        public ushort Rows { get; }

        /// <summary>
        /// Gets the total number of columns in this tilemap.
        /// </summary>
        public ushort Columns { get; }

        /// <summary>
        /// Gets the total number of tiles in this tilemap.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the scale factor to draw each tile at.
        /// </summary>
        public Vector2 Scale { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether this tilemap advances tile animations on update.
        /// </summary>
        /// <remarks>
        /// Initialized from whether the tileset had any animations when this tilemap was created. Set
        /// this to <see langword="true"/> manually after adding animations to the tileset afterwards.
        /// </remarks>
        public bool IsAnimated { get; set; }

        /// <summary>
        /// Gets or sets the position of the tilemap's top-left corner.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or sets the X-coordinate component of the position.
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        /// <summary>
        /// Gets or sets the Y-coordinate component of the position.
        /// </summary>
        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        /// <summary>
        /// Gets the width, in pixels, each tile is drawn at.
        /// </summary>
        public float TileWidth => _tileset.TileWidth * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, each tile is drawn at.
        /// </summary>
        public float TileHeight => _tileset.TileHeight * Scale.Y;

        /// <summary>
        /// Gets or sets the layer depth each tile is drawn at.
        /// </summary>
        /// <remarks>
        /// Defaults to 1.0f, the back of the scene, so entities drawn at a lower depth appear in front
        /// of the map.
        /// </remarks>
        public float LayerDepth { get; set; } = 1.0f;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Tilemap"/> class with the specified tileset and dimensions.
        /// </summary>
        /// <param name="tileset">The tileset used by this tilemap.</param>
        /// <param name="columns">The total number of columns in this tilemap.</param>
        /// <param name="rows">The total number of rows in this tilemap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tileset"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="columns"/> or <paramref name="rows"/> is zero.
        /// </exception>
        public Tilemap(Tileset tileset, ushort columns, ushort rows)
        {
            ArgumentNullException.ThrowIfNull(tileset);

            if (columns == 0)
                throw new ArgumentOutOfRangeException(nameof(columns), columns, "Columns must be greater than zero.");

            if (rows == 0)
                throw new ArgumentOutOfRangeException(nameof(rows), rows, "Rows must be greater than zero.");

            _tileset = tileset;

            Rows = rows;
            Columns = columns;

            // Held as int: the product of two ushorts overflows a ushort well before it overflows an int.
            Count = columns * rows;

            Scale = Vector2.One;
            IsAnimated = _tileset.Animations.Count > 0;

            _tiles = new ushort[Count];

            _animationRemap = new ushort[_tileset.Count];
            _animationElapsedStates = [];
            _animationFrameStates = [];

            ResetAnimationStates();
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Advances every tile animation and rebuilds the frame remap table used when drawing.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        /// <remarks>Does nothing when <see cref="IsAnimated"/> is false.</remarks>
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);

            if (!IsAnimated)
                return;

            foreach (var kvp in _tileset.Animations)
            {
                ushort originalID = kvp.Key;
                var animation = kvp.Value;

                // A tile ID outside the remap table cannot be drawn, so there is nothing to animate.
                if (originalID >= _animationRemap.Length)
                    continue;

                // Animations may have been added to the tileset after this tilemap was built, in which
                // case no state exists for them yet.
                if (!_animationElapsedStates.TryGetValue(originalID, out TimeSpan elapsed))
                {
                    elapsed = TimeSpan.Zero;
                    _animationFrameStates[originalID] = 0;
                }

                // Add elapsed time to this specific animation
                var currentElapsedTime = elapsed + gameTime.ElapsedGameTime;
                int currentFrameIndex = _animationFrameStates[originalID];

                // Determine next frame
                currentFrameIndex = animation.GetNextFrame(currentFrameIndex, currentElapsedTime, out TimeSpan newElapsedTime);

                // Update states
                _animationElapsedStates[originalID] = newElapsedTime;
                _animationFrameStates[originalID] = currentFrameIndex;

                // Pre-calculate the remap table value for rendering
                _animationRemap[originalID] = animation.Frames[currentFrameIndex];
            }
        }

        /// <summary>
        /// Draws this tilemap using the given sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        /// <remarks>Tiles outside the view of <see cref="Camera"/> are skipped.</remarks>
        public void Draw(SpriteBatch spriteBatch)
        {
            ArgumentNullException.ThrowIfNull(spriteBatch);

            for (int i = 0; i < Count; i++)
            {
                int column = i % Columns;
                int row = i / Columns;
                Vector2 position = new(X + column * TileWidth, Y + row * TileHeight);

                if (Camera != null)
                {
                    if (!Camera.IsInView(position, TileWidth, TileHeight))
                        continue;
                }

                ushort tilesetIndex = _animationRemap[_tiles[i]];
                var tile = _tileset.GetTile(tilesetIndex);

                spriteBatch.Draw(_tileset.Texture, position, tile, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, LayerDepth);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the tileset identifier for the tile at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the tile to update.</param>
        /// <param name="tilesetID">The identifier of the tile in the tileset to assign to this cell.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is not less than <see cref="Count"/>, or <paramref name="tilesetID"/>
        /// is not a valid tile in the tileset.
        /// </exception>
        public void SetTile(int index, ushort tilesetID)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Value must be between 0 and {Count - 1}.");

            if (tilesetID >= _tileset.Count)
                throw new ArgumentOutOfRangeException(nameof(tilesetID), tilesetID, $"Value must be less than the tileset tile count ({_tileset.Count}).");

            _tiles[index] = tilesetID;
        }

        /// <summary>
        /// Sets the tile at the specified column and row to use the given tileset identifier.
        /// </summary>
        /// <param name="column">The zero-based column index of the tile to set.</param>
        /// <param name="row">The zero-based row index of the tile to set.</param>
        /// <param name="tilesetID">The identifier of the tile in the tileset to assign to this cell.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="column"/> is not less than <see cref="Columns"/>, <paramref name="row"/> is
        /// not less than <see cref="Rows"/>, or <paramref name="tilesetID"/> is not a valid tile in the tileset.
        /// </exception>
        public void SetTile(int column, int row, ushort tilesetID)
        {
            ValidateCell(column, row);

            SetTile(row * Columns + column, tilesetID);
        }

        /// <summary>
        /// Gets the tileset identifier currently assigned to the tile at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the tile to read.</param>
        /// <returns>The tileset identifier stored in that cell.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not less than <see cref="Count"/>.</exception>
        public ushort GetTileID(int index)
        {
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Value must be between 0 and {Count - 1}.");

            return _tiles[index];
        }

        /// <summary>
        /// Gets the tileset identifier currently assigned to the tile at the specified column and row.
        /// </summary>
        /// <param name="column">The zero-based column index of the tile to read.</param>
        /// <param name="row">The zero-based row index of the tile to read.</param>
        /// <returns>The tileset identifier stored in that cell.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="column"/> is not less than <see cref="Columns"/>, or <paramref name="row"/>
        /// is not less than <see cref="Rows"/>.
        /// </exception>
        public ushort GetTileID(int column, int row)
        {
            ValidateCell(column, row);

            return _tiles[row * Columns + column];
        }

        /// <summary>
        /// Retrieves the texture region associated with the specified tile index.
        /// </summary>
        /// <param name="index">The zero-based index of the tile to retrieve.</param>
        /// <returns>The texture region for the tile stored in that cell, ignoring any animation remapping.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not less than <see cref="Count"/>.</exception>
        public Rectangle GetTile(int index)
        {
            return _tileset.GetTile(GetTileID(index));
        }

        /// <summary>
        /// Fills every cell of this tilemap with the given tileset identifier.
        /// </summary>
        /// <param name="tilesetID">The identifier of the tile in the tileset to assign to every cell.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="tilesetID"/> is not a valid tile in the tileset.</exception>
        public void Fill(ushort tilesetID)
        {
            if (tilesetID >= _tileset.Count)
                throw new ArgumentOutOfRangeException(nameof(tilesetID), tilesetID, $"Value must be less than the tileset tile count ({_tileset.Count}).");

            Array.Fill(_tiles, tilesetID);
        }

        /// <summary>
        /// Rebuilds the animation state of every animated tile, restarting each animation from its first frame.
        /// </summary>
        /// <remarks>
        /// Call this after adding animations to the tileset so that the new animations are tracked, and
        /// set <see cref="IsAnimated"/> accordingly.
        /// </remarks>
        public void ResetAnimationStates()
        {
            _animationElapsedStates.Clear();
            _animationFrameStates.Clear();

            for (int i = 0; i < _animationRemap.Length; i++)
            {
                _animationRemap[i] = (ushort)i;

                if (_tileset.Animations.ContainsKey((ushort)i))
                {
                    _animationElapsedStates[(ushort)i] = TimeSpan.Zero;
                    _animationFrameStates[(ushort)i] = 0;
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Validates that a column and row pair falls inside this tilemap's grid.
        /// </summary>
        /// <param name="column">The zero-based column index to validate.</param>
        /// <param name="row">The zero-based row index to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="column"/> is not less than <see cref="Columns"/>, or <paramref name="row"/>
        /// is not less than <see cref="Rows"/>.
        /// </exception>
        private void ValidateCell(int column, int row)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column), column, $"Value must be between 0 and {Columns - 1}.");

            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row), row, $"Value must be between 0 and {Rows - 1}.");
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Collections.Interfaces;
using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a grid of tiles for rendering in a 2D space.
    /// </summary>
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
        private readonly ushort[] _animationRemap;

        // Tracks elapsed time and frame states uniquely for each animated tile ID
        private readonly Dictionary<ushort, TimeSpan> _animationElapsedStates;
        private readonly Dictionary<ushort, int> _animationFrameStates;

        #endregion

        #region Properties

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
        public ushort Count { get; }

        /// <summary>
        /// Gets the scale factor to draw each tile at.
        /// </summary>
        public Vector2 Scale { get; init; }

        /// <summary>
        /// Gets or Sets a value indicating whether this tilemap contains any animated tiles.
        /// </summary>
        public bool IsAnimated { get; set; }

        /// <summary>
        /// Gets or Sets the position of the tilemap.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Gets or Sets the X-coordinate component of the position.
        /// </summary>
        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        /// <summary>
        /// Gets or Sets the Y-coordinate component of the position.
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

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="Tilemap"/> class with the specified tileset and dimensions.
        /// </summary>
        /// <param name="tileset">The tileset used by this tilemap.</param>
        /// <param name="columns">The total number of columns in this tilemap.</param>
        /// <param name="rows">The total number of rows in this tilemap.</param>
        public Tilemap(Tileset tileset, ushort columns, ushort rows)
        {
            _tileset = tileset;

            Rows = rows;
            Columns = columns;
            Count = (ushort)(Columns * Rows);
            Scale = Vector2.One;
            IsAnimated = _tileset.Animations.Count > 0;

            _tiles = new ushort[Count];

            _animationRemap = new ushort[_tileset.Count];
            _animationElapsedStates = new Dictionary<ushort, TimeSpan>();
            _animationFrameStates = new Dictionary<ushort, int>();

            ResetAnimationStates();
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the animation timer and pre-calculates the frame remap table.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!IsAnimated)
                return;

            foreach (var kvp in _tileset.Animations)
            {
                ushort originalID = kvp.Key;
                var animation = kvp.Value;

                // Add elapsed time to this specific animation
                var currentElapsedTime = _animationElapsedStates[originalID] + gameTime.ElapsedGameTime;
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
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Count; i++)
            {
                ushort tilesetIndex = _animationRemap[_tiles[i]];
                var tile = _tileset.GetTile(tilesetIndex);

                int column = i % Columns;
                int row = i / Columns;

                Vector2 position = new(X + column * TileWidth, Y + row * TileHeight);

                tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the tileset identifier for the tile at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the tile to update. Must be within the valid range of tile indices.</param>
        /// <param name="tilesetID">The identifier of the tileset to assign to the specified tile.</param>
        public void SetTile(ushort index, ushort tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        /// <summary>
        /// Sets the tile at the specified column and row to use the given tileset identifier.
        /// </summary>
        /// <param name="column">The zero-based column index of the tile to set. Must be less than the total number of columns.</param>
        /// <param name="row">The zero-based row index of the tile to set. Must be less than the total number of rows.</param>
        /// <param name="tilesetID">The identifier of the tileset to assign to the specified tile.</param>
        public void SetTile(ushort column, ushort row, ushort tilesetID)
        {
            int index = row * Columns + column;
            SetTile((ushort)index, tilesetID);
        }

        /// <summary>
        /// Retrieves the texture region associated with the specified tile index.
        /// </summary>
        /// <param name="index">The index of the tile to retrieve. Must be a valid index within the tile collection.</param>
        /// <returns>The texture region corresponding to the specified tile index.</returns>
        public TextureRegion GetTile(ushort index)
        {
            return _tileset.GetTile(_tiles[index]);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes or resets the animation states for all tiles in the tilemap.
        /// </summary>
        private void ResetAnimationStates()
        {
            for (ushort i = 0; i < _animationRemap.Length; i++)
            {
                _animationRemap[i] = i;

                if (_tileset.Animations.ContainsKey(i))
                {
                    _animationElapsedStates[i] = TimeSpan.Zero;
                    _animationFrameStates[i] = 0;
                }
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a collection of tiles, including their texture regions and animations, used for
    /// rendering a tile-based map in a grid layout.
    /// </summary>
    public sealed class Tileset
    {
        #region Properties

        /// <summary>
        /// Gets the source texture that the tileset's tiles are drawn from.
        /// </summary>
        public Texture2D Texture { get; }

        /// <summary>
        /// Gets the texture regions, within <see cref="Texture"/>, that represent the individual tiles
        /// in this tileset.
        /// </summary>
        public Rectangle[] Tiles { get; }

        /// <summary>
        /// Gets the tile animations in this tileset, mapped by the ID of the tile they animate.
        /// </summary>
        public Dictionary<ushort, TileAnimation> Animations { get; }

        /// <summary>
        /// Gets the width, in pixels, of a single tile.
        /// </summary>
        public ushort TileWidth { get; }

        /// <summary>
        /// Gets the height, in pixels, of a single tile.
        /// </summary>
        public ushort TileHeight { get; }

        /// <summary>
        /// Gets the total number of columns in this tileset.
        /// </summary>
        public ushort Columns { get; }

        /// <summary>
        /// Gets the total number of rows in this tileset.
        /// </summary>
        public ushort Rows { get; }

        /// <summary>
        /// Gets the total number of tiles within the tileset.
        /// </summary>
        public ushort Count { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Tileset"/> class using the specified texture,
        /// tile regions, and animations.
        /// </summary>
        /// <param name="texture">The source texture the tiles are drawn from.</param>
        /// <param name="tiles">
        /// The texture regions that represent the individual tiles. All tiles are assumed to be the
        /// same size as <c>tiles[0]</c>.
        /// </param>
        /// <param name="animations">
        /// The tile animations, mapped by tile ID. Pass <see langword="null"/> to start with no animations.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="texture"/> or <paramref name="tiles"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="tiles"/> is empty, or <c>tiles[0]</c> has a width or height that is not
        /// greater than zero.
        /// </exception>
        public Tileset(Texture2D texture, Rectangle[] tiles, Dictionary<ushort, TileAnimation> animations)
        {
            ArgumentNullException.ThrowIfNull(texture);
            ArgumentNullException.ThrowIfNull(tiles);

            if (tiles.Length == 0)
                throw new ArgumentException("Tiles collection cannot be empty.", nameof(tiles));

            if (tiles[0].Width <= 0 || tiles[0].Height <= 0)
                throw new ArgumentException("Tile width and height must be greater than zero.", nameof(tiles));

            Texture = texture;
            Tiles = tiles;
            TileWidth = (ushort)tiles[0].Width;
            TileHeight = (ushort)tiles[0].Height;
            Columns = (ushort)(texture.Width / TileWidth);
            Rows = (ushort)(texture.Height / TileHeight);
            Count = (ushort)tiles.Length;
            Animations = animations ?? new Dictionary<ushort, TileAnimation>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the texture region for the tile at the given index in this tileset.
        /// </summary>
        /// <param name="index">The index of the tile in this tileset.</param>
        /// <returns>The texture region for the tile at <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is outside the bounds of the tileset.
        /// </exception>
        public Rectangle GetTile(int index)
        {
            if (index < 0 || index >= Tiles.Length)
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Value must be between 0 and {Tiles.Length - 1}.");

            return Tiles[index];
        }

        /// <summary>
        /// Gets the texture region for the tile at the given column and row in this tileset.
        /// </summary>
        /// <param name="column">The zero-based column of the tile.</param>
        /// <param name="row">The zero-based row of the tile.</param>
        /// <returns>The texture region for the tile at the given location.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="column"/> is not less than <see cref="Columns"/>, <paramref name="row"/> is
        /// not less than <see cref="Rows"/>, or the resulting tile is outside the bounds of the tileset.
        /// </exception>
        public Rectangle GetTile(int column, int row)
        {
            if (column < 0 || column >= Columns)
                throw new ArgumentOutOfRangeException(nameof(column), column, $"Value must be between 0 and {Columns - 1}.");

            if (row < 0 || row >= Rows)
                throw new ArgumentOutOfRangeException(nameof(row), row, $"Value must be between 0 and {Rows - 1}.");

            int index = row * Columns + column;
            return GetTile(index);
        }

        /// <summary>
        /// Adds or replaces a tile animation, using a single delay shared by every frame, for the
        /// specified tile ID.
        /// </summary>
        /// <param name="tileID">The ID of the tile the animation applies to.</param>
        /// <param name="frameTilesIDs">The ordered tile IDs that make up the frames of the animation.</param>
        /// <param name="delay">The amount of time to wait before advancing to the next frame.</param>
        /// <exception cref="ArgumentNullException"><paramref name="frameTilesIDs"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="frameTilesIDs"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="delay"/> is negative.</exception>
        /// <remarks>If an animation is already registered for <paramref name="tileID"/>, it is replaced.</remarks>
        public void AddAnimation(ushort tileID, ushort[] frameTilesIDs, TimeSpan delay)
        {
            Animations[tileID] = new TileAnimation(frameTilesIDs, delay);
        }

        /// <summary>
        /// Adds or replaces a tile animation, using an individual delay for each frame, for the
        /// specified tile ID.
        /// </summary>
        /// <param name="tileID">The ID of the tile the animation applies to.</param>
        /// <param name="frameTilesIDs">The ordered tile IDs that make up the frames of the animation.</param>
        /// <param name="delays">
        /// The per-frame delays. Must contain the same number of elements as <paramref name="frameTilesIDs"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="frameTilesIDs"/> or <paramref name="delays"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="frameTilesIDs"/> is empty, or <paramref name="delays"/> does not contain the
        /// same number of elements as <paramref name="frameTilesIDs"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="delays"/> contains a negative value.
        /// </exception>
        /// <remarks>If an animation is already registered for <paramref name="tileID"/>, it is replaced.</remarks>
        public void AddAnimation(ushort tileID, ushort[] frameTilesIDs, TimeSpan[] delays)
        {
            Animations[tileID] = new TileAnimation(frameTilesIDs, delays);
        }

        /// <summary>
        /// Removes the tile animation registered for the specified tile ID, if any.
        /// </summary>
        /// <param name="tileID">The ID of the tile whose animation should be removed.</param>
        /// <returns><see langword="true"/> if an animation was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool RemoveAnimation(ushort tileID)
        {
            return Animations.Remove(tileID);
        }

        #endregion
    }
}
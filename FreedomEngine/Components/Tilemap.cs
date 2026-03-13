using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    /// <summary>
    /// Represents a grid of tiles for rendering in a 2D space.
    /// </summary>
    public class Tilemap
    {
        /// <summary>
        /// Represents the collection of tiles used for rendering by this instance.
        /// </summary>
        private readonly Tileset _tileset;

        /// <summary>
        /// Represents the elapsed time since the start of the operation.
        /// </summary>
        private TimeSpan _elapsed;

        /// <summary>
        /// The array of tileset ids for each tile in this tilemap.
        /// </summary>
        private readonly UInt16[] _tiles;

        /// <summary>
        /// Gets the total number of rows in this tilemap.
        /// </summary>
        public UInt16 Rows { get; }

        /// <summary>
        /// Gets the total number of columns in this tilemap.
        /// </summary>
        public UInt16 Columns { get; }

        /// <summary>
        /// Gets the total number of tiles in this tilemap.
        /// </summary>
        public UInt16 Count { get; }

        /// <summary>
        /// Gets or Sets the scale factor to draw each tile at.
        /// </summary>
        public Vector2 Scale { get; }

        /// <summary>
        /// Gets the width, in pixels, each tile is drawn at.
        /// </summary>
        public float TileWidth => _tileset.TileWidth * Scale.X;

        /// <summary>
        /// Gets the height, in pixels, each tile is drawn at.
        /// </summary>
        public float TileHeight => _tileset.TileHeight * Scale.Y;

        /// <summary>
        /// Gets or Sets a value indicating whether this tilemap contains any animated tiles.
        /// </summary>
        bool IsAnimated { get; set; }


        /// <summary>
        /// Creates a new tilemap.
        /// </summary>
        /// <param name="tileset">The tileset used by this tilemap.</param>
        /// <param name="columns">The total number of columns in this tilemap.</param>
        /// <param name="rows">The total number of rows in this tilemap.</param>
        public Tilemap(Tileset tileset, UInt16 columns, UInt16 rows)
        {
            _tileset = tileset;
            _elapsed = TimeSpan.Zero;

            Rows = rows;
            Columns = columns;
            Count = (UInt16) (Columns * Rows);
            Scale = Vector2.One;
            IsAnimated = (_tileset.Animations.Count > 0);

            _tiles = new UInt16[Count];
        }


        /// <summary>
        /// Sets the tile at the given index in this tilemap to use the tile from
        /// the tileset at the specified tileset id.
        /// </summary>
        /// <param name="index">The index of the tile in this tilemap.</param>
        /// <param name="tilesetID">The tileset id of the tile from the tileset to use.</param>
        public void SetTile(UInt16 index, UInt16 tilesetID)
        {
            _tiles[index] = tilesetID;
        }

        /// <summary>
        /// Sets the tile at the given column and row in this tilemap to use the tile
        /// from the tileset at the specified tileset id.
        /// </summary>
        /// <param name="column">The column of the tile in this tilemap.</param>
        /// <param name="row">The row of the tile in this tilemap.</param>
        /// <param name="tilesetID">The tileset id of the tile from the tileset to use.</param>
        public void SetTile(UInt16 column, UInt16 row, UInt16 tilesetID)
        {
            UInt16 index = (UInt16) (row * Columns + column);
            SetTile(index, tilesetID);
        }

        /// <summary>
        /// Gets the texture region of the tile from this tilemap at the specified index.
        /// </summary>
        /// <param name="index">The index of the tile in this tilemap.</param>
        /// <returns>The texture region of the tile from this tilemap at the specified index.</returns>
        public TextureRegion GetTile(UInt16 index)
        {
            return _tileset.GetTile(_tiles[index]);
        }

        /// <summary>
        /// Gets the texture region of the tile from this tilemap at the specified
        /// column and row.
        /// </summary>
        /// <param name="column">The column of the tile in this tilemap.</param>
        /// <param name="row">The row of the tile in this tilemap.</param>
        /// <returns>The texture region of the tile from this tilemap at the specified column and row.</returns>
        public TextureRegion GetTile(UInt16 column, UInt16 row)
        {
            UInt16 index = (UInt16) (row * Columns + column);
            return GetTile(index);
        }

        public void Update(GameTime gameTime)
        {
            if (!IsAnimated)
                return;

            _elapsed += gameTime.ElapsedGameTime;
        }

        /// <summary>
        /// Draws this tilemap using the given sprite batch.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw this tilemap.</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Count; i++)
            {
                UInt16 tileIndex = _tiles[i];
                TextureRegion tile = _tileset.GetTile(_tileset.TryGetAnimation(tileIndex, _elapsed)); //_tileset.GetTile(tilesetIndex);

                int x = i % Columns;
                int y = i / Columns;

                Vector2 position = new(x * TileWidth, y * TileHeight);
                tile.Draw(spriteBatch, position, Color.White, 0.0f, Vector2.Zero, Scale, SpriteEffects.None, 1.0f);
            }
        }
    }
}

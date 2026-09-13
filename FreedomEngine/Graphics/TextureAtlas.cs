using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Represents a named collection of <see cref="Sprite"/> instances that share a common source texture.
    /// </summary>
    public sealed class TextureAtlas
    {
        #region Fields

        /// <summary>
        /// Stores the mapping of sprite names to their corresponding <see cref="Sprite"/> instances.
        /// </summary>
        private readonly Dictionary<string, Sprite> _sprites;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source texture represented by this texture atlas.
        /// </summary>
        public Texture2D Texture { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAtlas"/> class using the specified
        /// source texture.
        /// </summary>
        /// <param name="texture">The source texture to be used for this texture atlas.</param>
        /// <exception cref="ArgumentNullException"><paramref name="texture"/> is <see langword="null"/>.</exception>
        public TextureAtlas(Texture2D texture)
        {
            ArgumentNullException.ThrowIfNull(texture);

            Texture = texture;
            _sprites = [];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the given sprite to this texture atlas under the specified name.
        /// </summary>
        /// <param name="name">The name to register the sprite under. Must be unique within this atlas.</param>
        /// <param name="sprite">The sprite to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> or <paramref name="sprite"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is empty, or an entry with the same name has already been added.
        /// </exception>
        public void AddSprite(string name, Sprite sprite)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentNullException.ThrowIfNull(sprite);

            _sprites.Add(name, sprite);
        }

        /// <summary>
        /// Gets the sprite from this texture atlas with the specified name.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to retrieve.</param>
        /// <returns>The sprite registered under <paramref name="spriteName"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="spriteName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="spriteName"/> is empty.</exception>
        /// <exception cref="KeyNotFoundException">No sprite is registered under <paramref name="spriteName"/>.</exception>
        public Sprite GetSprite(string spriteName)
        {
            ArgumentException.ThrowIfNullOrEmpty(spriteName);

            return _sprites[spriteName];
        }

        /// <summary>
        /// Attempts to get the sprite from this texture atlas with the specified name, without throwing
        /// if it is not found.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to retrieve.</param>
        /// <param name="sprite">
        /// When this method returns, contains the sprite registered under <paramref name="spriteName"/>,
        /// or <see langword="null"/> if no such sprite exists.
        /// </param>
        /// <returns><see langword="true"/> if a sprite was found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="spriteName"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="spriteName"/> is empty.</exception>
        public bool TryGetSprite(string spriteName, out Sprite sprite)
        {
            ArgumentException.ThrowIfNullOrEmpty(spriteName);

            return _sprites.TryGetValue(spriteName, out sprite);
        }

        /// <summary>
        /// Removes the sprite with the specified name from this texture atlas.
        /// </summary>
        /// <param name="spriteName">The name of the sprite to remove.</param>
        /// <returns><see langword="true"/> if the sprite was found and removed; otherwise, <see langword="false"/>.</returns>
        public bool RemoveSprite(string spriteName)
        {
            return _sprites.Remove(spriteName);
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Removes all sprites from this texture atlas.
        /// </summary>
        public void Clear()
        {
            _sprites.Clear();
        }

        #endregion
    }
}
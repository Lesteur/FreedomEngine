using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    public class TextureAtlas
    {
        /// <summary>
        /// Stores the mapping of texture region names to their corresponding TextureRegion objects.
        /// </summary>
        private readonly Dictionary<string, TextureRegion> _regions;

        /// <summary>
        /// Stores the mapping of animation names to their corresponding Animation objects.
        /// </summary>
        private readonly Dictionary<string, Sprite> _animations;


        /// <summary>
        /// Gets or Sets the source texture represented by this texture atlas.
        /// </summary>
        public Texture2D Texture { get; set; }


        /// <summary>
        /// Creates a new texture atlas.
        /// </summary>
        public TextureAtlas()
        {
            _regions = [];
            _animations = [];
        }

        /// <summary>
        /// Creates a new texture atlas instance using the given texture.
        /// </summary>
        /// <param name="texture">The source texture represented by the texture atlas.</param>
        public TextureAtlas(Texture2D texture)
        {
            Texture = texture;
            _regions = [];
            _animations = [];
        }


        /// <summary>
        /// Creates a new region and adds it to this texture atlas.
        /// </summary>
        /// <param name="name">The name to give the texture region.</param>
        /// <param name="x">The top-left x-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
        /// <param name="y">The top-left y-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
        /// <param name="width">The width, in pixels, of the region.</param>
        /// <param name="height">The height, in pixels, of the region.</param>
        public void AddRegion(string name, UInt16 x, UInt16 y, UInt16 width, UInt16 height)
        {
            TextureRegion region = new(Texture, x, y, width, height);
            _regions.Add(name, region);
        }

        /// <summary>
        /// Gets the region from this texture atlas with the specified name.
        /// </summary>
        /// <param name="name">The name of the region to retrieve.</param>
        /// <returns>The TextureRegion with the specified name.</returns>
        public TextureRegion GetRegion(string name)
        {
            return _regions[name];
        }

        /// <summary>
        /// Removes the region from this texture atlas with the specified name.
        /// </summary>
        /// <param name="name">The name of the region to remove.</param>
        /// <returns></returns>
        public bool RemoveRegion(string name)
        {
            return _regions.Remove(name);
        }

        /// <summary>
        /// Adds the given animation to this texture atlas with the specified name.
        /// </summary>
        /// <param name="animationName">The name of the animation to add.</param>
        /// <param name="animation">The animation to add.</param>
        public void AddAnimation(string animationName, Sprite animation)
        {
            _animations.Add(animationName, animation);
        }

        /// <summary>
        /// Gets the animation from this texture atlas with the specified name.
        /// </summary>
        /// <param name="animationName">The name of the animation to retrieve.</param>
        /// <returns>The animation with the specified name.</returns>
        public Sprite GetAnimation(string animationName)
        {
            return _animations[animationName];
        }

        /// <summary>
        /// Removes the animation with the specified name from this texture atlas.
        /// </summary>
        /// <param name="animationName">The name of the animation to remove.</param>
        /// <returns>true if the animation is removed successfully; otherwise, false.</returns>
        public bool RemoveAnimation(string animationName)
        {
            return _animations.Remove(animationName);
        }

        /// <summary>
        /// Removes all regions from this texture atlas.
        /// </summary>
        public void Clear()
        {
            _regions.Clear();
        }
    }
}

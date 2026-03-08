using System;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using FreedomEngine.Resources;

namespace FreedomEngine.Graphics
{
    public sealed class Texture : Resource<Texture2D>
    {
        private readonly UInt16 _width;

        private readonly UInt16 _height;


        public Texture2D NativeTexture => _native;

        public UInt16 Width => _width;

        public UInt16 Height => _height;


        public Texture(ContentManager contentManager, string name) : base(contentManager.Load<Texture2D>(name))
        {
            // Store dimensions as UInt16 for compactness
            _width = (UInt16) _native.Width;
            _height = (UInt16) _native.Height;
        }

        public Texture(Texture2D texture) : base(texture)
        {
            // Store dimensions as UInt16 for compactness
            _width = (UInt16) texture.Width;
            _height = (UInt16) texture.Height;
        }


        protected override void DisposeNativeResource()
        {
            // Do not dispose: managed by ContentManager
            // If you need to dispose dynamic textures, check a flag here
        }
    }
}
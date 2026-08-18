using Microsoft.Xna.Framework.Content;
using System;

namespace FreedomEngine.Collections.Interfaces
{
    public interface ILoadContent
    {
        public static abstract void LoadContent(ContentManager Content);

        public static abstract void UnloadContent();
    }
}
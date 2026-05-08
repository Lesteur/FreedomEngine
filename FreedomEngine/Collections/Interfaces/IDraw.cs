using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Collections.Interfaces
{
    public interface IDraw : IUpdate
    {
        void Draw(SpriteBatch spriteBatch);
    }
}

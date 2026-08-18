using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Collections.Interfaces
{
    public interface IDraw : IUpdate
    {
        public void Draw(SpriteBatch spriteBatch);
    }
}

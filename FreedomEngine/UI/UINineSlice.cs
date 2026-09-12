using FreedomEngine.Collections.Structures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.UI
{
    public class UINineSlice
    {
        #region Fields

        private readonly Rectangle[] _rectangles;

        #endregion

        #region Properties

        public Texture2D Texture;

        public Vector2 Position;

        public Vector2Int Dimensions { get; set; }

        #endregion

        #region Constructors

        public UINineSlice(Texture2D texture, Rectangle rectangle, Vector2 position)
        {
            ArgumentNullException.ThrowIfNull(texture, "Texture cannot be null.");

            if (rectangle.Width % 3 != 0 || rectangle.Height % 3 != 0)
                throw new ArgumentException("Rectangle width and height must be divisible by 3 for nine-slice scaling.");

            Texture = texture;
            Position = position;

            _rectangles = new Rectangle[9];

            for (int i = 0; i < 9; i++)
            {
                int x = rectangle.X + (i % 3) * (rectangle.Width / 3);
                int y = rectangle.Y + (i / 3) * (rectangle.Height / 3);
                _rectangles[i] = new Rectangle(x, y, rectangle.Width / 3, rectangle.Height / 3);
            }

            Dimensions = new Vector2Int(rectangle.Width, rectangle.Height);
        }

        #endregion

        #region Lifecycle Methods

        public void Update(GameTime gameTime)
        {
            // Update logic if needed
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var width = Dimensions.X - _rectangles[0].Width * 2;
            var height = Dimensions.Y - _rectangles[0].Height * 2;

            var rect1 = _rectangles[0];
            var destRect1 = new Rectangle((int)Position.X, (int)Position.Y, rect1.Width, rect1.Height);
            spriteBatch.Draw(Texture, destRect1, rect1, Color.White);

            var rect2 = _rectangles[1];
            var destRect2 = new Rectangle((int)Position.X + rect1.Width, (int)Position.Y, width, rect2.Height);
            spriteBatch.Draw(Texture, destRect2, rect2, Color.White);

            var rect3 = _rectangles[2];
            var destRect3 = new Rectangle((int)Position.X + rect1.Width + width, (int)Position.Y, rect3.Width, rect3.Height);
            spriteBatch.Draw(Texture, destRect3, rect3, Color.White);

            var rect4 = _rectangles[3];
            var destRect4 = new Rectangle((int)Position.X, (int)Position.Y + rect1.Height, rect4.Width, height);
            spriteBatch.Draw(Texture, destRect4, rect4, Color.White);

            var rect5 = _rectangles[4];
            var destRect5 = new Rectangle((int)Position.X + rect1.Width, (int)Position.Y + rect1.Height, width, height);
            spriteBatch.Draw(Texture, destRect5, rect5, Color.White);

            var rect6 = _rectangles[5];
            var destRect6 = new Rectangle((int)Position.X + rect1.Width + width, (int)Position.Y + rect1.Height, rect6.Width, height);
            spriteBatch.Draw(Texture, destRect6, rect6, Color.White);

            var rect7 = _rectangles[6];
            var destRect7 = new Rectangle((int)Position.X, (int)Position.Y + rect1.Height + height, rect7.Width, rect7.Height);
            spriteBatch.Draw(Texture, destRect7, rect7, Color.White);

            var rect8 = _rectangles[7];
            var destRect8 = new Rectangle((int)Position.X + rect1.Width, (int)Position.Y + rect1.Height + height, width, rect8.Height);
            spriteBatch.Draw(Texture, destRect8, rect8, Color.White);

            var rect9 = _rectangles[8];
            var destRect9 = new Rectangle((int)Position.X + rect1.Width + width, (int)Position.Y + rect1.Height + height, rect9.Width, rect9.Height);
            spriteBatch.Draw(Texture, destRect9, rect9, Color.White);
        }

        #endregion
    }
}

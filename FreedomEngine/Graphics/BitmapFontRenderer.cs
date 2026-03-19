using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.Graphics
{
    /// <summary>
    /// Provides helper methods to render BitmapFont instances using a SpriteBatch.
    /// </summary>
    public static class BitmapFontRenderer
    {
        /// <summary>
        /// Draws a string using a BitmapFont.
        /// </summary>
        public static void DrawString(
            SpriteBatch spriteBatch,
            BitmapFont font,
            string text,
            Vector2 position,
            Color color)
        {
            if (string.IsNullOrEmpty(text) || font == null)
                return;

            foreach (var glyph in font.GetGlyphs(text, position))
            {
                if (glyph.Character == null)
                    continue;

                var region = glyph.Character.TextureRegion;

                spriteBatch.Draw(
                    region.Texture,
                    glyph.Position,
                    region.SourceRectangle,
                    color
                );
            }
        }

        /// <summary>
        /// Draws a StringBuilder using a BitmapFont.
        /// </summary>
        public static void DrawString(
            SpriteBatch spriteBatch,
            BitmapFont font,
            StringBuilder text,
            Vector2 position,
            Color color)
        {
            if (text == null || text.Length == 0 || font == null)
                return;

            foreach (var glyph in font.GetGlyphs(text, position))
            {
                if (glyph.Character == null)
                    continue;

                var region = glyph.Character.TextureRegion;

                spriteBatch.Draw(
                    region.Texture,
                    glyph.Position,
                    region.SourceRectangle,
                    color
                );
            }
        }
    }
}
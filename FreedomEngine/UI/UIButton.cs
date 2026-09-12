using FreedomEngine.Components;
using FreedomEngine.Graphics;
using FreedomEngine.Graphics.BitmapFonts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FreedomEngine.UI
{
    public class UIButton : UIElement
    {
        #region Fields

        protected Text _textComponent;

        protected Vector2 _textPadding;

        #endregion

        #region Properties

        public string Text
        {
            get => _textComponent?.TextString;
            set
            {
                if (_textComponent != null)
                {
                    _textComponent.TextString = value;
                }
            }
        }

        #endregion

        #region Constructors

        public UIButton(Sprite backgroundSprite, Vector2 position, string text, BitmapFont font) : base(backgroundSprite, position)
        {
            _textComponent = new(font, text, new Vector2(0, 0))
            {
                VerticalAlignment = TextVerticalAlignment.Middle,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                MaxWidth = 300,
                JumpHeight = 25
            };

            _textPadding = new Vector2(backgroundSprite.Width / 2, backgroundSprite.Height / 2);
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the text logic (for shake/wave animations)
            _textComponent?.Update(gameTime);

            // Ensure the text position always follows the button's position (useful for your hover Tween)
            if (_textComponent != null)
            {
                _textComponent.Position = PositionTotalDraw + _textPadding;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background sprite (handled by base Entity/UIElement logic)
            base.Draw(spriteBatch);

            // Draw the rich text on top
            _textComponent?.Draw(spriteBatch);
        }

        #endregion

        #region Protected Methods

        protected override void OnPressed(bool pressed)
        {
            base.OnPressed(pressed);
        }

        #endregion
    }
}
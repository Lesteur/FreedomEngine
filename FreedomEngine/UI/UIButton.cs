using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Components;
using FreedomEngine.Graphics;
using FreedomEngine.Graphics.BitmapFonts;

namespace FreedomEngine.UI
{
    /// <summary>
    /// Represents a clickable UI button: a background sprite with a centered label drawn on top.
    /// </summary>
    public class UIButton : UIElement
    {
        #region Fields

        /// <summary>
        /// The text drawn centered over the button's background.
        /// </summary>
        protected Text _textComponent;

        /// <summary>
        /// The offset from the button's top-left corner to its background's center, used to position
        /// <see cref="_textComponent"/> so it lands in the middle of the button.
        /// </summary>
        protected Vector2 _textPadding;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the button's label text.
        /// </summary>
        /// <remarks>
        /// May contain the same inline markup <c>Text</c> supports elsewhere in the engine (color,
        /// scale, shake, wave, rainbow). Setting this while <see cref="_textComponent"/> has been
        /// nulled out by a derived class has no effect.
        /// </remarks>
        public string Text
        {
            get => _textComponent?.TextString;
            set
            {
                if (_textComponent != null)
                    _textComponent.TextString = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UIButton"/> class.
        /// </summary>
        /// <param name="backgroundSprite">The sprite drawn as the button's background.</param>
        /// <param name="position">The initial position of the button.</param>
        /// <param name="text">The button's label text. May contain inline markup.</param>
        /// <param name="font">The bitmap font used to render <paramref name="text"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="backgroundSprite"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Unlike the base <see cref="UIElement"/>, a button requires a background sprite: its size
        /// determines where the label is centered and where it wraps.
        /// </remarks>
        public UIButton(Sprite backgroundSprite, Vector2 position, string text, BitmapFont font) : base(backgroundSprite, position)
        {
            ArgumentNullException.ThrowIfNull(backgroundSprite);

            _textComponent = new(font, text, Vector2.Zero)
            {
                VerticalAlignment = TextVerticalAlignment.Middle,
                HorizontalAlignment = TextHorizontalAlignment.Center,
                MaxWidth = (int)backgroundSprite.Width,
                JumpHeight = 15
            };

            _textPadding = new Vector2(backgroundSprite.Width / 2f, backgroundSprite.Height / 2f);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the button and repositions its label to follow the button.
        /// </summary>
        /// <param name="gameTime">A snapshot of the game's timing values.</param>
        /// <exception cref="ArgumentNullException"><paramref name="gameTime"/> is <see langword="null"/>.</exception>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update the text logic (for shake/wave animations)
            _textComponent?.Update(gameTime);

            // Ensure the text position always follows the button, including any hover/press offset
            // applied through PositionDraw.
            if (_textComponent != null)
                _textComponent.Position = RenderPosition + _textPadding;
        }

        /// <summary>
        /// Draws the button's background and its label on top.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used for rendering.</param>
        /// <exception cref="ArgumentNullException"><paramref name="spriteBatch"/> is <see langword="null"/>.</exception>
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background sprite (handled by base DrawableEntity/UIElement logic)
            base.Draw(spriteBatch);

            // Draw the rich text on top
            _textComponent?.Draw(spriteBatch);
        }

        #endregion
    }
}
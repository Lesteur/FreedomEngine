using FreedomEngine.Collections.Tweens;
using FreedomEngine.Collections.Utilities;
using FreedomEngine.Graphics;
using FreedomEngine.Graphics.BitmapFonts;
using FreedomEngine.UI;
using Microsoft.Xna.Framework;
using System;

namespace MyGame.Scripts.Scenes
{
    public class MyButton : UIButton
    {
        #region Fields

        private Tween _hoverTween;

        #endregion

        #region Constructors

        public MyButton(Sprite backgroundSprite, Vector2 position, string text, BitmapFont font)
            : base(backgroundSprite, position, text, font)
        {
        }

        #endregion

        #region Protected Methods

        protected override void OnHovered(bool hovered)
        {
            base.OnHovered(hovered);

            if (_hoverTween != null && !_hoverTween.IsFinished)
            {
                _hoverTween.Stop();
            }

            if (hovered)
            {
                _hoverTween = new TweenVector2(PositionDraw, new Vector2(20, 0), TimeSpan.FromSeconds(0.15), val => PositionDraw = val, EasingFunctions.SineInOut);
                _textComponent.DefaultColor = Color.Yellow;
            }
            else
            {
                _hoverTween = new TweenVector2(PositionDraw, Vector2.Zero, TimeSpan.FromSeconds(0.15), val => PositionDraw = val, EasingFunctions.SineInOut);
                _textComponent.DefaultColor = Color.White;
            }
        }

        #endregion
    }
}

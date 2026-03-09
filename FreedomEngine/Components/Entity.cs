using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FreedomEngine.Graphics;

namespace FreedomEngine.Components
{
    public class Entity
    {
        private TimeSpan _elapsed = TimeSpan.Zero;

        protected Animation _sprite;

        protected int _currentFrame = 0;

        protected int _x = 0;

        protected int _y = 0;

        protected int _rotation = 0;

        protected float _scaleX = 1f;

        protected float _scaleY = 1f;

        protected SpriteEffects _spriteEffects = SpriteEffects.None;

        protected float _layerDepth = 0f;

        protected Color _color = Color.White;

        protected bool _visible = true;


        public Animation Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public int CurrentFrame
        {
            get => _currentFrame;
            set => _currentFrame = value;
        }

        public int X
        {
            get => _x;
            set => _x = value;
        }

        public int Y
        {
            get => _y;
            set => _y = value;
        }

        public int Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float ScaleX
        {
            get => _scaleX;
            set => _scaleX = value;
        }

        public float ScaleY
        {
            get => _scaleY;
            set => _scaleY = value;
        }

        public SpriteEffects SpriteEffects
        {
            get => _spriteEffects;
            set => _spriteEffects = value;
        }

        public float LayerDepth
        {
            get => _layerDepth;
            set => _layerDepth = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }


        public Entity(Animation sprite, int x = 0, int y = 0)
        {
            Sprite = sprite;
            X = x;
            Y = y;
        }


        public virtual void Update(GameTime gameTime)
        {
            if (_sprite == null)
                return;

            _elapsed += gameTime.ElapsedGameTime;

            if (_elapsed >= _sprite.Delay)
            {
                _elapsed -= _sprite.Delay;
                _currentFrame++;

                if (_currentFrame >= _sprite.Frames.Count)
                {
                    _currentFrame = 0;
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_sprite == null)
                return;

            spriteBatch.Draw(
                _sprite.Frames[_currentFrame].Texture,
                new Vector2(_x, _y),
                _sprite.Frames[_currentFrame].SourceRectangle,
                _color,
                MathHelper.ToRadians(_rotation),
                new Vector2(0, 0),
                new Vector2(_scaleX, _scaleY),
                _spriteEffects,
                _layerDepth
            );
        }
    }
}
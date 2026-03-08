using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Core
{
    public class Camera
    {
        private int _x;

        private int _y;

        private float _rotation;

        private float _scale;

        private int _viewportWidth;

        private int _viewportHeight;

        private Matrix _transformMatrix = Matrix.Identity;

        private bool _dirty = false;


        public int X
        {
            get => _x;
            set
            {
                _x = value;
                _dirty = true;
            }
        }

        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                _dirty = true;
            }
        }

        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _dirty = true;
            }
        }

        public float Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _dirty = true;
            }
        }

        public int ViewportWidth
        {
            get => _viewportWidth;
            set
            {
                _viewportWidth = value;
                _dirty = true;
            }
        }

        public int ViewportHeight
        {
            get => _viewportHeight;
            set
            {
                _viewportHeight = value;
                _dirty = true;
            }
        }

        public Matrix TransformMatrix
        {
            get
            {
                if (_dirty)
                    RecalculateTransformMatrix();

                return _transformMatrix;
            }
        }


        public Camera(int x, int y, int viewportWidth, int viewportHeight)
        {
            _x = x;
            _y = y;
            _rotation = 0f;
            _scale = 1f;
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
        }


        private void RecalculateTransformMatrix()
        {
            _transformMatrix =
                Matrix.CreateTranslation(new Vector3(-_x, -_y, 0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_scale, _scale, 1f) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0f));

            _dirty = false;
        }
    }
}
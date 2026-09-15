using System;

using Microsoft.Xna.Framework;

namespace FreedomEngine.Core
{
    /// <summary>
    /// Represents a 2D camera used to transform the view of the game world.
    /// </summary>
    public class Camera
    {
        #region Fields

        /// <summary>
        /// Coordinates of the camera's position in the game world.
        /// </summary>
        private Vector2 _position;

        /// <summary>
        /// Rotation of the camera in radians, applied around the center of the viewport.
        /// </summary>
        private float _rotation;

        /// <summary>
        /// Zoom scale of the camera, where 1.0f is normal size, less
        /// than 1.0f is zoomed out, and greater than 1.0f is zoomed in.
        /// </summary>
        private float _scale;

        /// <summary>
        /// Width of the viewport in pixels, used to calculate the center point for transformations.
        /// </summary>
        private int _viewportWidth;

        /// <summary>
        /// Height of the viewport in pixels, used to calculate the center point for transformations.
        /// </summary>
        private int _viewportHeight;

        /// <summary>
        /// Internal transformation matrix that combines translation,
        /// rotation, and scaling based on the camera's properties.
        /// Recalculated only when necessary to optimize performance.
        /// </summary>
        private Matrix _transformMatrix = Matrix.Identity;

        /// <summary>
        /// Value indicating whether the current state has been modified.
        /// </summary>
        private bool _dirty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the 2D position vector of the camera.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the X coordinate of the camera's position.
        /// </summary>
        public float X
        {
            get => _position.X;
            set
            {
                _position.X = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the Y coordinate of the camera's position.
        /// </summary>
        public float Y
        {
            get => _position.Y;
            set
            {
                _position.Y = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the camera in radians.
        /// </summary>
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the zoom scale of the camera.
        /// </summary>
        public float Scale
        {
            get => _scale;
            set
            {
                if (value <= 0f)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Scale must be greater than zero.");

                _scale = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the width of the viewport, in pixels.
        /// </summary>
        public int ViewportWidth
        {
            get => _viewportWidth;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Viewport width cannot be negative.");

                _viewportWidth = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the height of the viewport, in pixels.
        /// </summary>
        public int ViewportHeight
        {
            get => _viewportHeight;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Viewport height cannot be negative.");

                _viewportHeight = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets the transformation matrix used for rendering operations.
        /// Recalculates only when the camera properties have changed.
        /// </summary>
        public Matrix TransformMatrix
        {
            get
            {
                if (_dirty)
                    RecalculateTransformMatrix();

                return _transformMatrix;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="position">The initial position of the camera.</param>
        /// <param name="viewportWidth">The width of the viewport, in pixels.</param>
        /// <param name="viewportHeight">The height of the viewport, in pixels.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="viewportWidth"/> or <paramref name="viewportHeight"/> is negative.
        /// </exception>
        public Camera(Vector2 position, int viewportWidth, int viewportHeight)
        {
            _position = position;
            _rotation = 0f;
            _scale = 1f;

            ViewportWidth = viewportWidth;
            ViewportHeight = viewportHeight;

            _dirty = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// </summary>
        /// <param name="x">The initial X coordinate of the camera.</param>
        /// <param name="y">The initial Y coordinate of the camera.</param>
        /// <param name="viewportWidth">The width of the viewport, in pixels.</param>
        /// <param name="viewportHeight">The height of the viewport, in pixels.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="viewportWidth"/> or <paramref name="viewportHeight"/> is negative.
        /// </exception>
        public Camera(float x, float y, int viewportWidth, int viewportHeight)
            : this(new Vector2(x, y), viewportWidth, viewportHeight)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether an axis-aligned rectangle, given by its top-left corner and size, is
        /// visible within this camera's viewport.
        /// </summary>
        /// <param name="position">The top-left corner of the rectangle to test.</param>
        /// <param name="width">The width of the rectangle to test.</param>
        /// <param name="height">The height of the rectangle to test.</param>
        /// <returns><see langword="true"/> if the rectangle overlaps the viewport; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// This is a fast, axis-aligned overlap test. It does not account for <see cref="Rotation"/>
        /// or <see cref="Scale"/>, so it may report false positives or negatives for a rotated or
        /// zoomed camera.
        /// </remarks>
        public bool IsInView(Vector2 position, float width, float height)
        {
            float viewLeft = _position.X - (_viewportWidth / 2f);
            float viewTop = _position.Y - (_viewportHeight / 2f);

            return position.X + width > viewLeft &&
                   position.X < viewLeft + _viewportWidth &&
                   position.Y + height > viewTop &&
                   position.Y < viewTop + _viewportHeight;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Recalculates the internal transformation matrix based on current position, rotation, and scale.
        /// </summary>
        private void RecalculateTransformMatrix()
        {
            _transformMatrix =
                Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0f)) *
                Matrix.CreateRotationZ(_rotation) *
                Matrix.CreateScale(_scale, _scale, 1f) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0f));

            _dirty = false;
        }

        #endregion
    }
}
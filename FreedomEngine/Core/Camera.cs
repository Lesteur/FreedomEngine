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
                _scale = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the width of the viewport.
        /// </summary>
        public int ViewportWidth
        {
            get => _viewportWidth;
            set
            {
                _viewportWidth = value;
                _dirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the height of the viewport.
        /// </summary>
        public int ViewportHeight
        {
            get => _viewportHeight;
            set
            {
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
        /// <param name="Position">The initial position of the camera.</param>
        /// <param name="viewportWidth">The width of the viewport.</param>
        /// <param name="viewportHeight">The height of the viewport.</param>
        public Camera(Vector2 Position, int viewportWidth, int viewportHeight)
        {
            _position       = Position;
            _rotation       = 0f;
            _scale          = 1f;
            _viewportWidth  = viewportWidth;
            _viewportHeight = viewportHeight;
            _dirty          = true;
        }

        public Camera(float x, float y, int viewportWidth, int viewportHeight)
            : this(new Vector2(x, y), viewportWidth, viewportHeight)
        {
        }

        #endregion

        #region Public Methods

        public bool IsInView(Vector2 Position, float width, float height)
        {
            var _posX = _position.X - (_viewportWidth / 2f);
            var _posY = _position.Y - (_viewportHeight / 2f);

            return Position.X + width > _posX &&
                   Position.X < _posX + _viewportWidth &&
                   Position.Y + height > _posY &&
                   Position.Y < _posY + _viewportHeight;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Structures
{
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        #region Fields

        private static readonly Vector2Int _zeroVector = new(0, 0);

        private static readonly Vector2Int _oneVector = new(1, 1);

        private static readonly Vector2Int _upVector = new(0, 1);

        private static readonly Vector2Int _downVector = new(0, -1);

        private static readonly Vector2Int _leftVector = new(-1, 0);

        private static readonly Vector2Int _rightVector = new(1, 0);

        #endregion

        #region Properties

        public int X { get; set; }

        public int Y { get; set; }

        #endregion

        #region Constructors

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Operators

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Int operator *(Vector2Int a, int d)
        {
            return new Vector2Int(a.X * d, a.Y * d);
        }

        public static Vector2Int operator *(int d, Vector2Int a)
        {
            return new Vector2Int(a.X * d, a.Y * d);
        }

        public static Vector2Int operator /(Vector2Int a, int d)
        {
            return new Vector2Int(a.X / d, a.Y / d);
        }

        public static Vector2Int operator -(Vector2Int a)
        {
            return new Vector2Int(-a.X, -a.Y);
        }

        public static Vector2Int operator +(Vector2Int a)
        {
            return new Vector2Int(+a.X, +a.Y);
        }

        public static Vector2Int operator ++(Vector2Int a)
        {
            return new Vector2Int(a.X + 1, a.Y + 1);
        }

        public static Vector2Int operator --(Vector2Int a)
        {
            return new Vector2Int(a.X - 1, a.Y - 1);
        }

        public static Vector2Int operator %(Vector2Int a, int d)
        {
            return new Vector2Int(a.X % d, a.Y % d);
        }

        public static bool operator ==(Vector2Int left, Vector2Int right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2Int left, Vector2Int right)
        {
            return !(left == right);
        }

        #endregion

        #region Public Methods

        public void Set(int x, int y)
        {
            X = x;
            Y = y;
        }

        public readonly int SqrMagnitude()
        {
            return X * X + Y * Y;
        }

        public static Vector2Int Zero => _zeroVector;

        public static Vector2Int One => _oneVector;

        public static Vector2Int Up => _upVector;

        public static Vector2Int Down => _downVector;

        public static Vector2Int Left => _leftVector;

        public static Vector2Int Right => _rightVector;

        #endregion

        #region Override Methods

        public override readonly bool Equals(object obj)
        {
            if (obj is Vector2Int other)
            {
                return X == other.X && Y == other.Y;
            }

            return false;
        }

        public readonly bool Equals(Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public readonly bool Equals(in Vector2Int other)
        {
            return X == other.X && Y == other.Y;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override readonly string ToString()
        {
            return $"Vector2Int({X}, {Y})";
        }

        #endregion
    }
}

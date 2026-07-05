using System;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public class TileNode<T> : IComparable<TileNode<T>> where T : Tile<T>
    {
        public T Tile { get; }

        public int ID => Tile.ID;

        public int G { get; } // Cost from start

        public int H { get; } // Heuristic to target

        public int F => G + H; // Total estimated cost

        public TileNode<T> Parent { get; }

        public TileNode(T tile, int g, int h, TileNode<T> parent = null)
        {
            Tile = tile;
            G = g;
            H = h;
            Parent = parent;
        }

        public int CompareTo(TileNode<T> other) => F.CompareTo(other.F);

        public override bool Equals(object obj) => obj is TileNode<T> other && ID == other.ID;

        public override int GetHashCode() => ID.GetHashCode();
    }
}

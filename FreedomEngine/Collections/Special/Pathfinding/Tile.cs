using System;
using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public abstract class Tile<T> where T : Tile<T>
    {
        public int ID { get; }

        public Tile(int id)
        {
            ID = id;
        }

        public abstract List<T> GetNeighbors();

        public abstract int GetHeuristic(T other);

        public abstract bool IsCrossableBy(bool finalTile);
    }
}

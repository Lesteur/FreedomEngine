using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public readonly struct PathResult<T> where T : Tile<T>
    {
        public T Destination { get; }

        public IReadOnlyList<T> Path { get; }

        public int Length => Path.Count;

        public bool IsValid => Destination != null && Path.Count > 0;

        public PathResult(T destination, List<T> path)
        {
            Destination = destination;
            Path = path ?? [];
        }

        public override string ToString()
        {
            return $"PathResult -> Destination: {Destination?.ToString() ?? "null"}, Length: {Length}";
        }
    }
}

using System.Collections.Generic;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public class PathResult<T> where T : Tile<T>
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

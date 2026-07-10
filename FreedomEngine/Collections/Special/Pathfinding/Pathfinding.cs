using System;
using System.Collections.Generic;
using System.IO;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public class Pathfinding<T, U> where T : Tile<T> where U : Unit<T>
    {
        public List<T> FindPath(T start, T target, U unit)
        {
            var queue = new PriorityQueue<TileNode<T>, int>();
            var closedSet = new HashSet<int>();
            var visited = new Dictionary<int, int>();
            var startNode = new TileNode<T>(start, 0, start.GetHeuristic(target));

            queue.Enqueue(startNode, startNode.F);
            visited[start.ID] = 0;

            List<T> finalPath = null;

            while (queue.Count > 0)
            {
                TileNode<T> current = queue.Dequeue();

                if (current.ID == target.ID)
                {
                    var path = ReconstructPath(current); // Path found
                    finalPath = [.. path];
                    break;
                }

                closedSet.Add(current.ID);

                unit.ExpandNeighbors(current, visited, queue, null, target, closedSet);
            }

            closedSet.Clear();
            visited.Clear();
            queue.Clear();

            return finalPath;
        }

        public List<T> GetReachableTiles(T start, U unit)
        {
            var reachableTiles = new List<T>();
            var visited = new Dictionary<int, int>();
            var queue = new PriorityQueue<TileNode<T>, int>();

            var startNode = new TileNode<T>(start, 0, 0);
            queue.Enqueue(startNode, startNode.F);

            while (queue.Count > 0)
            {
                TileNode<T> current = queue.Dequeue();

                if (visited.TryGetValue(current.ID, out int value) && value <= current.G)
                    continue;

                visited[current.ID] = current.G;
                reachableTiles.Add(current.Tile);

                unit.ExpandNeighbors(current, visited, queue, null, null, null);
            }

            visited.Clear();
            queue.Clear();

            return reachableTiles;
        }

        public List<PathResult<T>> GetAllPathsFrom(T start, U unit)
        {
            var results = new List<PathResult<T>>();
            var visited = new Dictionary<int, int>();
            var paths = new Dictionary<int, TileNode<T>>();
            var queue = new PriorityQueue<TileNode<T>, int>();

            var startNode = new TileNode<T>(start, 0, 0);

            queue.Enqueue(startNode, startNode.F);
            paths[start.ID] = startNode;

            while (queue.Count > 0)
            {
                TileNode<T> current = queue.Dequeue();

                if (visited.TryGetValue(current.ID, out int value) && value <= current.G)
                    continue;

                visited[current.ID] = current.G;

                unit.ExpandNeighbors(current, visited, queue, paths, null, null);
            }

            foreach (var pathNode in paths.Values)
            {
                if (pathNode.Tile == start)
                    continue; // Skip start tile

                var path = ReconstructPath(pathNode);
                results.Add(new PathResult<T>(pathNode.Tile, path));
            }

            visited.Clear();
            paths.Clear();
            queue.Clear();

            return results;
        }

        private static List<T> ReconstructPath(TileNode<T> node)
        {
            var path = new List<T>();
            var current = node;

            while (current != null)
            {
                path.Add(current.Tile);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}

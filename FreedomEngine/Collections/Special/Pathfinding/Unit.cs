using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Special.Pathfinding
{
    public abstract class Unit<T> where T : Tile<T>
    {
        public abstract void ExpandNeighbors(TileNode<T> current, Dictionary<int, int> visited, PriorityQueue<TileNode<T>, int> queue, Dictionary<int, TileNode<T>> paths, T target = null, HashSet<int> closedSet = null);

        public virtual bool CanCross(T tile, bool finalTile)
        {
            if (tile == null || tile.IsCrossableBy(finalTile))
                return false;

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreedomEngine.Collections.Special.RPG
{
    public enum TargetTypeEnum
    {
        Self,
        Tile,
        Ally,
        AllyExceptSelf,
        AllAllies,
        AllAlliesExceptSelf,
        Enemy,
        AllEnemies,
        All
    }
}

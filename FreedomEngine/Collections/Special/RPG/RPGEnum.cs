namespace FreedomEngine.Collections.Special.RPG
{
    public enum AffinityEnum : byte
    {
        Slash,
        Blunt,
        Pierce,
        Concussive,
        Flexible,
        Resonant,
        Fire,
        Ice,
        Thunder,
        Water,
        Wind,
        Earth,
        Light,
        Dark,

        None,
    }

    public enum AffinityEffectEnum : byte
    {
        Neutral,
        Weak,
        Resistant,
        Immune,
        Absorb
    }

    public enum TargetTypeEnum : byte
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

    public enum ContextTypeEnum : byte
    {
        Battle,
        Exploration,
        All,
        None
    }
}
namespace FreedomEngine.Collections.Special.RPG
{
    public class BattlerData
    {
        #region Properties

        public Stats BaseStats { get; }

        public AffinityEffectEnum[] Affinities { get; }

        #endregion

        #region Public Methods

        public AffinityEffectEnum GetAffinityEffect(AffinityEnum affinity)
        {
            return Affinities[(int)affinity];
        }

        #endregion
    }
}
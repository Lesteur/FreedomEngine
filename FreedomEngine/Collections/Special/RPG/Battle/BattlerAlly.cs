using System.Collections;

using FreedomEngine.Collections.Coroutines;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    /// <summary>
    /// A battler controlled by the player's side.
    /// </summary>
    public class BattlerAlly : Battler
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BattlerAlly"/> class.
        /// </summary>
        /// <param name="data">The data describing this battler.</param>
        public BattlerAlly(BattlerData data) : base(data)
        {
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override IEnumerator Act()
        {
            yield return new WaitForSeconds(2f);

            Logger.Info("Test action completed after 2 seconds for the ally.");
        }

        #endregion
    }
}
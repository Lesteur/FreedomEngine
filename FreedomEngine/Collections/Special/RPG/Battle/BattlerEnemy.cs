using System.Collections;

using FreedomEngine.Collections.Coroutines;

namespace FreedomEngine.Collections.Special.RPG.Battle
{
    public class BattlerEnemy : Battler
    {
        #region Fields

        private Coroutine _coroutine;

        #endregion

        #region Constructors

        public BattlerEnemy(BattlerData data) : base(data)
        {
        }

        #endregion

        #region Public Methods

        public override bool StartTurn()
        {
            if (!base.StartTurn())
                return false;

            _coroutine = new Coroutine(TestCoroutine());

            FinishAction();

            return true;
        }

        #endregion

        #region Private Methods

        private static IEnumerator TestCoroutine()
        {
            yield return new WaitForSeconds(2f);

            Logger.Info("TestCoroutine completed after 2 seconds for the ally.");

            FinishAction();

            yield return null;
        }

        #endregion
    }
}
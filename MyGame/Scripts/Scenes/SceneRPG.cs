using Microsoft.Xna.Framework;

using FreedomEngine.Core;
using FreedomEngine.Collections.Special.RPG;
using FreedomEngine.Collections.Special.RPG.Battle;

namespace MyGame.Scripts.Scenes
{
    public class SceneRPG : Scene
    {
        private BattleSystem _battleManager;

        private BattlerAlly[] _battlers;

        private BattlerEnemy[] _enemies;

        public override void Initialize()
        {
            base.Initialize();

            _width = 640;
            _height = 360;

            _cameraLimitsMin = new Vector2(640 / 2f, 360 / 2f);
            _cameraLimitsMax = new Vector2(_width - 320, _height - 180);

            _battlers = [new(null), new(null), new(null)];
            _enemies = [new(null), new(null)];

            _battleManager = new BattleSystem(_battlers, _enemies, null);

            _battleManager.StartBattle();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
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

            var stats = new Stats()
            {
                HealthPoints = 100,
                SkillPoints = 50,
                Attack = 10,
                Magic = 5,
                Defense = 8,
                MagicDefense = 6,
                Precision = 90,
                Dodge = 10,
                Speed = 12,
                CriticalRate = 0.1f,
                CriticalPower = 1.5f
            };
            var battlerData = new BattlerData(stats, null);

            var enemyStats = new Stats()
            {
                HealthPoints = 80,
                SkillPoints = 30,
                Attack = 8,
                Magic = 4,
                Defense = 6,
                MagicDefense = 5,
                Precision = 85,
                Dodge = 12,
                Speed = 10,
                CriticalRate = 0.05f,
                CriticalPower = 1.2f
            };
            var enemyData = new BattlerData(enemyStats, null);

            _battlers = [new(battlerData), new(battlerData), new(battlerData)];
            _enemies = [new(enemyData), new(enemyData)];

            _battleManager = new BattleSystem(_battlers, _enemies, null);

            _battleManager.StartBattle();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
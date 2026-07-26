using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Collections.Special.Metroidvania;

namespace MyGame.Scripts.Metroid
{
    public class PlayerSamus : PhysicalEntity
    {
        #region Structs

        // Holds all the physics constants to reproduce the GML feel
        public struct PhysicsConfig
        {
            public float GroundAccel = 0.2f;
            public float GroundDecel = 0.25f;
            public float BrakeFriction = 0.4f;
            public float MaxRunSpeed = 2.6f;
            public float MaxDashSpeed = 4.8f;
            public float AirAccel = 0.15f;
            public float AirFriction = 0.02f;
            public float JumpImpulse = 8.2f;//5.2f;
            public float JumpCutoffFactor = 0.45f;
            public float Gravity = 0.20f;
            public float MaxFallSpeed = 7.5f;

            public PhysicsConfig() { }
        }

        #endregion

        #region Fields

        private StateMachineSamus _machine;

        #endregion

        #region Public Properties

        public PhysicsConfig Physics { get; private set; } = new PhysicsConfig();

        // Exposing inputs for the StateMachine to read
        public int Direction { get; set; } = 1; // 1 for Right, -1 for Left
        public int InputMoveX { get; private set; }
        public int InputMoveY { get; private set; }
        public bool InputRun { get; private set; }
        public bool InputJumpPressed { get; private set; }
        public bool InputJumpReleased { get; private set; }

        #endregion

        #region Constructors

        public PlayerSamus(Sprite sprite, Vector2 position, CollisionMask collisionMask = null) : base(sprite, position, collisionMask)
        {
            _machine = new StateMachineSamus(this);
        }

        #endregion

        #region Lifecycle Methods

        public override void Update(GameTime gameTime)
        {
            GetControllerInput();

            _machine.Update(gameTime);

            base.Update(gameTime);
        }

        #endregion

        #region Private Methods

        private void GetControllerInput()
        {
            // 1. Horizontal Axis (-1 for Left, 1 for Right, 0 for None)
            InputMoveX = 0;
            if (Core.Input.Keyboard.IsKeyDown(Keys.Right)) InputMoveX += 1;
            if (Core.Input.Keyboard.IsKeyDown(Keys.Left)) InputMoveX -= 1;

            InputMoveY = 0;
            if (Core.Input.Keyboard.IsKeyDown(Keys.Down)) InputMoveY += 1;
            if (Core.Input.Keyboard.IsKeyDown(Keys.Up)) InputMoveY -= 1;

            // 2. Action Buttons
            InputRun = Core.Input.Keyboard.IsKeyDown(Keys.X);

            InputJumpPressed = Core.Input.Keyboard.WasKeyJustPressed(Keys.Z);
            InputJumpReleased = Core.Input.Keyboard.WasKeyJustReleased(Keys.Z);
        }

        #endregion
    }
}
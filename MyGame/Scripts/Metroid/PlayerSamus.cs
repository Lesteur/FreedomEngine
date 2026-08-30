using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Collections.Special.Metroidvania;
using Microsoft.Xna.Framework.Graphics;
using FreedomEngine.Collections;
using FreedomEngine.Collections.Interfaces;

namespace MyGame.Scripts.Metroid
{
    public class PlayerSamus : PhysicalEntity, ILoadContent
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

        public struct Sprites
        {
            public Sprite StandCenter;
            public Sprite StandLeft;
            public Sprite StandRight;

            public Sprite RunLeg;
            public Sprite RunLeft;
            public Sprite RunRight;

            public Sprite JumpLeft;
            public Sprite JumpRight;

            public Sprites() { }
        }

        #endregion

        #region Fields

        private static TextureAtlas _textureAtlas;

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
        public Sprite SpriteTop { get; set; }
        public Sprite SpriteBottom { get; set; }

        #endregion

        #region Constructors

        public PlayerSamus(Vector2 position, CollisionMask collisionMask = null) : base(null, position, collisionMask)
        {
            _machine = new StateMachineSamus(this);

            SpriteBottom = _textureAtlas.GetSprite("RunLeg");
            SpriteTop = _textureAtlas.GetSprite("RunRight");

            Sprite = SpriteBottom;
        }

        #endregion

        #region Lifecycle Methods

        public static void LoadContent(ContentManager Content)
        {
            ArgumentNullException.ThrowIfNull(Content, nameof(Content));

            _textureAtlas = Content.Load<TextureAtlas>("Assets/TextureAtlas/Samus");
        }

        public static void UnloadContent()
        {
            // Unload any static content if necessary
        }

        public override void Update(GameTime gameTime)
        {
            GetControllerInput();

            _machine.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (SpriteTop != null)
            {
                var originTop = SpriteTop.Origin;

                spriteBatch.Draw(SpriteTop.Texture, Position, SpriteTop.Frames[CurrentFrame], Color, Rotation, originTop, Scale, Effects, LayerDepth);
            }
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
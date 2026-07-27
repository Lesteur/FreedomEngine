using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FreedomEngine.Graphics;
using FreedomEngine.Components.Collisions;
using FreedomEngine.Collections.Special.Metroidvania;
using Microsoft.Xna.Framework.Graphics;
using FreedomEngine.Collections;

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

        private Texture2D _texture;

        private TextureAtlas _textureAtlas;

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

            _texture = Core.Content.Load<Texture2D>("Assets/Textures/spr_samus");
            _textureAtlas = new TextureAtlas(_texture);

            // Load sprites from the texture atlas
            _textureAtlas.AddSprite("StandCenter", 1, Vector2.Zero, TimeSpan.Zero, 4, 16, 25, 48, 0, 0);
            _textureAtlas.AddSprite("StandRight", 6, Vector2.Zero, TimeSpan.FromMilliseconds(150), 4, 68, 29, 43, 2, 0);
            _textureAtlas.AddSprite("StandLeft", 6, Vector2.Zero, TimeSpan.FromMilliseconds(150), 4, 115, 29, 43, 2, 0);

            // _textureAtlas.AddSprite("RunLeg", 22, 18, 2, TimeSpan.FromMilliseconds(30), 4, 162, 36, 29, 2, 0);
            _textureAtlas.AddSprite("RunLeg", 22, new Vector2(18, 2), TimeSpan.FromMilliseconds(30), 4, 162, 36, 29, 2, 0);

            // _textureAtlas.AddSprite("RunRight", 22, 10, 15, TimeSpan.FromMilliseconds(30), 4, 245, 26, 25, 2, 0);
            // _textureAtlas.AddSprite("RunLeft", 22, 9, 15, TimeSpan.FromMilliseconds(30), 4, 274, 25, 23, 2, 0);
            _textureAtlas.AddSprite("RunRight", 22, new Vector2(10, 18), TimeSpan.FromMilliseconds(30), 4, 245, 26, 25, 2, 0);
            _textureAtlas.AddSprite("RunLeft", 22, new Vector2(10, 19), TimeSpan.FromMilliseconds(30), 4, 274, 25, 23, 2, 0);

            SpriteBottom = _textureAtlas.GetSprite("RunLeg");
            SpriteTop = _textureAtlas.GetSprite("RunLeft");

            ChangeSprite(SpriteBottom);
        }

        #endregion

        #region Lifecycle Methods

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
                var positionTop = new Vector2(X + originTop.X, Y + originTop.Y);

                SpriteTop.Animation.Frames[CurrentFrame].Draw(
                    spriteBatch,
                    Position,//positionTop,
                    Color,
                    Rotation,
                    originTop,
                    Scale,
                    Effects,
                    LayerDepth
                );
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
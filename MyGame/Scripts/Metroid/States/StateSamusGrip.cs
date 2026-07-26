using FreedomEngine.Collections.States;
using Microsoft.Xna.Framework;
using System;
using System.Reflection.PortableExecutable;

namespace MyGame.Scripts.Metroid.States
{
    internal class StateSamusGrip : StateSamus
    {
        #region Fields

        private bool _isVaulting;
        private float _vaultTimer;
        private readonly float _vaultDuration = 0.25f; // Duration of the climb animation in seconds

        // Hardcoded offsets to snap the player on top of the ledge after climbing
        private float _vaultOffsetX = 12f;
        private float _vaultOffsetY = -30f;

        #endregion

        #region Constructors

        public StateSamusGrip(PlayerSamus player, StateMachineSamus stateMachine) : base(player, stateMachine)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void OnEnter()
        {
            base.OnEnter();

            // 1. Freeze physics completely
            _player.XSpeed = 0f;
            _player.YSpeed = 0f;
            _isVaulting = false;
            _vaultTimer = 0f;

            _vaultOffsetX = _player.Width;
            _vaultOffsetY = -_player.Height - 3f;

            // Optional: Snap player position to the ledge corner here based on collision data
            // _player.Position = new Vector2(ledgeTargetX, ledgeTargetY);
        }

        public override void Update(GameTime gameTime)
        {
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isVaulting)
            {
                // Handle the cinematic climbing sequence
                ProcessVaulting(dt);
                return; // Block other inputs while climbing
            }

            // 2. Wait for player input to drop or climb
            HandleGripInputs();

            base.Update(gameTime);
        }

        #endregion

        #region Private Methods

        private void HandleGripInputs()
        {
            // Drop down if the player presses DOWN or AWAY from the wall
            if (_player.InputMoveY > 0 || (_player.InputMoveX == -_player.Direction))
            {
                // Re-enable gravity implicitly by leaving the state
                StateMachine.ChangeState(StateMachine.NormalState);
                return;
            }

            // Climb up if the player presses UP, JUMP, or TOWARDS the wall
            if (_player.InputMoveY < 0 || _player.InputJumpPressed || (_player.InputMoveX == _player.Direction))
            {
                _isVaulting = true;
                // Trigger climbing animation here
                // _player.Sprite.PlayAnimation("Vault");
            }
        }

        private void ProcessVaulting(float dt)
        {
            _vaultTimer += dt;

            // Wait for the animation to finish
            if (_vaultTimer >= _vaultDuration)
            {
                // 3. Teleport player on top of the ledge
                // We use the player's facing direction to know which way to shift X
                float shiftX = _vaultOffsetX * _player.Direction;

                _player.Position = new Vector2(_player.Position.X + shiftX, _player.Position.Y + _vaultOffsetY);

                // Exit Grip state and return to normal standing state
                StateMachine.ChangeState(StateMachine.NormalState);
            }
        }

        #endregion
    }
}
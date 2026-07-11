using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.States
{
    public abstract class StateMachine<IState> : IUpdate where IState : State<IState>
    {
        #region Fields

        protected IState _currentState;

        #endregion

        #region Constructors

        public StateMachine()
        {
        }

        #endregion

        #region Lifecycle Methods

        public virtual void Update(GameTime gameTime)
        {
            _currentState?.Update(gameTime);
        }

        #endregion

        #region Public Methods

        public void ChangeState(IState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState?.OnEnter();
        }

        #endregion
    }
}

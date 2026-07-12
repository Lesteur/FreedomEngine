using System;

using Microsoft.Xna.Framework;

using FreedomEngine.Collections.Interfaces;

namespace FreedomEngine.Collections.States
{
    public abstract class State<IState, IMachine> : IUpdate where IState : State<IState, IMachine>
    {
        #region Properties

        public IMachine StateMachine { get; private set; }

        #endregion

        #region Constructors

        public State(IMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        #endregion

        #region Lifecycle Methods

        public virtual void Update(GameTime gameTime)
        {
        }

        #endregion

        #region Public Methods

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        #endregion
    }
}

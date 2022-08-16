using System;
using System.Collections.Generic;
using OctanGames.StateMachine.States;

namespace OctanGames.StateMachine
{
    public abstract class BaseStateMachine<T> where T : Enum
    {
        protected abstract BaseStateFactory<T> StateFactory { get; }
        protected abstract T InitialStateType { get; }

        private BaseState<T> _currentState;
        private T _currentStateType;

        protected BaseStateMachine()
        {
            _currentStateType = InitialStateType;
        }

        public void SwitchState(T state)
        {
            if (EqualityComparer<T>.Default.Equals(_currentStateType, state))
            {
                return;
            }

            _currentState?.ExitState();
            BaseState<T> newState = StateFactory.GetState(state);
            newState.EnterState();

            _currentState = newState;
            _currentStateType = state;
        }

        public void Update()
        {
            _currentState?.UpdateState();
        }
    }
}
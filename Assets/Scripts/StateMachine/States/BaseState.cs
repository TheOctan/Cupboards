using System;

namespace OctanGames.StateMachine.States
{
    public abstract class BaseState<T> where T : Enum
    {
        protected GameContext GameContext { get; }
        private readonly BaseStateMachine<T> _stateMachine;

        protected BaseState(BaseStateMachine<T> stateMachine,
            GameContext gameContext)
        {
            _stateMachine = stateMachine;
            GameContext = gameContext;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();

        protected void SwitchState(T newState)
        {
            _stateMachine.SwitchState(newState);
        }
    }
}
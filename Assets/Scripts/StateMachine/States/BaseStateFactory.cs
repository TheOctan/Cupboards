using System;
using System.Collections.Generic;
using OctanGames.StateMachine;
using OctanGames.StateMachine.States;

public abstract class BaseStateFactory<T> where T : Enum
{
    private readonly Dictionary<T, BaseState<T>> _states =
        new Dictionary<T, BaseState<T>>();

    private readonly BaseStateMachine<T> _baseStateMachine;

    protected BaseStateFactory(BaseStateMachine<T> baseStateMachine)
    {
        _baseStateMachine = baseStateMachine;
    }

    public void RegisterStates()
    {
        RegisterStates(_states, _baseStateMachine);
    }

    protected abstract void RegisterStates(Dictionary<T, BaseState<T>> states,
        BaseStateMachine<T> stateMachine);

    public BaseState<T> GetState(T state)
    {
        return _states.ContainsKey(state)
            ? _states[state]
            : null;
    }
}
using System.Collections.Generic;
using OctanGames.StateMachine.States;

namespace OctanGames.StateMachine
{
    public enum GameState
    {
        None,
        Deselect,
        Select,
        FindPath
    }
    
    public class GameStateFactory : BaseStateFactory<GameState>
    {
        private readonly GameContext _gameContext;

        public GameStateFactory(BaseStateMachine<GameState> baseStateMachine,
            GameContext gameContext) : base(baseStateMachine)
        {
            _gameContext = gameContext;
        }

        protected override void RegisterStates(Dictionary<GameState, BaseState<GameState>> states,
            BaseStateMachine<GameState> stateMachine)
        {
            states.Add(GameState.Deselect, new DeselectState(stateMachine, _gameContext));
            states.Add(GameState.Select, new SelectState(stateMachine, _gameContext));
            states.Add(GameState.FindPath, new FindPathState(stateMachine, _gameContext));
        }
    }
}
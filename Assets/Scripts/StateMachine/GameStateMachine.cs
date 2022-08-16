namespace OctanGames.StateMachine
{
    public class GameStateMachine : BaseStateMachine<GameState>
    {
        private readonly GameStateFactory _gameStateFactory;

        protected override BaseStateFactory<GameState> StateFactory => _gameStateFactory;
        protected override GameState InitialStateType => GameState.None;

        public GameStateMachine(GameContext gameContext)
        {
            _gameStateFactory = new GameStateFactory(this, gameContext);
            _gameStateFactory.RegisterStates();
        }
    }
}
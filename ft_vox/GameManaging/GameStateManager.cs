using ft_vox.GameStates;

namespace ft_vox.GameManaging
{
    internal class GameStateManager : IGameStateManager
    {
        private IGameState _gameState;

        public event GameStateEventHandlers.GameStateChangedEventHandler OnGameStateChanged;

        public IGameState GetGameState()
        {
            return _gameState;
        }

        public void SetGameState(IGameState gameState)
        {
            OnGameStateChanged?.Invoke(this, _gameState, gameState);
            _gameState = gameState;
        }
    }
}

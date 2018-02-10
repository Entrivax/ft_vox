using ft_vox.GameStates;

namespace ft_vox.GameManaging
{
    internal interface IGameStateManager
    {
        event GameStateEventHandlers.GameStateChangedEventHandler OnGameStateChanged;

        void SetGameState(IGameState gameState);

        IGameState GetGameState();
    }

    internal static class GameStateEventHandlers
    {
        public delegate void GameStateChangedEventHandler(object sender, IGameState oldGameState, IGameState newGameState);
    }
}

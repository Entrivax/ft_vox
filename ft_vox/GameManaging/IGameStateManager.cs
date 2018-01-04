using ft_vox.GameStates;
using static ft_vox.GameManaging.GameStateEventHandlers;

namespace ft_vox.GameManaging
{
    internal interface IGameStateManager
    {
        event GameStateChangedEventHandler OnGameStateChanged;

        void SetGameState(IGameState gameState);

        IGameState GetGameState();
    }

    internal static class GameStateEventHandlers
    {
        public delegate void GameStateChangedEventHandler(object sender, IGameState oldGameState, IGameState newGameState);
    }
}

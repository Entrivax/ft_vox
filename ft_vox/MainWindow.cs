using ft_vox.GameManaging;
using ft_vox.GameStates;
using ft_vox.Helpers;
using ft_vox.Worlds;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;

namespace ft_vox
{
    internal class MainWindow : GameWindow
    {
        private IGameStateManager _gameStateManager;

        public MainWindow(IGameStateManager gameStateManager, World world) : base(800, 600, GraphicsMode.Default, "ft_vox", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            _gameStateManager = gameStateManager;
            
            _gameStateManager.OnGameStateChanged += OnGameStateChanged;
            _gameStateManager.SetGameState(new GameStatePlay(world));
        }

        private void OnGameStateChanged(object sender, IGameState oldGameState, IGameState newGameState)
        {
            oldGameState?.OnUnload();
            newGameState.OnLoad(Width, Height);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            _gameStateManager.GetGameState().OnResize(Width, Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardHelper.Update();
            MouseHelper.Update();

            var gameState = _gameStateManager.GetGameState();
            gameState.Update(e.Time);
            CursorVisible = gameState.CursorVisible;

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Title = $"ft_vox FPS: {1f / e.Time}";
            _gameStateManager.GetGameState().Draw(e.Time);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}

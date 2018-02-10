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
            _gameStateManager.SetGameState(new GameStatePlay(gameStateManager, world));
        }

        protected override void OnLoad(EventArgs e)
        {
            Console.WriteLine($"GL Vendor: {GL.GetString(StringName.Vendor)}");
            Console.WriteLine($"GL Renderer: {GL.GetString(StringName.Renderer)}");
            Console.WriteLine($"GL Version: {GL.GetString(StringName.Version)}");
            Console.WriteLine($"GL Shading language version: {GL.GetString(StringName.ShadingLanguageVersion)}");
            base.OnLoad(e);
        }

        private void OnGameStateChanged(object sender, IGameState oldGameState, IGameState newGameState)
        {
            oldGameState?.OnUnload();
            if (newGameState == null)
            {
                Exit();
                return;
            }
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

            gameState?.Update(e.Time);
            CursorVisible = gameState?.CursorVisible ?? true;

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_gameStateManager != null)
                _gameStateManager.GetGameState()?.Draw(e.Time);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}

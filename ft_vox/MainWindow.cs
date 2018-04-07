using ft_vox.GameManaging;
using ft_vox.GameStates;
using ft_vox.Helpers;
using ft_vox.Worlds;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.ComponentModel;
using System.IO;
using OpenTK.Input;

namespace ft_vox
{
    internal class MainWindow : GameWindow
    {
        private readonly IGameStateManager _gameStateManager;

        public MainWindow(IGameStateManager gameStateManager, World world) : base(1280, 720, GraphicsMode.Default, "ft_vox", GameWindowFlags.Default, DisplayDevice.Default, 4, 0, GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.Off;
            _gameStateManager = gameStateManager;
            
            _gameStateManager.OnGameStateChanged += OnGameStateChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            Console.WriteLine($"GL Vendor: {GL.GetString(StringName.Vendor)}");
            Console.WriteLine($"GL Renderer: {GL.GetString(StringName.Renderer)}");
            Console.WriteLine($"GL Version: {GL.GetString(StringName.Version)}");
            Console.WriteLine($"GL Shading language version: {GL.GetString(StringName.ShadingLanguageVersion)}");
            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _gameStateManager.GetGameState()?.OnUnload();
            base.OnClosing(e);
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
            if (KeyboardHelper.IsKeyPressed(Key.F5))
            {
                WindowState = WindowState != WindowState.Fullscreen ? WindowState.Fullscreen : WindowState.Normal;
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _gameStateManager.GetGameState()?.Draw(e.Time);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
    }
}

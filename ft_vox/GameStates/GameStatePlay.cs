using ft_vox.OpenGL;
using OpenTK.Graphics.OpenGL4;
using ft_vox.Gameplay;
using ft_vox.Worlds;
using OpenTK;
using System;
using OpenTK.Graphics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ft_vox.Helpers;

namespace ft_vox.GameStates
{
    internal class GameStatePlay : IGameState
    {
        public bool CursorVisible => true;

        private World _world;
        private Player _player;
        private Camera _camera;

        private Texture _terrainTexture;

        private Shader _baseShader;
        private Shader _guiShader;

        private int _width;
        private int _height;
        private Matrix4 _proj;
        private Matrix4 _guiProj;

        private int _renderDistance;
        
        private Text _text;

        private Thread _loadingThread;

        public GameStatePlay(World world)
        {
            _world = world;

            _baseShader = ShaderManager.Get("BaseShader");
            _guiShader = ShaderManager.Get("GuiShader");
            _player = new Player() { Position = new Vector3(0, 64, 0) };
            _camera = new Camera(new Vector3(0), new Vector3(0), (float)(80f * (Math.PI / 180f)));
            _terrainTexture = TextureManager.Get("terrain.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _text = new Text(new Vector2(13, 30), FontManager.Get("glyphs"), _guiShader, "");
            _renderDistance = 16;
            _loadingThread = new Thread(new ThreadStart(
                () =>
                {
                    while(true)
                    {
                        Thread.Sleep(10);
                        
                        var chunks = _world.GetLoadedChunks();
                        var playerPosition = _player.Position;
                        var playerPos2D = playerPosition.Xz;

                        var chunkPositionsThatCouldBeLoaded = new List<ChunkPosition>();
                        for (int x = -_renderDistance; x <= _renderDistance; x++)
                            for (int z = -_renderDistance; z <= _renderDistance; z++)
                            {
                                var chunkPositionInLocalCoordinates = new Vector2(x, z);
                                if ((chunkPositionInLocalCoordinates).LengthSquared < _renderDistance * _renderDistance)
                                {
                                    var chunkPosition = new ChunkPosition((int)((chunkPositionInLocalCoordinates.X + playerPos2D.X / 16)), (int)((chunkPositionInLocalCoordinates.Y + playerPos2D.Y / 16)));
                                    if (!chunks.Any(chunk => chunk.Item1.Equals(chunkPosition)))
                                        chunkPositionsThatCouldBeLoaded.Add(chunkPosition);
                                }
                            }
                        
                        var orderedChunks = chunkPositionsThatCouldBeLoaded.OrderBy(chunkPosition => (new Vector2(chunkPosition.X * 16 + 8 * (chunkPosition.X < 0 ? -1 : 1), chunkPosition.Z * 16 + 8 * (chunkPosition.Z < 0 ? -1 : 1)) - playerPos2D).LengthSquared).Cast<ChunkPosition?>();
                        var closestChunkToLoad = orderedChunks.FirstOrDefault();
                        if (closestChunkToLoad != null)
                            _world.GetChunkAt(closestChunkToLoad.Value.X, closestChunkToLoad.Value.Z);
                        
                        foreach (var chunk in chunks)
                        {
                            var chunkPos = chunk.Item1;
                            var chunkPositionInWorldCoordinates = new Vector2(chunkPos.X * 16 + 8 * (chunkPos.X < 0 ? -1 : 1), chunkPos.Z * 16 + 8 * (chunkPos.Z < 0 ? -1 : 1));
                            if ((playerPos2D - chunkPositionInWorldCoordinates).LengthFast > _renderDistance * _renderDistance + 16)
                                _world.SetChunkToUnload(chunkPos.X, chunkPos.Z);
                        }

                        _world.CheckInvalidations();
                    }
                }));
            _loadingThread.IsBackground = true;
            _loadingThread.Start();
        }

        public void Draw(double deltaTime)
        {
            GL.ClearColor(new Color4(0.6f, 0.8f, 0.85f, 1f));
            GL.Enable(EnableCap.DepthTest);
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.CullFace);

            GL.Disable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);

            var view = Matrix4.CreateTranslation(-(_player.Position + new Vector3(0, 1.7f, 0))) * Matrix4.CreateRotationY(_player.Rotations.Y) * Matrix4.CreateRotationX(_player.Rotations.X);

            //view = Matrix4.LookAt(new Vector3(6, 1, 6), new Vector3(8, 0, 8), new Vector3(0, 1, 0));

            _baseShader.Bind();
            _baseShader.SetUniformMatrix4("proj", false, ref _proj);
            _baseShader.SetUniformMatrix4("view", false, ref view);
            var chunks = _world.GetLoadedChunks();
            foreach(var chunk in chunks)
            {
                chunk.Item2.Draw(_baseShader, _terrainTexture);
            }
            _baseShader.Unbind();

            _guiShader.Bind();
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _guiShader.SetUniformMatrix4("proj", false, ref _guiProj);
            view = Matrix4.CreateTranslation(new Vector3(_text.Position));
            _guiShader.SetUniformMatrix4("view", false, ref view);
            _text.Draw();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            _guiShader.Unbind();
        }

        public void OnLoad(int width, int height)
        {
            OnResize(width, height);
        }

        public void OnResize(int width, int height)
        {
            _width = width;
            _height = height;

            _proj = _camera.ComputeProjectionMatrix(_width / (float)_height);
            _guiProj = Matrix4.CreateOrthographic(_width, _height, 0, 1);
        }

        public void OnUnload()
        {
        }

        public void Update(double deltaTime)
        {
            _world.UnloadChunks();
            _player.Update(deltaTime);

            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.P))
                StaticReferences.ParallelMode = !StaticReferences.ParallelMode;
            _text.Str = $"Direction : {_player.Forward.X} ; {_player.Forward.Y} ; {_player.Forward.Z}\nPosition: {_player.Position.X} ; {_player.Position.Y} ; {_player.Position.Z}\nParallel Mode: {StaticReferences.ParallelMode}";
        }
    }
}

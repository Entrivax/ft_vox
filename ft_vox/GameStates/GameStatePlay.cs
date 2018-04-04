﻿using ft_vox.OpenGL;
using ft_vox.Gameplay;
using ft_vox.Worlds;
using OpenTK;
using System;
using OpenTK.Graphics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ft_vox.Helpers;
using ft_vox.GameManaging;
using ft_vox.Frustum;
using OpenTK.Graphics.ES20;
using OpenTK.Input;
using BlendingFactorDest = OpenTK.Graphics.OpenGL4.BlendingFactorDest;
using BlendingFactorSrc = OpenTK.Graphics.OpenGL4.BlendingFactorSrc;
using ClearBufferMask = OpenTK.Graphics.OpenGL4.ClearBufferMask;
using DepthFunction = OpenTK.Graphics.OpenGL4.DepthFunction;
using EnableCap = OpenTK.Graphics.OpenGL4.EnableCap;
using FrontFaceDirection = OpenTK.Graphics.OpenGL4.FrontFaceDirection;
using GL = OpenTK.Graphics.OpenGL4.GL;
using HintMode = OpenTK.Graphics.OpenGL4.HintMode;
using HintTarget = OpenTK.Graphics.OpenGL4.HintTarget;
using TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter;
using TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter;

namespace ft_vox.GameStates
{
    internal class GameStatePlay : IGameState
    {
        public bool CursorVisible => false;

        private World _world;
        private Player _player;
        private Camera _camera;
        private Plane[] _frustum;
        public static DebugObjects Debug;
        private Mesh _skybox;

        private Texture _terrainTexture;
        private Texture _skyTexture;

        private Shader _baseShader;
        private Shader _debugShader;
        private Shader _guiShader;
        private Shader _skyboxShader;

        private int _width;
        private int _height;
        private Matrix4 _proj;
        private Matrix4 _guiProj;
        private float _framerate;

        private int _renderDistance;
        
        private Text _text;

        private Thread _loadingThread;
        private bool _stopLoading;
        private IGameStateManager _gameStateManager;

        public GameStatePlay(IGameStateManager gameStateManager, World world)
        {
            Debug = new DebugObjects();
            
            _world = world;
            _gameStateManager = gameStateManager;

            _baseShader = ShaderManager.GetWithGeometry("BaseShader");
            _debugShader = ShaderManager.GetWithGeometry("Debug");
            _guiShader = ShaderManager.Get("GuiShader");
            _skyboxShader = ShaderManager.Get("SkyboxShader");
            _player = new Player() { Position = new Vector3(0, 64, 0) };
            _camera = new Camera(new Vector3(0), new Vector3(0), (float)(80f * (Math.PI / 180f)));
            _frustum = new Plane[6];
            for (int i = 0; i < _frustum.Length; i++)
                _frustum[i] = new Plane();
            _terrainTexture = TextureManager.Get("terrain.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _skyTexture = TextureManager.Get("skybox.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _text = new Text(new Vector2(5, 0), FontManager.Get("glyphs"), _guiShader, "");
            _renderDistance = 16;

            _skybox = new Mesh()
            {
                Vertices = new List<Vertex>()
                {
                    // FRONT
                    new Vertex(new Vector3(-10, -10, -10), Vector3.One, new Vector2(1 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, -10, -10), Vector3.One, new Vector2(2 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, 10, -10), Vector3.One, new Vector2(1 / 4f, 1 / 3f)),
                    
                    new Vertex(new Vector3(-10, 10, -10), Vector3.One, new Vector2(1 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(10, -10, -10), Vector3.One, new Vector2(2 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, 10, -10), Vector3.One, new Vector2(2 / 4f, 1 / 3f)),
                    
                    // RIGHT
                    new Vertex(new Vector3(10, -10, -10), Vector3.One, new Vector2(2 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, -10, 10), Vector3.One, new Vector2(3 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, 10, -10), Vector3.One, new Vector2(2 / 4f, 1 / 3f)),
                    
                    new Vertex(new Vector3(10, 10, -10), Vector3.One, new Vector2(2 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(10, -10, 10), Vector3.One, new Vector2(3 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, 10, 10), Vector3.One, new Vector2(3 / 4f, 1 / 3f)),
                    
                    // LEFT
                    new Vertex(new Vector3(-10, -10, 10), Vector3.One, new Vector2(0 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, -10, -10), Vector3.One, new Vector2(1 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, 10, 10), Vector3.One, new Vector2(0 / 4f, 1 / 3f)),
                    
                    new Vertex(new Vector3(-10, 10, 10), Vector3.One, new Vector2(0 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(-10, -10, -10), Vector3.One, new Vector2(1 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, 10, -10), Vector3.One, new Vector2(1 / 4f, 1 / 3f)),
                    
                    // TOP
                    new Vertex(new Vector3(-10, 10, -10), Vector3.One, new Vector2(1 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(10, 10, -10), Vector3.One, new Vector2(2 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(-10, 10, 10), Vector3.One, new Vector2(1 / 4f, 0 / 3f)),
                    
                    new Vertex(new Vector3(-10, 10, 10), Vector3.One, new Vector2(1 / 4f, 0 / 3f)),
                    new Vertex(new Vector3(10, 10, -10), Vector3.One, new Vector2(2 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(10, 10, 10), Vector3.One, new Vector2(2 / 4f, 0 / 3f)),
                    
                    // BOTTOM
                    new Vertex(new Vector3(-10, -10, 10), Vector3.One, new Vector2(1 / 4f, 3 / 3f)),
                    new Vertex(new Vector3(10, -10, 10), Vector3.One, new Vector2(2 / 4f, 3 / 3f)),
                    new Vertex(new Vector3(-10, -10, -10), Vector3.One, new Vector2(1 / 4f, 2 / 3f)),
                    
                    new Vertex(new Vector3(-10, -10, -10), Vector3.One, new Vector2(1 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, -10, 10), Vector3.One, new Vector2(2 / 4f, 3 / 3f)),
                    new Vertex(new Vector3(10, -10, -10), Vector3.One, new Vector2(2 / 4f, 2 / 3f)),
                    
                    // BACK
                    new Vertex(new Vector3(10, -10, 10), Vector3.One, new Vector2(3 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, -10, 10), Vector3.One, new Vector2(4 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(10, 10, 10), Vector3.One, new Vector2(3 / 4f, 1 / 3f)),
                    
                    new Vertex(new Vector3(10, 10, 10), Vector3.One, new Vector2(3 / 4f, 1 / 3f)),
                    new Vertex(new Vector3(-10, -10, 10), Vector3.One, new Vector2(4 / 4f, 2 / 3f)),
                    new Vertex(new Vector3(-10, 10, 10), Vector3.One, new Vector2(4 / 4f, 1 / 3f)),
                }
            };
            _skybox.LoadInGl();
            
            _loadingThread = new Thread(new ThreadStart(
                () =>
                {
                    while (!_stopLoading)
                    {
                        var chunks = _world.GetLoadedChunks();
                        var playerPosition = _player.Position;
                        var playerPos2D = playerPosition.Xz;

                        var chunkPositionsThatCouldBeLoaded = new List<ChunkPosition>();
                        for (int x = -_renderDistance; x <= _renderDistance; x++)
                            for (int z = -_renderDistance; z <= _renderDistance; z++)
                            {
                                var chunkPositionInLocalCoordinates = new Vector2(x, z);
                                if ((chunkPositionInLocalCoordinates).LengthFast < _renderDistance)
                                {
                                    var chunkPosition = new ChunkPosition((int)((chunkPositionInLocalCoordinates.X + playerPos2D.X / 16 + (playerPos2D.X < 0 ? -1 : 0))), (int)((chunkPositionInLocalCoordinates.Y + playerPos2D.Y / 16 + (playerPos2D.Y < 0 ? -1 : 0))));
                                    if (!chunks.Any(chunk => chunk.Item1.Equals(chunkPosition)))
                                        chunkPositionsThatCouldBeLoaded.Add(chunkPosition);
                                }
                            }

                        var orderedChunks = chunkPositionsThatCouldBeLoaded.OrderBy(chunkPosition => (new Vector2(chunkPosition.X * 16 + 8, chunkPosition.Z * 16 + 8) - playerPos2D).LengthSquared).Cast<ChunkPosition?>();
                        var closestChunkToLoad = orderedChunks.FirstOrDefault();
                        if (closestChunkToLoad != null)
                            _world.GetChunkAt(closestChunkToLoad.Value.X, closestChunkToLoad.Value.Z);
                        else
                            Thread.Sleep(1000);

                        foreach (var chunk in chunks)
                        {
                            var chunkPos = chunk.Item1;
                            var chunkPositionInWorldCoordinates = new Vector2(chunkPos.X * 16 + 8, chunkPos.Z * 16 + 8);
                            if ((playerPos2D - chunkPositionInWorldCoordinates).LengthFast > _renderDistance * 16 + 16)
                                _world.SetChunkToUnload(chunkPos.X, chunkPos.Z);
                        }
                    }
                }))
            {
                IsBackground = true
            };
            _loadingThread.Start();
        }
        
        public void Draw(double deltaTime)
        {
            Debug.Clear();

            _world.CheckInvalidations();
            
            _framerate = (float)(1 / deltaTime);
            GL.ClearColor(new Color4(0.6f, 0.8f, 0.85f, 1f));
            
            GL.ClearDepth(1);
            GL.DepthFunc(DepthFunction.Less);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.Enable(EnableCap.CullFace);

            GL.Disable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Fastest);
            
            var cameraPostition = _player.Position + new Vector3(0, 1.7f, 0);
            GL.Disable(EnableCap.DepthTest);
            var skyboxView = Matrix4.CreateRotationY(_player.Rotations.Y) *
                             Matrix4.CreateRotationX(_player.Rotations.X);
            var skyboxColor = new Vector3(1, 1, 1);
            _skyboxShader.Bind();
            _skyboxShader.SetUniformMatrix4("proj", false, ref _proj);
            _skyboxShader.SetUniformMatrix4("view", false, ref skyboxView);
            _skyboxShader.SetUniform3("col", ref skyboxColor);
            _skybox.BindVao(_skyboxShader);
            _skybox.Draw(_skyTexture);
            GL.Enable(EnableCap.DepthTest);

            try
            {
                var hitInfo = Raycast.Cast(_world, cameraPostition, _player.EyeForward, 20f);
                if (hitInfo != null)
                {
                    Debug.AddObject(new DebugObjects.DebugObject
                    {
                        Type = (int)DebugObjects.DebugObject.DebugObjectType.Star,
                        Position = new Vector3(hitInfo.X + 0.5f, hitInfo.Y + 0.5f, hitInfo.Z + 0.5f),
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            var view = Matrix4.CreateTranslation(-cameraPostition) * Matrix4.CreateRotationY(_player.Rotations.Y) * Matrix4.CreateRotationX(_player.Rotations.X);
            
            var mv = (view * _proj);
            mv.Transpose();
            _frustum[0].Nums = mv.Row3 + mv.Row0;
            _frustum[1].Nums = mv.Row3 - mv.Row0;
            _frustum[2].Nums = mv.Row3 - mv.Row1;
            _frustum[3].Nums = mv.Row3 + mv.Row1;
            _frustum[4].Nums = mv.Row2;
            _frustum[5].Nums = mv.Row3 - mv.Row2;
            
            _baseShader.Bind();
            _baseShader.SetUniform3("cameraPosition", ref cameraPostition);
            _baseShader.SetUniformMatrix4("proj", false, ref _proj);
            _baseShader.SetUniformMatrix4("view", false, ref view);
            var chunks = _world.GetVisibleChunks(cameraPostition, _frustum);
            _visibleChunks = chunks.Count;
            _gpuBlocks = chunks.Count > 0 ? chunks.Select(c => c.DisplayableBlocks).Aggregate((a, b) => a + b) : 0;
            TextureManager.Use(_terrainTexture);
            foreach (var chunk in chunks)
            {
                chunk.Draw(_baseShader);
            }
            TextureManager.Disable();
            _baseShader.Unbind();
            
            // START DEBUG DRAWING
            GL.Disable(EnableCap.DepthTest);
            
            _debugShader.Bind();
            _debugShader.SetUniformMatrix4("proj", false, ref _proj);
            _debugShader.SetUniformMatrix4("view", false, ref view);
            Debug.UpdateData();
            Debug.BindVao(_debugShader);
            Debug.Draw();
            _debugShader.Unbind();
            GL.Enable(EnableCap.DepthTest);
            
            // END DEBUG DRAWING

            _guiShader.Bind();
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _guiShader.SetUniformMatrix4("proj", false, ref _guiProj);
            view = Matrix4.CreateTranslation(new Vector3(_text.Position) - new Vector3(_width / 2, _height / 2, 0));
            _guiShader.SetUniformMatrix4("view", false, ref view);
            _text.Draw();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
            _guiShader.Unbind();
        }

        private int _gpuBlocks = 0;
        private int _visibleChunks = 0;

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
            _stopLoading = true;
            _loadingThread.Join();
        }

        private void CleanMeshes()
        {
            Mesh mesh;
            while (StaticReferences.MeshesToClean.TryTake(out mesh))
                mesh.Dispose();
            Blocks3D blocks;
            while (StaticReferences.BlocksToClean.TryTake(out blocks))
                blocks.Dispose();
        }

        public void Update(double deltaTime)
        {
            _world.UnloadChunks();
            CleanMeshes();
            _player.Update(deltaTime);

            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.P))
                StaticReferences.ParallelMode = !StaticReferences.ParallelMode;
            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Plus))
                _renderDistance += 1;
            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Minus))
                _renderDistance = _renderDistance > 1 ? _renderDistance - 1 : _renderDistance;
            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Escape))
            {
                OnUnload();
                _gameStateManager.SetGameState(null);
            }
            if (MouseHelper.IsKeyPressed(MouseButton.Left))
            {
                var hitInfo = Raycast.Cast(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 20f);
                if (hitInfo != null)
                {
                    _world.SetBlockIdAt(hitInfo.X, hitInfo.Y, hitInfo.Z, 0);
                }
            }
            if (MouseHelper.IsKeyPressed(MouseButton.Right))
            {
                var hitInfo = Raycast.Cast(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 20f);
                if (hitInfo != null)
                {
                    var x = hitInfo.X;
                    var y = hitInfo.Y;
                    var z = hitInfo.Z;
                    if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Left)
                        x--;
                    else if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Right)
                        x++;
                    else if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Top)
                        y++;
                    else if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Bottom)
                        y--;
                    else if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Front)
                        z++;
                    else if (hitInfo.Face == Raycast.HitInfo.FaceEnum.Back)
                        z--;
                    _world.SetBlockIdAt(x, y, z, 1);
                }
            }
            
            var txt = $"Framerate: {_framerate:0.0}\nDirection : {_player.EyeForward.X:0.00} ; {_player.EyeForward.Y:0.00} ; {_player.EyeForward.Z:0.00}\nPosition: {_player.Position.X:0.00} ; {_player.Position.Y:0.00} ; {_player.Position.Z:0.00}\nParallel Mode: {StaticReferences.ParallelMode}\nRender distance: {_renderDistance} chunks";
            txt += $"\nVisible chunks: {_visibleChunks}";
            txt += $"\nVisible blocks: {_gpuBlocks}";
            _text.Str = txt;
            _text.Position = new Vector2(5, _height - 5);
        }
    }
}

using ft_vox.OpenGL;
using ft_vox.Gameplay;
using ft_vox.Worlds;
using OpenTK;
using System;
using OpenTK.Graphics;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private Sprite _crosshair;
        private AABBObjects _selectedBlocks;

        private AnimatedSprite _catGifSprite;
        private AnimatedSprite _catGifSprite2;
        private AnimatedSprite _catGifSprite3;
        private AnimatedSprite _catGifSprite4;

        private Texture _terrainTexture;
        private Texture _iconsTexture;
        private Texture _skyTexture;
        private Texture _catTexture;

        private Shader _aabbShader;
        private Shader _baseShader;
        private Shader _debugShader;
        private Shader _guiShader;
        private Shader _skyboxShader;
        private Shader _animatedSpriteShader;

        private int _width;
        private int _height;
        private Matrix4 _proj;
        private Matrix4 _guiProj;
        private float _framerate;

        private int _renderDistance;
        private int _destroySphereRadius = 3;
        private int _selectedBlockId;
        private int _selectedBlockMetadata;
        
        private Text _text;

        private Thread _loadingThread;
        private bool _stopLoading;
        private IGameStateManager _gameStateManager;
        private readonly IWorldManager _worldManager;
        private readonly IChunkManager _chunkManager;
        private readonly IChunkPartManager _chunkPartManager;
        private readonly IBlockSelector _blockSelector;
        private readonly IBlocksProvider _blocksProvider;

        public GameStatePlay(IGameStateManager gameStateManager, IWorldManager worldManager, IChunkManager chunkManager, IChunkPartManager chunkPartManager, IBlockSelector blockSelector, IBlocksProvider blocksProvider, World world)
        {
            Debug = new DebugObjects();
            
            _world = world;
            _gameStateManager = gameStateManager;
            _worldManager = worldManager;
            _chunkManager = chunkManager;
            _chunkPartManager = chunkPartManager;
            _blockSelector = blockSelector;
            _blocksProvider = blocksProvider;
            _selectedBlockId = _blockSelector.GetNextBlock(0);
            _selectedBlockMetadata = 0;
            Image gifImg = Image.FromFile("./Resources/cat.gif");
            FrameDimension dimension = new FrameDimension(gifImg.FrameDimensionsList[0]);
            // Number of frames
            frameCount = gifImg.GetFrameCount(dimension);
            // Return an Image at a certain index
            gifImg.SelectActiveFrame(dimension, 0);
            Bitmap b = new Bitmap(gifImg.Width * frameCount, gifImg.Height);
            using (var g = Graphics.FromImage(b))
                for (int i = 0; i < frameCount; i++)
                {
                    gifImg.SelectActiveFrame(dimension, i);
                    g.DrawImage(gifImg, new Point(gifImg.Width * i, 0));
                }

            _catTexture = TextureManager.Create("cat", b);
            
            _animatedSpriteShader = ShaderManager.Get("AnimatedSprite");
            _catGifSprite = new AnimatedSprite(_catTexture, Vector2.Zero, new Vector2(60, 60),
                new Vector2(1f / frameCount, 1), _animatedSpriteShader);
            _catGifSprite2 = new AnimatedSprite(_catTexture, new Vector2(_width - 60, 0), new Vector2(60, 60),
                new Vector2(1f / frameCount, 1), _animatedSpriteShader, true);
            _catGifSprite3 = new AnimatedSprite(_catTexture, new Vector2(0, _height - 60), new Vector2(60, 60),
                new Vector2(1f / frameCount, 1), _animatedSpriteShader, false, true);
            _catGifSprite4 = new AnimatedSprite(_catTexture, new Vector2(_width - 60, _height - 60), new Vector2(60, 60),
                new Vector2(1f / frameCount, 1), _animatedSpriteShader, true, true);

            _aabbShader = ShaderManager.GetWithGeometry("AabbShader");
            _baseShader = ShaderManager.GetWithGeometry("BaseShader");
            _debugShader = ShaderManager.GetWithGeometry("Debug");
            _guiShader = ShaderManager.Get("GuiShader");
            _skyboxShader = ShaderManager.Get("SkyboxShader");
            _player = new Player() { Position = new Vector3(0, 90, 0) };
            _camera = new Camera(new Vector3(0), new Vector3(0), (float)(80f * (Math.PI / 180f)));
            _frustum = new Plane[6];
            for (int i = 0; i < _frustum.Length; i++)
                _frustum[i] = new Plane();
            _terrainTexture = TextureManager.Get("terrain.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _iconsTexture = TextureManager.Get("icons.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _skyTexture = TextureManager.Get("skybox.png", TextureMinFilter.Nearest, TextureMagFilter.Nearest);
            _text = new Text(new Vector2(5, 0), FontManager.Get("glyphs"), _guiShader, "");
            _renderDistance = 16;

            _crosshair = new Sprite(new Vector2(36, 36), new Vector2(0), new Vector2(16 / 256f), _guiShader);
            _crosshair.UploadInGl();
            
            _selectedBlocks = new AABBObjects();

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
                        var chunks = _worldManager.GetLoadedChunks(_world);
                        var playerPosition = _player.Position;
                        var playerPos2D = playerPosition.Xz;

                        var chunkPositionsThatCouldBeLoaded = new List<ChunkPosition>();
                        for (int x = -_renderDistance; x <= _renderDistance; x++)
                            for (int z = -_renderDistance; z <= _renderDistance; z++)
                            {
                                var chunkPositionInLocalCoordinates = new Vector2(x, z);
                                if ((chunkPositionInLocalCoordinates).LengthFast < _renderDistance)
                                {
                                    var chunkPosition = new ChunkPosition((int)((chunkPositionInLocalCoordinates.X + Math.Floor(playerPos2D.X / 16f))), (int)((chunkPositionInLocalCoordinates.Y + Math.Floor(playerPos2D.Y / 16f))));
                                    if (!chunks.Any(chunk => chunk.Item1.Equals(chunkPosition)))
                                        chunkPositionsThatCouldBeLoaded.Add(chunkPosition);
                                }
                            }

                        var orderedChunks = chunkPositionsThatCouldBeLoaded.OrderBy(chunkPosition => (new Vector2(chunkPosition.X * 16 + 8, chunkPosition.Z * 16 + 8) - playerPos2D).LengthSquared).Cast<ChunkPosition?>();
                        var closestChunkToLoad = orderedChunks.FirstOrDefault();
                        if (closestChunkToLoad != null)
                            _worldManager.GetChunkAt(_world, closestChunkToLoad.Value.X, closestChunkToLoad.Value.Z);
                        else
                            Thread.Sleep(1000);
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
            _selectedBlocks.Clear();

            _chunkPartManager.CheckInvalidations(_world);
            
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
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, cameraPostition, _player.EyeForward, 20f, out hitInfo))
                {
                    _selectedBlocks.AddAABB(new AABBObjects.AABBObject
                    {
                        Position = new Vector3(hitInfo.X - 0.001f, hitInfo.Y - 0.001f, hitInfo.Z - 0.001f),
                        Position2 = new Vector3(hitInfo.X + 1.001f, hitInfo.Y + 1.001f, hitInfo.Z + 1.001f),
                        Color = new Vector4(0, 0, 0, 0.4f),
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
            var chunks = _worldManager.GetVisibleChunks(_world, cameraPostition, _frustum);
            _visibleChunks = chunks.Count;
            TextureManager.Use(_terrainTexture);
            foreach (var chunk in chunks)
            {
                _chunkManager.Draw(chunk, _baseShader);
            }
            TextureManager.Disable();
            _baseShader.Unbind();
            
            GL.Enable(EnableCap.Blend);
            _aabbShader.Bind();
            _aabbShader.SetUniformMatrix4("proj", false, ref _proj);
            _aabbShader.SetUniformMatrix4("view", false, ref view);
            _selectedBlocks.UpdateData();
            _selectedBlocks.BindVao(_aabbShader);
            _selectedBlocks.Draw();
            _aabbShader.Unbind();
            GL.Disable(EnableCap.Blend);
            
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
            
            TextureManager.Use(_iconsTexture);
            view = Matrix4.CreateTranslation(-new Vector3(_crosshair.Size.X / 2, _crosshair.Size.Y / 2, 0));
            _guiShader.SetUniformMatrix4("view", false, ref view);
            _crosshair.Draw();
            TextureManager.Disable();
            _guiShader.Unbind();
            
            _animatedSpriteShader.Bind();
            TextureManager.Use(_catTexture);
            _animatedSpriteShader.SetUniformMatrix4("proj", false, ref _guiProj);
            view = Matrix4.CreateTranslation(new Vector3(_catGifSprite.Position) - new Vector3(_width / 2, _height / 2, 0));
            _animatedSpriteShader.SetUniformMatrix4("view", false, ref view);
            _catGifSprite.Draw(frame);
            view = Matrix4.CreateTranslation(new Vector3(_catGifSprite2.Position) - new Vector3(_width / 2, _height / 2, 0));
            _animatedSpriteShader.SetUniformMatrix4("view", false, ref view);
            _catGifSprite2.Draw(frame);
            view = Matrix4.CreateTranslation(new Vector3(_catGifSprite3.Position) - new Vector3(_width / 2, _height / 2, 0));
            _animatedSpriteShader.SetUniformMatrix4("view", false, ref view);
            _catGifSprite3.Draw(frame);
            view = Matrix4.CreateTranslation(new Vector3(_catGifSprite4.Position) - new Vector3(_width / 2, _height / 2, 0));
            _animatedSpriteShader.SetUniformMatrix4("view", false, ref view);
            _catGifSprite4.Draw(frame);
            TextureManager.Disable();
            _animatedSpriteShader.Unbind();

            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);
        }

        private int _gpuBlocks = 0;
        private int _visibleChunks = 0;
        private int frame = 0;
        private int frameCount = 0;
        private int delay = 0;

        public void OnLoad(int width, int height)
        {
            OnResize(width, height);
        }

        public void OnResize(int width, int height)
        {
            _width = width;
            _height = height;
            
            _catGifSprite2.Position = new Vector2(_width - 60, 0);
            _catGifSprite3.Position = new Vector2(0, _height - 60);
            _catGifSprite4.Position = new Vector2(_width - 60, _height - 60);

            _proj = _camera.ComputeProjectionMatrix(_width / (float)_height);
            _guiProj = Matrix4.CreateOrthographic(_width, _height, 0, 1);
        }

        public void OnUnload()
        {
            _stopLoading = true;
            _loadingThread.Join();
            Debug.Dispose();
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
            delay++;
            if (delay >= 2)
            {
                delay = 0;
                frame = (frame + 1) % frameCount;
            }
            
            var chunks = _worldManager.GetLoadedChunks(_world);
            foreach (var chunk in chunks)
            {
                var chunkPos = chunk.Item1;
                var chunkPositionInWorldCoordinates = new Vector2(chunkPos.X * 16 + 8, chunkPos.Z * 16 + 8);
                if ((_player.Position.Xz - chunkPositionInWorldCoordinates).LengthFast > _renderDistance * 16 + 16)
                    _worldManager.SetChunkToUnload(_world, chunkPos.X, chunkPos.Z);
            }

            _worldManager.UnloadChunks(_world);
            CleanMeshes();
            _player.Update(deltaTime);

            if (KeyboardHelper.IsKeyPressed(Key.P))
                StaticReferences.ParallelMode = !StaticReferences.ParallelMode;
            if (KeyboardHelper.IsKeyPressed(Key.Plus))
                _renderDistance += 1;
            if (KeyboardHelper.IsKeyPressed(Key.Minus))
                _renderDistance = _renderDistance > 1 ? _renderDistance - 1 : _renderDistance;
            if (KeyboardHelper.IsKeyPressed(Key.Escape))
            {
                OnUnload();
                _gameStateManager.SetGameState(null);
            }
            if (MouseHelper.IsKeyPressed(MouseButton.Left))
            {
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 20f, out hitInfo))
                {
                    _worldManager.SetBlockIdAt(_world, hitInfo.X, hitInfo.Y, hitInfo.Z, 0);
                }
            }
            if (MouseHelper.IsKeyPressed(MouseButton.Right))
            {
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 20f, out hitInfo))
                {
                    var x = hitInfo.X;
                    var y = hitInfo.Y;
                    var z = hitInfo.Z;
                    if (hitInfo.Face == HitInfo.FaceEnum.Left)
                        x--;
                    else if (hitInfo.Face == HitInfo.FaceEnum.Right)
                        x++;
                    else if (hitInfo.Face == HitInfo.FaceEnum.Top)
                        y++;
                    else if (hitInfo.Face == HitInfo.FaceEnum.Bottom)
                        y--;
                    else if (hitInfo.Face == HitInfo.FaceEnum.Front)
                        z++;
                    else if (hitInfo.Face == HitInfo.FaceEnum.Back)
                        z--;
                    _worldManager.SetBlockIdAndMetadataAt(_world, x, y, z, (byte)_selectedBlockId, (byte)_selectedBlockMetadata);
                }
            }
            if (KeyboardHelper.IsKeyPressed(Key.Q))
            {
                _selectedBlockId = _blockSelector.GetPreviousBlock(_selectedBlockId);
                _selectedBlockMetadata = 0;
            }
            if (KeyboardHelper.IsKeyPressed(Key.E))
            {
                _selectedBlockId = _blockSelector.GetNextBlock(_selectedBlockId);
                _selectedBlockMetadata = 0;
            }
            if (KeyboardHelper.IsKeyPressed(Key.R))
                _selectedBlockMetadata = _selectedBlockMetadata >= 255 ? 0 : _selectedBlockMetadata + 1;
            if (KeyboardHelper.IsKeyPressed(Key.F))
                _selectedBlockMetadata = _selectedBlockMetadata <= 0 ? 255 : _selectedBlockMetadata - 1;
            if (KeyboardHelper.IsKeyPressed(Key.BracketRight))
                _destroySphereRadius = _destroySphereRadius < 30 ? _destroySphereRadius + 1 : _destroySphereRadius;
            if (KeyboardHelper.IsKeyPressed(Key.BracketLeft))
                _destroySphereRadius = _destroySphereRadius > 1 ? _destroySphereRadius - 1 : _destroySphereRadius;
            if (KeyboardHelper.IsKeyPressed(Key.X))
            {
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 800f, out hitInfo))
                {
                    var impactPoint = new Vector3(hitInfo.X, hitInfo.Y, hitInfo.Z);
                    for (int x = hitInfo.X - _destroySphereRadius + 1; x <= hitInfo.X + _destroySphereRadius - 1; x++)
                    {
                        for (int y = hitInfo.Y - _destroySphereRadius + 1; y <= hitInfo.Y + _destroySphereRadius - 1; y++)
                        {
                            for (int z = hitInfo.Z - _destroySphereRadius + 1; z <= hitInfo.Z + _destroySphereRadius - 1; z++)
                            {
                                if ((new Vector3(x, y, z) - impactPoint).LengthFast <= _destroySphereRadius)
                                    _worldManager.SetBlockIdAt(_world, x, y, z, 0);
                            }
                        }
                    }
                }
            }
            if (KeyboardHelper.IsKeyPressed(Key.C))
            {
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 800f, out hitInfo))
                {
                    var impactPoint = new Vector3(hitInfo.X, hitInfo.Y, hitInfo.Z);
                    for (int x = hitInfo.X - _destroySphereRadius + 1; x <= hitInfo.X + _destroySphereRadius - 1; x++)
                    {
                        for (int y = hitInfo.Y - _destroySphereRadius + 1; y <= hitInfo.Y + _destroySphereRadius - 1; y++)
                        {
                            for (int z = hitInfo.Z - _destroySphereRadius + 1; z <= hitInfo.Z + _destroySphereRadius - 1; z++)
                            {
                                if ((new Vector3(x, y, z) - impactPoint).LengthFast <= _destroySphereRadius)
                                    _worldManager.SetBlockIdAndMetadataAt(_world, x, y, z, (byte)_selectedBlockId, (byte)_selectedBlockMetadata);
                            }
                        }
                    }
                }
            }
            if (KeyboardHelper.IsKeyPressed(Key.V))
            {
                HitInfo hitInfo;
                if (_worldManager.CastRay(_world, _player.Position + new Vector3(0, 1.7f, 0f), _player.EyeForward, 800f, out hitInfo))
                {
                    int[,] cupil = new[,]
                    {
                        {-1, -1, -1, -1, -1, 07, 07, 07, 07, -1, -1, -1, -1},
                        {-1, -1, -1, 07, 07, 08, 08, 08, 08, 07, -1, -1, -1},
                        {-1, -1, 07, 00, 00, 00, 00, 00, 08, 08, 07, -1, -1},
                        {-1, 07, 08, 00, 00, 08, 08, 08, 08, 08, 08, 07, -1},
                        {07, 07, 08, 00, 08, 08, 08, 08, 08, 08, 08, 07, -1},
                        {07, 08, 00, 08, 08, 08, 08, 15, 08, 08, 15, 07, -1},
                        {-1, 07, 08, 08, 08, 07, 06, 08, 08, 08, 08, 06, 07},
                        {07, 08, 08, 08, 08, 08, 07, 07, 07, 07, 07, 07, -1},
                        {07, 00, 08, 07, 08, 08, 08, 08, 08, 08, 07, -1, -1},
                        {07, 00, 07, 08, 07, 07, 08, 07, 07, 07, 08, 07, -1},
                        {-1, 07, 08, 08, 07, -1, 07, -1, -1, -1, 07, -1, -1},
                        {-1, -1, 07, 07, -1, -1, -1, -1, -1, -1, -1, -1, -1},
                    };
                    var impactPoint = new Vector3(hitInfo.X, hitInfo.Y, hitInfo.Z);
                    for (int x = 0; x < 13; x++)
                    {
                        for (int y = 11; y >= 0; y--)
                        {
                            if (cupil[y, x] != -1)
                                _worldManager.SetBlockIdAndMetadataAt(_world, hitInfo.X - 7 + x, hitInfo.Y + 12 - y, hitInfo.Z, (byte)35, (byte)cupil[y, x]);
                        }
                    }
                }
            }
            
            var txt = $"Framerate: {_framerate:0.0}\nDirection : {_player.EyeForward.X:0.00} ; {_player.EyeForward.Y:0.00} ; {_player.EyeForward.Z:0.00}\nPosition: {_player.Position.X:0.00} ; {_player.Position.Y:0.00} ; {_player.Position.Z:0.00}\nParallel Mode: {StaticReferences.ParallelMode}\nRender distance: {_renderDistance} chunks\nDestroy sphere radius: {_destroySphereRadius}\nHand: {_blocksProvider.GetBlockForId((byte)_selectedBlockId).Name}:{_selectedBlockMetadata}";
            txt += $"\nVisible chunks: {_visibleChunks}";
            txt += $"\nVisible blocks: {_gpuBlocks}";
            _text.Str = txt;
            _text.Position = new Vector2(5, _height - 5);
        }
    }
}

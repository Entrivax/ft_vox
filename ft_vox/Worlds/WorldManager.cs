using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ft_vox.Frustum;
using ft_vox.Gameplay;
using ft_vox.GameStates;
using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class WorldManager : IWorldManager
    {
        private readonly IBlocksProvider _blocksProvider;
        private readonly IChunkManager _chunkManager;
        private readonly IChunkGenerator _chunkGenerator;

        public WorldManager(IBlocksProvider blocksProvider, IChunkManager chunkManager, IChunkGenerator chunkGenerator)
        {
            _blocksProvider = blocksProvider;
            _chunkManager = chunkManager;
            _chunkGenerator = chunkGenerator;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ABS(int x)
        {
            return x < 0 ? -x : x;
        }
        
        public Chunk GetChunkAt(World world, int x, int z)
        {
            return ProvideChunk(world, x, z);
        }

        public Chunk GetChunkAtWorldCoordinates(World world, int x, int z)
        {
            return DirectGetChunk(world, x >> 4, z >> 4);
        }

        public byte GetBlockIdAtForCurrentlyLoadedChunks(World world, int x, int y, int z)
        {
            var x4 = x >> 4;
            var z4 = z >> 4;
            var chunk = DirectGetChunk(world, x4, z4);
            return chunk == null ? (byte)0 : _chunkManager.GetBlockId(chunk, (byte)(x - (x4 << 4)), (byte)y, (byte)(z - (z4 << 4)));
        }

        public byte GetBlockIdAt(World world, int x, int y, int z)
        {
            var x4 = x >> 4;
            var z4 = z >> 4;
            var chunk = ProvideChunk(world, x4, z4);
            return _chunkManager.GetBlockId(chunk, (byte)(x - (x4 << 4)), (byte)y, (byte)(z - (z4 << 4)));
        }
        
        public Chunk[] GetSiblingChunks(World world, ChunkPosition chunkPosition)
        {
            var chunks = new Chunk[4];
            chunks[0] = DirectGetChunk(world, chunkPosition.X - 1, chunkPosition.Z);
            chunks[1] = DirectGetChunk(world, chunkPosition.X, chunkPosition.Z + 1);
            chunks[2] = DirectGetChunk(world, chunkPosition.X + 1, chunkPosition.Z);
            chunks[3] = DirectGetChunk(world, chunkPosition.X, chunkPosition.Z - 1);
            return chunks;
        }

        public void SetBlockIdAt(World world, int x, int y, int z, byte blockId)
        {
            SetBlockIdAndMetadataAt(world, x, y, z, blockId, 0);
        }
        
        public void SetBlockIdAndMetadataAt(World world, int x, int y, int z, byte blockId, byte metadata)
        {
            var x40 = x >> 4;
            var z40 = z >> 4;
            var by = (byte)y;
            var chunk0 = ProvideChunk(world, x40, z40);
            _chunkManager.SetBlockIdAndMetadata(chunk0, (byte)(x & 0xF), (byte)y, (byte)(z & 0xF), blockId, metadata);
            _chunkManager.Invalidate(chunk0, by);
            if (y > 0)
                _chunkManager.Invalidate(chunk0, (byte)(y - 1));
            if (y < 255)
                _chunkManager.Invalidate(chunk0, (byte)(y + 1));

            var x41 = (x - 1) >> 4;
            var x42 = (x + 1) >> 4;
            if (x41 != x40)
                _chunkManager.Invalidate(ProvideChunk(world, x41, z40), by);
            else if (x42 != x40)
                _chunkManager.Invalidate(ProvideChunk(world, x42, z40), by);

            var z41 = (z - 1) >> 4;
            var z42 = (z + 1) >> 4;
            if (z41 != z40)
                _chunkManager.Invalidate(ProvideChunk(world, x40, z41), by);
            else if (z42 != z40)
                _chunkManager.Invalidate(ProvideChunk(world, x40, z42), by);
        }
        
        private Chunk ProvideChunk(World world, int x, int z)
        {
            var position = new ChunkPosition(x, z);

            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (!world.ChunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                chunkBlock = new Chunk[8 * 8];
                world.ChunkBlocks.TryAdd(chunkBlockPosition, chunkBlock);
            }
            var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
            if (chunkBlock[chunkIndex] != null)
                return chunkBlock[chunkIndex];
            
            var newChunk = new Chunk();
            _chunkGenerator.PopulateChunk(world, newChunk, position);
            chunkBlock[chunkIndex] = newChunk;

            var chunk = DirectGetChunk(world, x - 1, z);
            if (chunk != null)
                _chunkManager.Invalidate(chunk);
            chunk = DirectGetChunk(world, x + 1, z);
            if (chunk != null)
                _chunkManager.Invalidate(chunk);
            chunk = DirectGetChunk(world, x, z - 1);
            if (chunk != null)
                _chunkManager.Invalidate(chunk);
            chunk = DirectGetChunk(world, x, z + 1);
            if (chunk != null)
                _chunkManager.Invalidate(chunk);
            return newChunk;
        }
        
        public bool CastRay(World world, Vector3 origin, Vector3 direction, float maxDistance, out HitInfo hitInfo)
        {
            var invDir = new Vector3(direction.X == 0 ? float.PositiveInfinity : 1 / direction.X,
                direction.Y == 0 ? float.PositiveInfinity : 1 / direction.Y,
                direction.Z == 0 ? float.PositiveInfinity : 1 / direction.Z);

            hitInfo = new HitInfo();

            var hitChunkBlocks = new List<Tuple<float, KeyValuePair<ChunkPosition, Chunk[]>>>();
            
            foreach (var chunkBlock in world.ChunkBlocks)
            {
                var chunkBlockPosition = chunkBlock.Key;
                var bXMin = chunkBlockPosition.X * 128;
                var bZMin = chunkBlockPosition.Z * 128;
                var bXMax = bXMin + 128;
                var bZMax = bZMin + 128;

                var t1 = (bXMin - origin.X) * invDir.X;
                var t2 = (bXMax - origin.X) * invDir.X;

                var tMin = Math.Min(t1, t2);
                var tMax = Math.Max(t1, t2);
                
                t1 = (bZMin - origin.Z) * invDir.Z;
                t2 = (bZMax - origin.Z) * invDir.Z;
                
                tMin = Math.Max(tMin, Math.Min(t1, t2));
                tMax = Math.Min(tMax, Math.Max(t1, t2));
                
                t1 = -origin.Y * invDir.Y;
                t2 = (256 - origin.Y) * invDir.Y;
                
                tMin = Math.Max(tMin, Math.Min(t1, t2));
                tMax = Math.Min(tMax, Math.Max(t1, t2));

                if (tMax > Math.Max(tMin, 0f))
                {
                    hitChunkBlocks.Add(new Tuple<float, KeyValuePair<ChunkPosition, Chunk[]>>(tMin, chunkBlock));
                }
            }
            
            hitChunkBlocks.Sort((a, b) => a.Item1.CompareTo(b.Item1));

            var hitChunkParts = new List<Tuple<Vector3, Chunk>>();
            
            foreach (var hitChunkBlock in hitChunkBlocks)
            {
                var chunks = hitChunkBlock.Item2.Value;
                var chunkBlockPosition = hitChunkBlock.Item2.Key;
                for (int i = 0; i < chunks.Length; i++)
                {
                    if (chunks[i] == null)
                        continue;
                    
                    var bXMin = ((chunkBlockPosition.X + (chunkBlockPosition.X < 0 ? 1 : 0)) * 8 +
                             (i / 8 * (chunkBlockPosition.X < 0 ? -1 : 1))) * 16;
                    var bZMin = ((chunkBlockPosition.Z + (chunkBlockPosition.Z < 0 ? 1 : 0)) * 8 +
                             (i % 8 * (chunkBlockPosition.Z < 0 ? -1 : 1))) * 16;
                    
                    var bXMax = bXMin + 16;
                    var bZMax = bZMin + 16;
                    
                    var t1 = (bXMin - origin.X) * invDir.X;
                    var t2 = (bXMax - origin.X) * invDir.X;

                    var tMin = Math.Min(t1, t2);
                    var tMax = Math.Max(t1, t2);
                
                    t1 = (bZMin - origin.Z) * invDir.Z;
                    t2 = (bZMax - origin.Z) * invDir.Z;
                
                    tMin = Math.Max(tMin, Math.Min(t1, t2));
                    tMax = Math.Min(tMax, Math.Max(t1, t2));

                    if (tMax > Math.Max(tMin, 0f))
                    {
                        for (int y = 0; y < 256; y += 16)
                        {
                            t1 = (y - origin.Y) * invDir.Y;
                            t2 = (y + 16 - origin.Y) * invDir.Y;

                            var ntMin = Math.Max(tMin, Math.Min(t1, t2));
                            var ntMax = Math.Min(tMax, Math.Max(t1, t2));

                            if (ntMax > Math.Max(ntMin, 0f) && tMin <= maxDistance)
                            {
                                hitChunkParts.Add(new Tuple<Vector3, Chunk>(new Vector3(bXMin, y, bZMin), chunks[i]));
                            }
                        }
                    }
                }
            }

            var closestBlock = new Vector3();
            Chunk chunkHitted = null;
            var nearest = float.PositiveInfinity;
            var hitSide = HitInfo.FaceEnum.Top;
            var hit = false;
            
            foreach (var hitChunkPart in hitChunkParts)
            {
                for (byte x = 0; x < 16; x++)
                {
                    for (int y = (int)hitChunkPart.Item1.Y; y < (int)hitChunkPart.Item1.Y + 16; y++)
                    {
                        for (byte z = 0; z < 16; z++)
                        {
                            if (_chunkManager.GetBlockId(hitChunkPart.Item2, x, (byte) y, z) == 0)
                                continue;
                            
                            var bXMin = hitChunkPart.Item1.X + x;
                            var bYMin = y;
                            var bZMin = hitChunkPart.Item1.Z + z;
                            var bXMax = bXMin + 1;
                            var bYMax = bYMin + 1;
                            var bZMax = bZMin + 1;

                            var t1 = (bXMin - origin.X) * invDir.X;
                            var t2 = (bXMax - origin.X) * invDir.X;

                            var tMin = Math.Min(t1, t2);
                            var tMax = Math.Max(t1, t2);

                            var side = 1;
                            
                            t1 = (bYMin - origin.Y) * invDir.Y;
                            t2 = (bYMax - origin.Y) * invDir.Y;

                            tMin = Math.Max(tMin, Math.Min(t1, t2));
                            tMax = Math.Min(tMax, Math.Max(t1, t2));

                            if (tMin == t1 || tMin == t2)
                            {
                                side = 2;
                            }
                            
                            t1 = (bZMin - origin.Z) * invDir.Z;
                            t2 = (bZMax - origin.Z) * invDir.Z;

                            tMin = Math.Max(tMin, Math.Min(t1, t2));
                            tMax = Math.Min(tMax, Math.Max(t1, t2));
                            
                            if (tMin == t1 || tMin == t2)
                            {
                                side = 3;
                            }
                            
                            if (tMax > Math.Max(tMin, 0f) && tMin <= maxDistance && tMin < nearest)
                            {
                                closestBlock = new Vector3(hitChunkPart.Item1.X + x, y, hitChunkPart.Item1.Z + z);
                                hitSide = side == 1 ? (direction.X > 0 ? HitInfo.FaceEnum.Left : HitInfo.FaceEnum.Right)
                                    : (side == 3 ? (direction.Z > 0 ? HitInfo.FaceEnum.Back : HitInfo.FaceEnum.Front)
                                        : (direction.Y > 0 ? HitInfo.FaceEnum.Bottom : HitInfo.FaceEnum.Top)); 
                                nearest = tMin;
                                hit = true;
                                chunkHitted = hitChunkPart.Item2;
                            }
                        }
                    }
                }
            }

            if (!hit)
                return false;

            hitInfo.Chunk = chunkHitted;
            hitInfo.X = (int)closestBlock.X;
            hitInfo.Y = (int)closestBlock.Y;
            hitInfo.Z = (int)closestBlock.Z;
            hitInfo.Face = hitSide;
            return true;
        }

        public List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks(World world)
        {
            var chunks = new List<Tuple<ChunkPosition, Chunk>>();
            foreach (var chunkBlock in world.ChunkBlocks)
            {
                var chunkBlockPosition = chunkBlock.Key;
                var currentChunks = chunkBlock.Value;
                for(int i = 0; i < currentChunks.Length; i++)
                {
                    if (currentChunks[i] != null)
                    {
                        chunks.Add(new Tuple<ChunkPosition, Chunk>(
                            new ChunkPosition(
                                (chunkBlockPosition.X + (chunkBlockPosition.X < 0 ? 1 : 0)) * 8 + (i / 8 * (chunkBlockPosition.X < 0 ? -1 : 1)),
                                (chunkBlockPosition.Z + (chunkBlockPosition.Z < 0 ? 1 : 0)) * 8 + (i % 8 * (chunkBlockPosition.Z < 0 ? -1 : 1))
                            ), currentChunks[i]));
                    }
                }
            }
            return chunks;
        }

        public void SetChunkToUnload(World world, int x, int z)
        {
            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (world.ChunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
                if (chunkBlock[chunkIndex] != null)
                {
                    world.ChunksToUnload.Add(chunkBlock[chunkIndex]);
                    chunkBlock[chunkIndex] = null;

                    for (int i = 0; i < chunkBlock.Length; i++)
                        if (chunkBlock[i] != null)
                            return;
                    Chunk[] tmp;
                    world.ChunkBlocks.TryRemove(chunkBlockPosition, out tmp);
                }
            }
        }

        public Chunk DirectGetChunk(World world, int x, int z)
        {
            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (world.ChunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
                if (chunkBlock[chunkIndex] != null)
                    return chunkBlock[chunkIndex];
            }
            return null;
        }

        public void UnloadChunks(World world)
        {
            while (world.ChunksToUnload.Count > 0)
            {
                Chunk chunk;
                if (!world.ChunksToUnload.TryTake(out chunk))
                    break;
                _chunkManager.Unload(chunk);
            }
        }

        public void Clean(World world)
        {
            foreach (var c in world.ChunkBlocks)
            {
                for(int i = 0; i < 8; i++)
                    for(int j = 0; j < 8; j++)
                        SetChunkToUnload(world, c.Key.X * 8 + i, c.Key.Z * 8 + j);
            }
            UnloadChunks(world);
        }

        public List<Chunk> GetVisibleChunks(World world, Vector3 cameraPosition, Plane[] frustumPlanes)
        {
            var chunks = new List<Chunk>();
            var aabb = new AABB();
            foreach (var chunkBlock in world.ChunkBlocks)
            {
                var chunkBlockPosition = chunkBlock.Key;
                var currentChunks = chunkBlock.Value;
                
                aabb.Min = new Vector3(chunkBlockPosition.X * 128, 0, chunkBlockPosition.Z * 128);
                aabb.Max = new Vector3(chunkBlockPosition.X * 128 + 128, 256, chunkBlockPosition.Z * 128 + 128);
                        
                if (!FrustumCollision.IsInFrustum(frustumPlanes, aabb, cameraPosition))
                    continue;
                
                for (int i = 0; i < currentChunks.Length; i++)
                {
                    if (currentChunks[i] != null)
                    {
                        var x = ((chunkBlockPosition.X + (chunkBlockPosition.X < 0 ? 1 : 0)) * 8 +
                                (i / 8 * (chunkBlockPosition.X < 0 ? -1 : 1))) * 16;
                        var z = ((chunkBlockPosition.Z + (chunkBlockPosition.Z < 0 ? 1 : 0)) * 8 +
                                (i % 8 * (chunkBlockPosition.Z < 0 ? -1 : 1))) * 16;
                        aabb.Min = new Vector3(x, 0, z);
                        aabb.Max = new Vector3(x + 16, 256, z + 16);
                        
                        if (!FrustumCollision.IsInFrustum(frustumPlanes, aabb, cameraPosition))
                            continue;
                        chunks.Add(currentChunks[i]);
                    }
                }
            }
            return chunks;
        }
    }
}

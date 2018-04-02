using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ft_vox.Frustum;
using ft_vox.GameStates;
using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class ChunkProvider : IChunkProvider
    {
        private ConcurrentDictionary<ChunkPosition, Chunk[]> _chunkBlocks;
        private ConcurrentBag<Chunk> _chunksToUnload;
        private IBlocksProvider _blocksProvider;
        private IChunkGenerator _chunkGenerator;

        public ChunkProvider(IBlocksProvider blocksProvider, IChunkGenerator chunkGenerator)
        {
            _blocksProvider = blocksProvider;
            _chunkBlocks = new ConcurrentDictionary<ChunkPosition, Chunk[]>();
            _chunkGenerator = chunkGenerator;
            _chunksToUnload = new ConcurrentBag<Chunk>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int ABS(int x)
        {
            return x < 0 ? -x : x;
        }

        public Chunk ProvideChunk(World world, int x, int z)
        {
            var position = new ChunkPosition(x, z);

            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (!_chunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                chunkBlock = new Chunk[8 * 8];
                _chunkBlocks.TryAdd(chunkBlockPosition, chunkBlock);
            }
            var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
            if (chunkBlock[chunkIndex] != null)
                return chunkBlock[chunkIndex];
            
            var newChunk = new Chunk(world, _blocksProvider);
            if (_chunkGenerator != null)
                _chunkGenerator.PopulateChunk(newChunk, position);
            else
                PopulateChunk(newChunk);
            chunkBlock[chunkIndex] = newChunk;

            var chunk = DirectGetChunk(x - 1, z);
            chunk?.Invalidate();
            chunk = DirectGetChunk(x + 1, z);
            chunk?.Invalidate();
            chunk = DirectGetChunk(x, z - 1);
            chunk?.Invalidate();
            chunk = DirectGetChunk(x, z + 1);
            chunk?.Invalidate();
            return newChunk;
        }

        public List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks()
        {
            var chunks = new List<Tuple<ChunkPosition, Chunk>>();
            foreach (var chunkBlock in _chunkBlocks)
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

        private void PopulateChunk(Chunk chunk)
        {
            for(byte x = 0; x < 16; x++)
                for (byte y = 0; y < 64; y++)
                    for (byte z = 0; z < 16; z++)
                        chunk.SetBlockId(x, y, z, 1);
        }

        public void SetChunkToUnload(int x, int z)
        {
            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (_chunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
                if (chunkBlock[chunkIndex] != null)
                {
                    _chunksToUnload.Add(chunkBlock[chunkIndex]);
                    chunkBlock[chunkIndex] = null;

                    for (int i = 0; i < chunkBlock.Length; i++)
                        if (chunkBlock[i] != null)
                            return;
                    Chunk[] tmp;
                    _chunkBlocks.TryRemove(chunkBlockPosition, out tmp);
                }
            }
        }

        public Chunk DirectGetChunk(int x, int z)
        {
            var chunkBlockPosition = new ChunkPosition(x / 8 + (x < 0 ? -1 : 0), z / 8 + (z < 0 ? -1 : 0));
            Chunk[] chunkBlock;
            if (_chunkBlocks.TryGetValue(chunkBlockPosition, out chunkBlock))
            {
                var chunkIndex = ((ABS(x) % 8) * 8 + ABS(z) % 8);
                if (chunkBlock[chunkIndex] != null)
                    return chunkBlock[chunkIndex];
            }
            return null;
        }

        public void UnloadChunks()
        {
            while (_chunksToUnload.Count > 0)
            {
                Chunk chunk;
                if (!_chunksToUnload.TryTake(out chunk))
                    break;
                chunk.Unload();
            }
        }

        public void Clean()
        {
            foreach (var c in _chunkBlocks)
            {
                for(int i = 0; i < 8; i++)
                    for(int j = 0; j < 8; j++)
                        SetChunkToUnload(c.Key.X * 8 + i, c.Key.Z * 8 + j);
            }
            UnloadChunks();
        }

        public List<Chunk> GetVisibleChunks(Vector3 cameraPosition, Plane[] frustumPlanes)
        {
            var chunks = new List<Chunk>();
            var aabb = new AABB();
            foreach (var chunkBlock in _chunkBlocks)
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

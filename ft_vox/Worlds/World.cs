using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ft_vox.Worlds
{
    class World
    {
        private IChunkProvider _chunkProvider;

        public World(IChunkProvider chunkProvider)
        {
            _chunkProvider = chunkProvider;
        }

        public Chunk GetChunkAt(int x, int z)
        {
            return _chunkProvider.ProvideChunk(this, x, z);
        }

        public byte GetBlockIdAtForCurrentlyLoadedChunks(int x, int y, int z)
        {
            var x4 = x >> 4;
            var z4 = z >> 4;
            return _chunkProvider.DirectGetChunk(x4, z4)?.GetBlockId((byte)(x - (x4 << 4)), (byte)y, (byte)(z - (z4 << 4))) ?? 0;
        }

        public byte GetBlockIdAt(int x, int y, int z)
        {
            var x4 = x >> 4;
            var z4 = z >> 4;
            return _chunkProvider.ProvideChunk(this, x4, z4).GetBlockId((byte)(x - (x4 << 4)), (byte)y, (byte)(z - (z4 << 4)));
        }

        public void CheckInvalidations()
        {
            var chunks = _chunkProvider.GetLoadedChunks();
            foreach(var chunk in chunks)
            {
                chunk.Item2.CheckInvalidations(chunk.Item1);
            }
        }

        public List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks()
        {
            return _chunkProvider.GetLoadedChunks();
        }

        public void SetChunkToUnload(int x, int z)
        {
            _chunkProvider.SetChunkToUnload(x, z);
        }

        public void UnloadChunks()
        {
            _chunkProvider.UnloadChunks();
        }
    }
}

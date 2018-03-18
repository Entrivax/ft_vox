using System;
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

        public Chunk[] GetSiblingChunks(ChunkPosition chunkPosition)
        {
            var chunks = new Chunk[4];
            chunks[0] = _chunkProvider.DirectGetChunk(chunkPosition.X - 1, chunkPosition.Z);
            chunks[1] = _chunkProvider.DirectGetChunk(chunkPosition.X, chunkPosition.Z + 1);
            chunks[2] = _chunkProvider.DirectGetChunk(chunkPosition.X + 1, chunkPosition.Z);
            chunks[3] = _chunkProvider.DirectGetChunk(chunkPosition.X, chunkPosition.Z - 1);
            return chunks;
        }

        public byte GetBlockIdAt(int x, int y, int z)
        {
            var x4 = x >> 4;
            var z4 = z >> 4;
            return _chunkProvider.ProvideChunk(this, x4, z4).GetBlockId((byte)(x - (x4 << 4)), (byte)y, (byte)(z - (z4 << 4)));
        }

        public void SetBlockIdAt(int x, int y, int z, byte blockId)
        {
            var x40 = x >> 4;
            var z40 = z >> 4;
            var by = (byte)y;
            var chunk0 = _chunkProvider.ProvideChunk(this, x40, z40);
            chunk0.Invalidate(by);
            if (y > 0)
                chunk0.Invalidate((byte)(y - 1));
            if (y < 255)
                chunk0.Invalidate((byte)(y + 1));

            var x41 = (x - 1) >> 4;
            var x42 = (x + 1) >> 4;
            if (x41 != x40)
                _chunkProvider.ProvideChunk(this, x41, z40).Invalidate(by);
            else if (x42 != x40)
                _chunkProvider.ProvideChunk(this, x42, z40).Invalidate(by);

            var z41 = (z - 1) >> 4;
            var z42 = (z + 1) >> 4;
            if (z41 != z40)
                _chunkProvider.ProvideChunk(this, x40, z41).Invalidate(by);
            else if (z42 != z40)
                _chunkProvider.ProvideChunk(this, x40, z42).Invalidate(by);
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

using System;
using System.Collections.Generic;

namespace ft_vox.Worlds
{
    interface IChunkProvider
    {
        Chunk ProvideChunk(World world, int x, int z);
        Chunk DirectGetChunk(int x, int z);
        List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks();
        void SetChunkToUnload(int x, int z);
        void UnloadChunks();
    }
}

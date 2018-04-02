using ft_vox.Frustum;
using OpenTK;
using System;
using System.Collections.Generic;

namespace ft_vox.Worlds
{
    interface IChunkProvider
    {
        Chunk ProvideChunk(World world, int x, int z);
        Chunk DirectGetChunk(int x, int z);
        List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks();
        List<Chunk> GetVisibleChunks(Vector3 cameraPosition, Plane[] frustumPlanes);
        void SetChunkToUnload(int x, int z);
        void UnloadChunks();
    }
}

using ft_vox.Frustum;
using OpenTK;
using System;
using System.Collections.Generic;
using ft_vox.Gameplay;

namespace ft_vox.Worlds
{
    public interface IWorldManager
    {
        Chunk GetChunkAt(World world, int x, int z);
        Chunk GetChunkAtWorldCoordinates(World world, int x, int z);
        byte GetBlockIdAtForCurrentlyLoadedChunks(World world, int x, int y, int z);
        byte GetBlockIdAt(World world, int x, int y, int z);
        Chunk[] GetSiblingChunks(World world, ChunkPosition chunkPosition);
        void SetBlockIdAt(World world, int x, int y, int z, byte blockId);
        void SetBlockIdAndMetadataAt(World world, int x, int y, int z, byte blockId, byte metadata);
        bool CastRay(World world, Vector3 origin, Vector3 direction, float maxDistance, out HitInfo hitInfo);
        List<Tuple<ChunkPosition, Chunk>> GetLoadedChunks(World world);
        void SetChunkToUnload(World world, int x, int z);
        Chunk DirectGetChunk(World world, int x, int z);
        void UnloadChunks(World world);
        void Clean(World world);
        List<Chunk> GetVisibleChunks(World world, Vector3 cameraPosition, Plane[] frustumPlanes);
    }
}

namespace ft_vox.Worlds
{
    public interface IChunkLoader
    {
        void SaveChunk(World world, Chunk chunk, ChunkPosition chunkPosition);
        Chunk LoadChunk(World world, ChunkPosition chunkPosition);
    }
}
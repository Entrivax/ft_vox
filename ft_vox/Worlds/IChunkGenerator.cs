namespace ft_vox.Worlds
{
    interface IChunkGenerator
    {
        void PopulateChunk(World world, Chunk chunk, ChunkPosition position);
    }
}

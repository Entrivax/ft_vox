namespace ft_vox.Worlds
{
    interface IChunkGenerator
    {
        void PopulateChunk(Chunk chunk, ChunkPosition position);
    }
}

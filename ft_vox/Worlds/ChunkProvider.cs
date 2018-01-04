using System.Collections.Concurrent;

namespace ft_vox.Worlds
{
    class ChunkProvider : IChunkProvider
    {
        private ConcurrentDictionary<ChunkPosition, Chunk> _chunks;
        private ConcurrentBag<Chunk> _chunksToUnload;
        private IBlocksProvider _blocksProvider;
        private IChunkGenerator _chunkGenerator;

        public ChunkProvider(IBlocksProvider blocksProvider, IChunkGenerator chunkGenerator)
        {
            _blocksProvider = blocksProvider;
            _chunks = new ConcurrentDictionary<ChunkPosition, Chunk>();
            _chunkGenerator = chunkGenerator;
            _chunksToUnload = new ConcurrentBag<Chunk>();
        }

        public Chunk ProvideChunk(World world, int x, int z)
        {
            var position = new ChunkPosition(x, z);
            Chunk chunk;
            if (_chunks.TryGetValue(position, out chunk))
                return chunk;
            var newChunk = new Chunk(world, _blocksProvider);
            if (_chunkGenerator != null)
                _chunkGenerator.PopulateChunk(newChunk, position);
            else
                PopulateChunk(newChunk);
            _chunks.TryAdd(position, newChunk);
            return newChunk;
        }

        public ConcurrentDictionary<ChunkPosition, Chunk> GetLoadedChunks()
        {
            return _chunks;
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
            Chunk chunk;
            _chunks.TryRemove(new ChunkPosition(x, z), out chunk);
            _chunksToUnload.Add(chunk);
        }

        public Chunk DirectGetChunk(int x, int z)
        {
            var position = new ChunkPosition(x, z);
            Chunk chunk;
            if (_chunks.TryGetValue(position, out chunk))
                return chunk;
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
    }
}

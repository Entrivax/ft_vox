using System.IO;
using System.Linq;

namespace ft_vox.Worlds
{
    public class ChunkLoader : IChunkLoader
    {
        private readonly string _worldsDir;

        public ChunkLoader(string worldsDir)
        {
            _worldsDir = worldsDir;
        }
        
        public void SaveChunk(World world, Chunk chunk, ChunkPosition chunkPosition)
        {
            var worldName = string.Join("_", world.Name.Split(Path.GetInvalidFileNameChars()));
            var worldDir = Path.Combine(_worldsDir, worldName);
            if (!Directory.Exists(worldDir))
                Directory.CreateDirectory(worldDir);

            var chunkBytes = chunk.Blocks.Concat(chunk.BlockMetadatas).Concat(chunk.Humidity)
                .Concat(chunk.Temperatures).ToArray();
            
            File.WriteAllBytes(Path.Combine(worldDir, $"{chunkPosition.X},{chunkPosition.Z}"), chunkBytes);
        }

        public Chunk LoadChunk(World world, ChunkPosition chunkPosition)
        {
            var worldName = string.Join("_", world.Name.Split(Path.GetInvalidFileNameChars()));
            var worldDir = Path.Combine(_worldsDir, worldName);

            var chunkPath = Path.Combine(worldDir, $"{chunkPosition.X},{chunkPosition.Z}");

            if (!File.Exists(chunkPath))
                return null;

            var chunkData = File.ReadAllBytes(chunkPath);

            if (chunkData.Length != 131584)
                return null;
            
            var chunk = new Chunk(chunkData.Take(65536).ToArray(),
                chunkData.Skip(65536).Take(65536).ToArray(),
                chunkData.Skip(131072).Take(256).ToArray(),
                chunkData.Skip(131328).Take(256).ToArray());

            return chunk;
        }
    }
}
using System.Linq;
using ft_vox.OpenGL;

namespace ft_vox.Worlds
{
    public class ChunkManager : IChunkManager
    {
        public Chunk.ChunkBlockInformation GetBlockInformation(Chunk chunk, byte x, byte y, byte z)
        {
            return new Chunk.ChunkBlockInformation
            {
                Id = chunk.Blocks[(y << 8) + x + (z << 4)],
                Metadata = chunk.BlockMetadatas[(y << 8) + x + (z << 4)],
                Humidity = chunk.Humidity[(z << 4) + x],
                Temperature = chunk.Temperatures[(z << 4) + x],
            };
        }

        public byte GetBlockId(Chunk chunk, byte x, byte y, byte z)
        {
            return chunk.Blocks[(y << 8) + x + (z << 4)];
        }

        public byte GetBlockMetadata(Chunk chunk, byte x, byte y, byte z)
        {
            return chunk.BlockMetadatas[(y << 8) + x + (z << 4)];
        }

        public void SetHumidity(Chunk chunk, byte x, byte z, byte humidity)
        {
            chunk.Humidity[(z << 4) + x] = humidity;
        }
        
        public void SetTemperature(Chunk chunk, byte x, byte z, byte temperature)
        {
            chunk.Temperatures[(z << 4) + x] = temperature;
        }
        
        public void SetBlockId(Chunk chunk, byte x, byte y, byte z, byte id)
        {
            SetBlockIdAndMetadata(chunk, x, y, z, id, 0);
        }
        
        public void SetBlockIdAndMetadata(Chunk chunk, byte x, byte y, byte z, byte id, byte metadata)
        {
            chunk.ChunkParts[y >> 4].Invalidated = true;
            var index = (y << 8) + x + (z << 4);
            chunk.Blocks[index] = id;
            chunk.BlockMetadatas[index] = metadata;
        }

        public void Invalidate(Chunk chunk, byte y)
        {
            chunk.ChunkParts[y >> 4].Invalidated = true;
        }

        public void Invalidate(Chunk chunk)
        {
            for (int i = 0; i < chunk.ChunkParts.Length; i++)
                chunk.ChunkParts[i].Invalidated = true;
        }

        private void SetSunlight(Chunk chunk, byte x, byte y, byte z, byte value)
        {
            chunk.ChunkParts[y >> 4].Invalidated = true;
            var loc = (y << 8) + x + (z << 4);
            chunk.LightMap[loc] = (byte)((chunk.LightMap[loc] & 0xf) | (value << 4));
        }

        private byte GetSunlight(Chunk chunk, byte x, byte y, byte z)
        {
            return (byte)((chunk.LightMap[(y << 8) + x + (z << 4)] >> 4) & 0xf);
        }

        private void SetTorchlight(Chunk chunk, byte x, byte y, byte z, byte value)
        {
            chunk.ChunkParts[y >> 4].Invalidated = true;
            var loc = (y << 8) + x + (z << 4);
            chunk.LightMap[loc] = (byte)((chunk.LightMap[loc] & 0xf0) | value);
        }

        private byte GetTorchlight(Chunk chunk, byte x, byte y, byte z)
        {
            return (byte)(chunk.LightMap[(y << 8) + x + (z << 4)] & 0xf);
        }

        public Blocks3D[] GetMeshes(Chunk chunk)
        {
            return chunk.ChunkParts.Select(chunkPart => chunkPart.Blocks).Where(mesh => mesh != null).ToArray();
        }

        public void Draw(Chunk chunk, Shader shader)
        {
            foreach (var chunkPart in chunk.ChunkParts)
            {
                if (chunkPart.Blocks == null)
                    continue;
                if (!chunkPart.Blocks.Loaded && !chunkPart.Invalidated)
                    chunkPart.Blocks.LoadInGl();
                chunkPart.Blocks.BindVao(shader);
                chunkPart.Blocks.Draw();
            }
        }

        public void Unload(Chunk chunk)
        {
            for (int i = 0; i < chunk.ChunkParts.Length; i++)
            {
                var chunkPart = chunk.ChunkParts[i];
                chunkPart.Blocks?.Dispose();
                chunkPart.Blocks = null;
                chunk.ChunkParts[i] = null;
            }
        }
    }
}
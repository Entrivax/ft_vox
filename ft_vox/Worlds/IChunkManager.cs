using ft_vox.OpenGL;

namespace ft_vox.Worlds
{
    public interface IChunkManager
    {
        Chunk.ChunkBlockInformation GetBlockInformation(Chunk chunk, byte x, byte y, byte z);
        byte GetBlockId(Chunk chunk, byte x, byte y, byte z);
        byte GetBlockMetadata(Chunk chunk, byte x, byte y, byte z);
        void SetHumidity(Chunk chunk, byte x, byte z, byte humidity);
        void SetTemperature(Chunk chunk, byte x, byte z, byte temperature);
        void SetBlockId(Chunk chunk, byte x, byte y, byte z, byte id);
        void SetBlockIdAndMetadata(Chunk chunk, byte x, byte y, byte z, byte id, byte metadata);
        void Invalidate(Chunk chunk, byte y);
        void Invalidate(Chunk chunk);
        Blocks3D[] GetMeshes(Chunk chunk);
        void Draw(Chunk chunk, Shader shader);
        void Unload(Chunk chunk);
    }
}
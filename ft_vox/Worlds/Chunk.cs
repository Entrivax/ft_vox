using ft_vox.OpenGL;
using System.Collections.Generic;
using System.Linq;

namespace ft_vox.Worlds
{
    class Chunk
    {
        byte[] blocks;
        byte[] lightMap;
        ChunkPart[] chunkParts;
        protected World world;
        protected IBlocksProvider blocksProvider;
        private bool _invalidated;

        public Chunk(World world, IBlocksProvider blocksProvider)
        {
            this.world = world;
            blocks = new byte[16 * 16 * 256];
            var chunkPartsNumber = 256 / 16;
            chunkParts = new ChunkPart[chunkPartsNumber];
            for (int i = 0; i < chunkPartsNumber; i++)
            {
                chunkParts[i] = new ChunkPart();
            }
            this.blocksProvider = blocksProvider;
        }

        public byte GetBlockId(byte x, byte y, byte z)
        {
            return blocks[(y << 8) + x + (z << 4)];
        }

        public void SetBlockId(byte x, byte y, byte z, byte id)
        {
            _invalidated = true;
            chunkParts[y >> 4].Invalidated = true;
            blocks[(y << 8) + x + (z << 4)] = id;
        }

        private void SetSunlight(byte x, byte y, byte z, byte value)
        {
            _invalidated = true;
            chunkParts[y >> 4].Invalidated = true;
            var loc = (y << 8) + x + (z << 4);
            lightMap[loc] = (byte)((lightMap[loc] & 0xf) | (value << 4));
        }

        private byte GetSunlight(byte x, byte y, byte z)
        {
            return (byte)((lightMap[(y << 8) + x + (z << 4)] >> 4) & 0xf);
        }

        private void SetTorchlight(byte x, byte y, byte z, byte value)
        {
            _invalidated = true;
            chunkParts[y >> 4].Invalidated = true;
            var loc = (y << 8) + x + (z << 4);
            lightMap[loc] = (byte)((lightMap[loc] & 0xf0) | value);
        }

        private byte GetTorchlight(byte x, byte y, byte z)
        {
            return (byte)(lightMap[(y << 8) + x + (z << 4)] & 0xf);
        }

        public void CheckInvalidations(ChunkPosition chunkPosition)
        {
            if (_invalidated)
                for(int i = 0; i < chunkParts.Length; i++)
                {
                    var chunkPart = chunkParts[i];
                    if (chunkPart.Invalidated)
                        chunkPart.ComputeMesh(this, i, chunkPosition);
                }
            _invalidated = false;
        }

        public Mesh[] GetMeshes()
        {
            return chunkParts.Select(chunkPart => chunkPart.Mesh).Where(mesh => mesh != null).ToArray();
        }

        public void Draw(Shader shader, Texture texture)
        {
            foreach (var chunkPart in chunkParts)
            {
                if (chunkPart.Mesh == null)
                    continue;
                if (!chunkPart.Mesh.Loaded && !chunkPart.Invalidated)
                    chunkPart.Mesh.LoadInGl();
                chunkPart.Mesh.BindVao(shader);
                chunkPart.Mesh.Draw(texture);
            }
        }

        public void Unload()
        {
            for (int i = 0; i < chunkParts.Length; i++)
            {
                var chunkPart = chunkParts[i];
                chunkPart.Mesh?.Dispose();
                chunkPart.Mesh = null;
            }
            chunkParts = null;
            blocks = null;
        }

        public class ChunkPart
        {
            public bool Invalidated { get; set; }
            public Mesh Mesh { get; set; }

            public void ComputeMesh(Chunk chunk, int partNumber, ChunkPosition chunkPosition)
            {
                Mesh?.Dispose();
                Mesh = new Mesh();
                var partNumberMultipliedBy16 = partNumber << 4;
                for (byte x = 0; x < 16; x++)
                {
                    for (byte z = 0; z < 16; z++)
                    {
                        for (short y = (short)partNumberMultipliedBy16; y < partNumberMultipliedBy16 + 16; y++)
                        {
                            var blockId = chunk.GetBlockId(x, (byte)y, z);
                            if (blockId == 0)
                                continue;
                            var block = chunk.blocksProvider.GetBlockForId(blockId);
                            var vertices = new List<Vertex>(24);
                            var vertexList = new VertexList(vertices);
                            block.GetVertices(vertexList, chunk.world, (chunkPosition.X << 4) + x, y, (chunkPosition.Z << 4) + z);
                            Mesh.Vertices.AddRange(vertices);
                        }
                    }
                }
                Invalidated = false;
            }
        }
    }
}

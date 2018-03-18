using ft_vox.OpenGL;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public void Invalidate(byte y)
        {
            _invalidated = true;
            chunkParts[y >> 4].Invalidated = true;
        }

        public void Invalidate()
        {
            _invalidated = true;
            for (int i = 0; i < chunkParts.Length; i++)
                chunkParts[i].Invalidated = true;
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
                        //chunkPart.ComputeMesh(this, i, chunkPosition);
                        chunkPart.ComputeBlocks(this, i, chunkPosition, blocksProvider, world);
                }
            _invalidated = false;
        }

        /*public Mesh[] GetMeshes()
        {
            return chunkParts.Select(chunkPart => chunkPart.Mesh).Where(mesh => mesh != null).ToArray();
        }*/
        public Blocks3D[] GetMeshes()
        {
            return chunkParts.Select(chunkPart => chunkPart.Blocks).Where(mesh => mesh != null).ToArray();
        }

        /*public void Draw(Shader shader, Texture texture)
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
        }*/
        public void Draw(Shader shader, Texture texture)
        {
            foreach (var chunkPart in chunkParts)
            {
                if (chunkPart.Blocks == null)
                    continue;
                if (!chunkPart.Blocks.Loaded && !chunkPart.Invalidated)
                    chunkPart.Blocks.LoadInGl();
                chunkPart.Blocks.BindVao(shader);
                chunkPart.Blocks.Draw(texture);
            }
        }

        /*public void Unload()
        {
            for (int i = 0; i < chunkParts.Length; i++)
            {
                var chunkPart = chunkParts[i];
                chunkPart.Mesh?.Dispose();
                chunkPart.Mesh = null;
            }
            chunkParts = null;
            blocks = null;
        }*/
        public void Unload()
        {
            for (int i = 0; i < chunkParts.Length; i++)
            {
                var chunkPart = chunkParts[i];
                chunkPart.Blocks?.Dispose();
                chunkPart.Blocks = null;
            }
            chunkParts = null;
            blocks = null;
        }

        public class ChunkPart
        {
            public bool Invalidated { get; set; }
            /*public Mesh Mesh { get; set; }

            public void ComputeMesh(Chunk chunk, int partNumber, ChunkPosition chunkPosition)
            {
                if (Mesh != null)
                    StaticReferences.MeshesToClean.Add(Mesh);
                Mesh = new Mesh();
                var partNumberMultipliedBy16 = partNumber << 4;
                if (StaticReferences.ParallelMode)
                {
                    List<Vertex>[] allVertices = new List<Vertex>[256];
                    Parallel.For(0, 256, (i) =>
                    {
                        var x = i % 16;
                        var z = i / 16;
                        allVertices[i] = new List<Vertex>(24);
                        for (short y = (short)partNumberMultipliedBy16; y < partNumberMultipliedBy16 + 16; y++)
                        {
                            var blockId = chunk.GetBlockId((byte)x, (byte)y, (byte)z);
                            if (blockId == 0)
                                continue;
                            var block = chunk.blocksProvider.GetBlockForId(blockId);
                            var vertexList = new VertexList(allVertices[i]);
                            block.GetVertices(vertexList, chunk.world, (chunkPosition.X << 4) + x, y, (chunkPosition.Z << 4) + z);
                        }
                    });
                    for (int i = 0; i < 256; i++)
                        Mesh.Vertices.AddRange(allVertices[i]);
                }
                else
                {
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
                }
                Invalidated = false;
            }*/

            public Blocks3D Blocks { get; set; }

            public void ComputeBlocks(Chunk chunk, int partNumber, ChunkPosition chunkPosition, IBlocksProvider blocksProvider, World world)
            {
                if (Blocks != null)
                    StaticReferences.BlocksToClean.Add(Blocks);
                Blocks = new Blocks3D();
                var partNumberMultipliedBy16 = partNumber << 4;

                var siblingChunks = world.GetSiblingChunks(chunkPosition);

                if (StaticReferences.ParallelMode)
                {
                    var blockInfos = new List<BlockInfo>[256];
                    Parallel.For(0, 256, (i) =>
                    {
                        var x = (byte)(i % 16);
                        var z = (byte)(i / 16);

                        var bInfos = new List<BlockInfo>(64);
                        for (short y = (short)partNumberMultipliedBy16; y < partNumberMultipliedBy16 + 16; y++)
                        {
                            var blockId = chunk.GetBlockId(x, (byte)y, z);
                            if (blockId == 0)
                                continue;
                            BlockInfo? blockInfo;
                            if (ComputeBlockInfo(chunk, chunkPosition, blocksProvider, siblingChunks, x, z, y, blockId, out blockInfo))
                            {
                                bInfos.Add((BlockInfo)blockInfo);
                            }
                        }
                        blockInfos[i] = bInfos;
                    });
                    for (int i = 0; i < 256; i++)
                        Blocks.BlockInfos.AddRange(blockInfos[i]);
                }
                else
                {
                    for (byte x = 0; x < 16; x++)
                    {
                        for (byte z = 0; z < 16; z++)
                        {
                            for (short y = (short)partNumberMultipliedBy16; y < partNumberMultipliedBy16 + 16; y++)
                            {
                                var blockId = chunk.GetBlockId(x, (byte)y, z);
                                if (blockId == 0)
                                    continue;
                                BlockInfo? blockInfo;
                                if (ComputeBlockInfo(chunk, chunkPosition, blocksProvider, siblingChunks, x, z, y, blockId, out blockInfo))
                                    Blocks.BlockInfos.Add((BlockInfo)blockInfo);
                            }
                        }
                    }
                }
                Invalidated = false;
            }

            private bool ComputeBlockInfo(Chunk chunk, ChunkPosition chunkPosition, IBlocksProvider blocksProvider, Chunk[] siblingChunks, byte x, byte z, short y, byte blockId, out BlockInfo? blockInfo)
            {
                var blockVisibility = chunk.blocksProvider.GetBlockForId(blockId).IsOpaque ? 0 : 0x3F00;
                if (blockVisibility == 0)
                {
                    if ((x > 0 && blocksProvider.GetBlockForId(chunk.GetBlockId((byte)(x - 1), (byte)y, z))?.IsOpaque != true)
                        || (x == 0 && blocksProvider.GetBlockForId(siblingChunks[0]?.GetBlockId(15, (byte)y, z) ?? 0)?.IsOpaque != true))
                    {
                        blockVisibility |= (int)BlockVisibility.Left;
                    }
                    if ((x < 15 && blocksProvider.GetBlockForId(chunk.GetBlockId((byte)(x + 1), (byte)y, z))?.IsOpaque != true)
                        || (x == 15 && blocksProvider.GetBlockForId(siblingChunks[2]?.GetBlockId(0, (byte)y, z) ?? 0)?.IsOpaque != true))
                    {
                        blockVisibility |= (int)BlockVisibility.Right;
                    }
                    if ((z > 0 && blocksProvider.GetBlockForId(chunk.GetBlockId(x, (byte)y, (byte)(z - 1)))?.IsOpaque != true)
                        || (z == 0 && blocksProvider.GetBlockForId(siblingChunks[3]?.GetBlockId(x, (byte)y, 15) ?? 0)?.IsOpaque != true))
                    {
                        blockVisibility |= (int)BlockVisibility.Front;
                    }
                    if ((z < 15 && blocksProvider.GetBlockForId(chunk.GetBlockId(x, (byte)y, (byte)(z + 1)))?.IsOpaque != true)
                        || (z == 15 && blocksProvider.GetBlockForId(siblingChunks[1]?.GetBlockId(x, (byte)y, 0) ?? 0)?.IsOpaque != true))
                    {
                        blockVisibility |= (int)BlockVisibility.Back;
                    }
                    if (blocksProvider.GetBlockForId(chunk.GetBlockId(x, (byte)(y - 1), z))?.IsOpaque != true)
                    {
                        blockVisibility |= (int)BlockVisibility.Bottom;
                    }
                    if (blocksProvider.GetBlockForId(chunk.GetBlockId(x, (byte)(y + 1), z))?.IsOpaque != true)
                    {
                        blockVisibility |= (int)BlockVisibility.Top;
                    }
                }
                if (blockVisibility != 0)
                {
                    blockInfo = new BlockInfo
                    {
                        BlockIdAndBlockVisibility = blockVisibility | blockId,
                        Position = new Vector3((chunkPosition.X << 4) + x, y, (chunkPosition.Z << 4) + z)
                    };
                    return true;
                }
                blockInfo = null;
                return false;
            }
        }
    }
}

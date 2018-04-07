using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    public class ChunkPartManager : IChunkPartManager
    {
        private readonly IWorldManager _worldManager;
        private readonly IChunkManager _chunkManager;
        private readonly IBlocksProvider _blocksProvider;

        public ChunkPartManager(IWorldManager worldManager, IChunkManager chunkManager, IBlocksProvider blocksProvider)
        {
            _blocksProvider = blocksProvider;
            _chunkManager = chunkManager;
            _worldManager = worldManager;
        }
        
        public void CheckInvalidations(World world)
        {
            var chunks = _worldManager.GetLoadedChunks(world);
            foreach(var chunk in chunks)
            {
                CheckInvalidations(world, chunk.Item2, chunk.Item1);
            }
        }
        
        private void CheckInvalidations(World world, Chunk chunk, ChunkPosition chunkPosition)
        {
            if (chunk.ChunkParts.Any(cp => cp.Invalidated))
                for(int i = 0; i < chunk.ChunkParts.Length; i++)
                {
                    var chunkPart = chunk.ChunkParts[i];
                    if (chunkPart.Invalidated)
                        ComputeBlocks(world, chunk, chunkPart, i, chunkPosition);
                }
        }
        
        private void ComputeBlocks(World world, Chunk chunk, Chunk.ChunkPart chunkPart, int partNumber, ChunkPosition chunkPosition)
        {
            if (chunkPart.Blocks != null)
                StaticReferences.BlocksToClean.Add(chunkPart.Blocks);
            chunkPart.Blocks = new Blocks3D();
            var partNumberMultipliedBy16 = partNumber << 4;

            var siblingChunks = _worldManager.GetSiblingChunks(world, chunkPosition);

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
                        var chunkBlockInfo = _chunkManager.GetBlockInformation(chunk, x, (byte) y, z);
                        if (chunkBlockInfo.Id == 0)
                            continue;
                        BlockInfo? blockInfo;
                        if (ComputeBlockInfo(chunk, chunkPosition, siblingChunks, x, z, y, chunkBlockInfo, out blockInfo))
                        {
                            bInfos.Add((BlockInfo)blockInfo);
                        }
                    }
                    blockInfos[i] = bInfos;
                });
                for (int i = 0; i < 256; i++)
                    chunkPart.Blocks.BlockInfos.AddRange(blockInfos[i]);
            }
            else
            {
                for (byte x = 0; x < 16; x++)
                {
                    for (byte z = 0; z < 16; z++)
                    {
                        for (short y = (short)partNumberMultipliedBy16; y < partNumberMultipliedBy16 + 16; y++)
                        {
                            var chunkBlockInfo = _chunkManager.GetBlockInformation(chunk, x, (byte) y, z);
                            if (chunkBlockInfo.Id == 0)
                                continue;
                            BlockInfo? blockInfo;
                            if (ComputeBlockInfo(chunk, chunkPosition, siblingChunks, x, z, y,
                                chunkBlockInfo, out blockInfo))
                            {
                                chunkPart.Blocks.BlockInfos.Add((BlockInfo)blockInfo);                                    
                            }
                        }
                    }
                }
            }
            chunkPart.Invalidated = false;
        }

        private bool ComputeBlockInfo(Chunk chunk, ChunkPosition chunkPosition, Chunk[] siblingChunks, byte x, byte z, short y, Chunk.ChunkBlockInformation chunkBlockInformation, out BlockInfo? blockInfo)
        {
            var blockVisibility = _blocksProvider.GetBlockForId(chunkBlockInformation.Id).IsOpaque ? 0 : 0x3F00;
            if (blockVisibility == 0)
            {
                if ((x > 0 && _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, (byte)(x - 1), (byte)y, z))?.IsOpaque != true)
                    || (x == 0 && _blocksProvider.GetBlockForId(siblingChunks[0] != null ? _chunkManager.GetBlockId(siblingChunks[0], 15, (byte)y, z) : (byte)0)?.IsOpaque != true))
                {
                    blockVisibility |= (int)BlockVisibility.Left;
                }
                if ((x < 15 && _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, (byte)(x + 1), (byte)y, z))?.IsOpaque != true)
                    || (x == 15 && _blocksProvider.GetBlockForId(siblingChunks[2] != null ? _chunkManager.GetBlockId(siblingChunks[2], 0, (byte)y, z) : (byte)0)?.IsOpaque != true))
                {
                    blockVisibility |= (int)BlockVisibility.Right;
                }
                if ((z > 0 && _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, x, (byte)y, (byte)(z - 1)))?.IsOpaque != true)
                    || (z == 0 && _blocksProvider.GetBlockForId(siblingChunks[3] != null ? _chunkManager.GetBlockId(siblingChunks[3], x, (byte)y, 15) : (byte)0)?.IsOpaque != true))
                {
                    blockVisibility |= (int)BlockVisibility.Front;
                }
                if ((z < 15 && _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, x, (byte)y, (byte)(z + 1)))?.IsOpaque != true)
                    || (z == 15 && _blocksProvider.GetBlockForId(siblingChunks[1] != null ? _chunkManager.GetBlockId(siblingChunks[1], x, (byte)y, 0) : (byte)0)?.IsOpaque != true))
                {
                    blockVisibility |= (int)BlockVisibility.Back;
                }
                if (y == 0 || _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, x, (byte)(y - 1), z))?.IsOpaque != true)
                {
                    blockVisibility |= (int)BlockVisibility.Bottom;
                }
                if (y == 255 || _blocksProvider.GetBlockForId(_chunkManager.GetBlockId(chunk, x, (byte)(y + 1), z))?.IsOpaque != true)
                {
                    blockVisibility |= (int)BlockVisibility.Top;
                }
            }
            if (blockVisibility != 0)
            {
                blockInfo = new BlockInfo
                {
                    BlockIdAndBlockVisibilityAndMetadata = blockVisibility | chunkBlockInformation.Id | (chunkBlockInformation.Metadata << 14),
                    HumidityAndTemperature = chunkBlockInformation.Humidity << 8 | chunkBlockInformation.Temperature,
                    Position = new Vector3((chunkPosition.X << 4) + x, y, (chunkPosition.Z << 4) + z)
                };
                return true;
            }
            blockInfo = null;
            return false;
        }
    }
}
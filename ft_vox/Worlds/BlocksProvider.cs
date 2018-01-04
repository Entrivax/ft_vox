using System;
using System.Collections.Generic;

namespace ft_vox.Worlds
{
    class BlocksProvider : IBlocksProvider
    {
        private Dictionary<byte, IBlock> _blocks;

        public BlocksProvider()
        {
            _blocks = new Dictionary<byte, IBlock>();
        }

        public IBlock GetBlockForId(byte id)
        {
            if (id == 0)
                return null;
            return _blocks[id];
        }

        public void RegisterBlock(byte id, IBlock block)
        {
            if (_blocks.ContainsKey(id))
            {
                throw new ArgumentException($"Block id {id} is already registered!");
            }
            _blocks.Add(id, block);
        }
    }
}

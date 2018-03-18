using System;

namespace ft_vox.Worlds
{
    class BlocksProvider : IBlocksProvider
    {
        private IBlock[] _blocks;

        public BlocksProvider()
        {
            _blocks = new IBlock[256];
        }

        public IBlock GetBlockForId(byte id)
        {
            if (id == 0)
                return null;
            return _blocks[id];
        }

        public void RegisterBlock(byte id, IBlock block)
        {
            if (_blocks[id] != null)
            {
                throw new ArgumentException($"Block id {id} is already registered!");
            }
            _blocks[id] = block;
        }
    }
}

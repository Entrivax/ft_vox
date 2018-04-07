using System;
using System.Collections.Generic;

namespace ft_vox.Gameplay
{
    class BlockSelector : IBlockSelector
    {
        private readonly List<int> _blocks;

        public BlockSelector()
        {
            _blocks = new List<int>();
        }
        
        public void AddSelectableBlock(int id)
        {
            if (_blocks.Contains(id))
            {
                throw new ArgumentException("A block with the same id is already registered!");
            }
            _blocks.Add(id);
            _blocks.Sort();
        }

        public int GetNextBlock(int id)
        {
            if (id <= 0)
                return _blocks[0];
            var index = _blocks.IndexOf(id);
            return index == _blocks.Count - 1 ? _blocks[0] : _blocks[++index];
        }

        public int GetPreviousBlock(int id)
        {
            if (id <= 0)
                return _blocks[0];
            var index = _blocks.IndexOf(id);
            return index == 0 ? _blocks[_blocks.Count - 1] : _blocks[--index];
        }
    }
}
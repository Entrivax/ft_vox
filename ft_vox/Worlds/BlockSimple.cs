using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class BlockSimple : IBlock
    {
        public BlockSimple(string name, bool isOpaque)
        {
            IsOpaque = isOpaque;
            Name = name;
        }

        public bool IsOpaque { get; }
        public string Name { get; }
    }
}

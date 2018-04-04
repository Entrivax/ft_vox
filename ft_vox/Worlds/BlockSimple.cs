using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class BlockSimple : IBlock
    {
        public BlockSimple(bool isOpaque)
        {
            IsOpaque = isOpaque;
        }

        public bool IsOpaque { get; }
    }
}

using ft_vox.OpenGL;

namespace ft_vox.Worlds
{
    interface IBlock
    {
        bool IsOpaque { get; }

        void GetVertices(IVertexList vertices, World world, int x, int y, int z);
    }
}

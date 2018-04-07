using ft_vox.OpenGL;

namespace ft_vox.Worlds
{
    public interface IBlock
    {
        bool IsOpaque { get; }
        
        string Name { get; }
    }
}

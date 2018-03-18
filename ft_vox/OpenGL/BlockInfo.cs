using OpenTK;

namespace ft_vox.OpenGL
{
    public struct BlockInfo
    {
        public Vector3 Position;
        public int BlockIdAndBlockVisibility;
    }

    public enum BlockVisibility
    {
        Top = 256,
        Bottom = 512,
        Front = 1024,
        Back = 2048,
        Left = 4096,
        Right = 8192,
    }
}

using OpenTK;

namespace ft_vox.OpenGL
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Uv;

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            Uv = uv;
        }
    }
}

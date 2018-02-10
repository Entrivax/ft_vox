using OpenTK;

namespace ft_vox.OpenGL
{
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Uv;
        public Vector4 Color;

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            Uv = uv;
            Color = Vector4.One;
        }

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv, Vector4 color)
        {
            Position = position;
            Normal = normal;
            Uv = uv;
            Color = color;
        }
    }
}

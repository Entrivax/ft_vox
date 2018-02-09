using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class BlockTallGrass : IBlock
    {
        public bool IsOpaque => false;
        private byte _textureX;
        private byte _textureY;
        private Vector2[] _uv;

        public BlockTallGrass(int textureId)
        {
            float blockUvSize = 16f / 256f;
            _textureX = (byte)(textureId % 16);
            _textureY = (byte)(textureId / 16);
            _uv = new Vector2[]
            {
                new Vector2(_textureX * blockUvSize, _textureY * blockUvSize),
                new Vector2((_textureX + 0.5f) * blockUvSize, _textureY * blockUvSize),
                new Vector2((_textureX + 0.5f) * blockUvSize, (_textureY + 1) * blockUvSize),
                new Vector2(_textureX * blockUvSize, (_textureY + 1) * blockUvSize),
                new Vector2((_textureX + 1f) * blockUvSize, _textureY * blockUvSize),
                new Vector2((_textureX + 1f) * blockUvSize, (_textureY + 1) * blockUvSize),
            };
            // UV 0----1----4
            //    |  / |  / |
            //    | /  | /  |
            //    3----2----5
        }

        public void GetVertices(IVertexList vertices, World world, int x, int y, int z)
        {
            // Front-Right diagonal
            {
                var normal = new Vector3(0, 0, -1);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), normal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), normal, _uv[4]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), normal, _uv[5]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), normal, _uv[4]));
            }

            // Front-Left diagonal
            {
                var normal = new Vector3(0, 0, 1);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), normal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), normal, _uv[4]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), normal, _uv[5]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), normal, _uv[4]));
            }

            // Back-Right diagonal
            {
                var normal = new Vector3(0, 0, -1);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), normal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), normal, _uv[4]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), normal, _uv[5]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), normal, _uv[4]));
            }

            // Back-Right diagonal
            {
                var normal = new Vector3(0, 0, -1);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), normal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), normal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), normal, _uv[4]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), normal, _uv[5]));

                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y, z + 0.5f), normal, _uv[2]));
                vertices.AddVertex(new Vertex(new Vector3(x + 0.5f, y + 1, z + 0.5f), normal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), normal, _uv[4]));
            }
        }
    }
}

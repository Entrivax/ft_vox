using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class BlockSimple : IBlock
    {
        private IBlocksProvider _blocksProvider;
        private byte _textureX;
        private byte _textureY;
        private Vector2[] _uv;

        public BlockSimple(IBlocksProvider blocksProvider, int textureId)
        {
            _blocksProvider = blocksProvider;
            float blockUvSize = 16f / 256f;
            _textureX = (byte)(textureId % 16);
            _textureY = (byte)(textureId / 16);
            _uv = new Vector2[]
            {
                new Vector2(_textureX * blockUvSize, _textureY * blockUvSize),
                new Vector2((_textureX + 1) * blockUvSize, _textureY * blockUvSize),
                new Vector2((_textureX + 1) * blockUvSize, (_textureY + 1) * blockUvSize),
                new Vector2(_textureX * blockUvSize, (_textureY + 1) * blockUvSize),
            };
            // UV 0----1
            //    |  / |
            //    | /  |
            //    3----2
        }

        public bool IsOpaque => true;

        public void GetVertices(IVertexList vertices, World world, int x, int y, int z)
        {
            // Face Left
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x - 1, y, z))?.IsOpaque != true)
            {
                var leftNormal = new Vector3(-1, 0, 0);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), leftNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), leftNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), leftNormal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), leftNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), leftNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), leftNormal, _uv[1]));
            }

            // Face Right
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x + 1, y, z))?.IsOpaque != true)
            {
                var rightNormal = new Vector3(1, 0, 0);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), rightNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), rightNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), rightNormal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), rightNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), rightNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), rightNormal, _uv[1]));
            }

            // Face Front
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y, z - 1))?.IsOpaque != true)
            {
                var frontNormal = new Vector3(0, 0, -1);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), frontNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), frontNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), frontNormal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z), frontNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), frontNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), frontNormal, _uv[1]));
            }

            // Face Back
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y, z + 1))?.IsOpaque != true)
            {
                var backNormal = new Vector3(0, 0, 1);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), backNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), backNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), backNormal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), backNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), backNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), backNormal, _uv[1]));
            }

            // Face Top
            if (y == 128 || _blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y + 1, z))?.IsOpaque != true)
            {
                var topNormal = new Vector3(0, 1, 0);
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), topNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), topNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), topNormal, _uv[2]));
                
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), topNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), topNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), topNormal, _uv[1]));
            }

            // Face Bottom
            if (y == 0 || _blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y - 1, z))?.IsOpaque != true)
            {
                var bottomNormal = new Vector3(0, -1, 0);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), bottomNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), bottomNormal, _uv[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), bottomNormal, _uv[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), bottomNormal, _uv[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), bottomNormal, _uv[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), bottomNormal, _uv[1]));
            }
        }
    }
}

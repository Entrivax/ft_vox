using ft_vox.OpenGL;
using OpenTK;

namespace ft_vox.Worlds
{
    class BlockSimpleMultiTextured : IBlock
    {
        private IBlocksProvider _blocksProvider;
        private Vector2[] _uvTop;
        private Vector2[] _uvBottom;
        private Vector2[] _uvSide;

        public BlockSimpleMultiTextured(IBlocksProvider blocksProvider, int textureIdTop, int textureIdBottom, int textureIdSide)
        {
            _blocksProvider = blocksProvider;
            float blockUvSize = 16f / 256f;
            byte _textureXTop = (byte)(textureIdTop % 16);
            byte _textureYTop = (byte)(textureIdTop / 16);
            _uvTop = new Vector2[]
            {
                new Vector2(_textureXTop * blockUvSize, _textureYTop * blockUvSize),
                new Vector2((_textureXTop + 1) * blockUvSize, _textureYTop * blockUvSize),
                new Vector2((_textureXTop + 1) * blockUvSize, (_textureYTop + 1) * blockUvSize),
                new Vector2(_textureXTop * blockUvSize, (_textureYTop + 1) * blockUvSize),
            };

            byte _textureXBottom = (byte)(textureIdBottom % 16);
            byte _textureYBottom = (byte)(textureIdBottom / 16);
            _uvBottom = new Vector2[]
            {
                new Vector2(_textureXBottom * blockUvSize, _textureYBottom * blockUvSize),
                new Vector2((_textureXBottom + 1) * blockUvSize, _textureYBottom * blockUvSize),
                new Vector2((_textureXBottom + 1) * blockUvSize, (_textureYBottom + 1) * blockUvSize),
                new Vector2(_textureXBottom * blockUvSize, (_textureYBottom + 1) * blockUvSize),
            };

            byte _textureXSide = (byte)(textureIdSide % 16);
            byte _textureYSide = (byte)(textureIdSide / 16);
            _uvSide = new Vector2[]
            {
                new Vector2(_textureXSide * blockUvSize, _textureYSide * blockUvSize),
                new Vector2((_textureXSide + 1) * blockUvSize, _textureYSide * blockUvSize),
                new Vector2((_textureXSide + 1) * blockUvSize, (_textureYSide + 1) * blockUvSize),
                new Vector2(_textureXSide * blockUvSize, (_textureYSide + 1) * blockUvSize),
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
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), leftNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), leftNormal, _uvSide[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), leftNormal, _uvSide[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), leftNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), leftNormal, _uvSide[0]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), leftNormal, _uvSide[1]));
            }

            // Face Right
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x + 1, y, z))?.IsOpaque != true)
            {
                var rightNormal = new Vector3(1, 0, 0);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), rightNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), rightNormal, _uvSide[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), rightNormal, _uvSide[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), rightNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), rightNormal, _uvSide[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), rightNormal, _uvSide[1]));
            }

            // Face Front
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y, z - 1))?.IsOpaque != true)
            {
                var frontNormal = new Vector3(0, 0, -1);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), frontNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), frontNormal, _uvSide[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), frontNormal, _uvSide[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z), frontNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), frontNormal, _uvSide[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), frontNormal, _uvSide[1]));
            }

            // Face Back
            if (_blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y, z + 1))?.IsOpaque != true)
            {
                var backNormal = new Vector3(0, 0, 1);
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), backNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), backNormal, _uvSide[1]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), backNormal, _uvSide[2]));

                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), backNormal, _uvSide[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), backNormal, _uvSide[0]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), backNormal, _uvSide[1]));
            }

            // Face Top
            if (y == 128 || _blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y + 1, z))?.IsOpaque != true)
            {
                var topNormal = new Vector3(0, 1, 0);
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), topNormal, _uvTop[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), topNormal, _uvTop[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z), topNormal, _uvTop[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z), topNormal, _uvTop[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y + 1, z + 1), topNormal, _uvTop[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y + 1, z + 1), topNormal, _uvTop[1]));
            }

            // Face Bottom
            if (y == 0 || _blocksProvider.GetBlockForId(world.GetBlockIdAtForCurrentlyLoadedChunks(x, y - 1, z))?.IsOpaque != true)
            {
                var bottomNormal = new Vector3(0, -1, 0);
                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), bottomNormal, _uvBottom[3]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), bottomNormal, _uvBottom[1]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z + 1), bottomNormal, _uvBottom[2]));

                vertices.AddVertex(new Vertex(new Vector3(x, y, z + 1), bottomNormal, _uvBottom[3]));
                vertices.AddVertex(new Vertex(new Vector3(x, y, z), bottomNormal, _uvBottom[0]));
                vertices.AddVertex(new Vertex(new Vector3(x + 1, y, z), bottomNormal, _uvBottom[1]));
            }
        }
    }
}

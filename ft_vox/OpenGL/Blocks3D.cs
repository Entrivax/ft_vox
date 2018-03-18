using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ft_vox.OpenGL
{
    public class Blocks3D : IDisposable
    {
        public List<BlockInfo> BlockInfos;
        public bool Loaded { get; private set; }

        private int VerticesCount = -1;
        private Vao<BlockInfo> _vao;
        private Vbo _vbo;
        private int _lastUsedShader;

        public Blocks3D()
        {
            BlockInfos = new List<BlockInfo>();
            Loaded = false;
        }

        public void Dispose()
        {
            Loaded = false;
            ClearVertices();
            VerticesCount = 0;
            _vao?.Dispose();
            _vao = null;
            _vbo?.Dispose();
            _vbo = null;
        }

        public void ClearVertices()
        {
            BlockInfos?.Clear();
            BlockInfos = null;
        }

        public void LoadInGl()
        {
            if (Loaded)
                return;
            VerticesCount = BlockInfos.Count;
            if (BlockInfos.Count == 0)
                return;
            var vertices = BlockInfos.ToArray();
            _vbo = new Vbo();
            _vbo.Bind();
            _vbo.SetData(vertices);
            _vbo.Unbind();

            ClearVertices();
            Loaded = true;
        }

        public void BindVao(Shader shader)
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
                _vao = new Vao<BlockInfo>();
            if (_lastUsedShader != shader.ProgramId)
            {
                _vao.BindVbo(_vbo, shader, new[] {
                    new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector3.SizeInBytes + 4, 0),
                    new VertexAttribute("_blockIdAndVisibility", 1, VertexAttribIntegerType.Int, Vector3.SizeInBytes + 4, Vector3.SizeInBytes),
                });
                _lastUsedShader = shader.ProgramId;
            }
        }
        
        public void Draw(Texture texture)
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
            {
                throw new InvalidOperationException("Vao must be created before drawing");
            }
            TextureManager.Use(texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Points, 0, VerticesCount);
            TextureManager.Disable();
        }
    }
}

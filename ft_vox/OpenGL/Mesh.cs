using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ft_vox.OpenGL
{
    public class Mesh : IDisposable
    {
        public List<Vertex> Vertices;

        public Material Material;
        public bool Loaded { get; private set; }

        private int VerticesCount = -1;
        private Vao<Vertex> _vao;
        private Vbo _vbo;
        private int _lastUsedShader;

        public Mesh()
        {
            Vertices = new List<Vertex>();
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

        public void LoadInGl()
        {
            if (Loaded)
                return;
            VerticesCount = Vertices.Count;
            if (Vertices.Count == 0)
                return;
            var vertices = Vertices.ToArray();
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
                _vao = new Vao<Vertex>();
            if (_lastUsedShader != shader.ProgramId)
            {
                _vao.BindVbo(_vbo, shader, new[] {
                    new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes, 0),
                    new VertexAttribute("_norm", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes, Vector3.SizeInBytes),
                    new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes, Vector3.SizeInBytes * 2),
                });
                _lastUsedShader = shader.ProgramId;
            }
        }

        public void ClearVertices()
        {
            Vertices?.Clear();
            Vertices = null;
        }

        public void Draw()
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
            {
                throw new InvalidOperationException("Vao must be created before drawing");
            }
            if (Material?.Texture != null)
                Draw(Material.Texture);
            else
            {
                _vao.Bind();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
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
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
            TextureManager.Disable();
        }

        public void Draw(PolygonMode mode)
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
            {
                throw new InvalidOperationException("Vao must be created before drawing");
            }
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, mode);
            GL.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
        }
    }
}

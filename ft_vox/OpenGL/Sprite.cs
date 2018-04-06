using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ft_vox.OpenGL
{
    public class Sprite : IDisposable
    {
        public Color4 Color { get; set; } = Color4.White;
        public Shader Shader { get; set; }
        public Vector2 Size { get; }

        private int _trianglesCount;
        private Vao<Vertex> _vao;
        private Vbo _vbo;
        private Vector2 _uvStart;
        private Vector2 _uvEnd;
        
        public Sprite(Vector2 size, Vector2 uvStart, Vector2 uvEnd, Shader shader)
        {
            Size = size;
            Shader = shader;
            _uvStart = uvStart;
            _uvEnd = uvEnd;
        }

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
        }

        public void UploadInGl()
        {
            var vertices = new Vertex[]
            {
                new Vertex(new Vector3(0, 0 + Size.Y, 0f), new Vector3(0, 0, 1), new Vector2(_uvStart.X, _uvStart.Y)),
                new Vertex(new Vector3(0, 0, 0f), new Vector3(0, 0, 1), new Vector2(_uvStart.X, _uvEnd.Y)),
                new Vertex(new Vector3(0 + Size.X, 0 + Size.Y, 0f), new Vector3(0, 0, 1), new Vector2(_uvEnd.X, _uvStart.Y)),

                new Vertex(new Vector3(0 + Size.X, 0 + Size.Y, 0f), new Vector3(0, 0, 1), new Vector2(_uvEnd.X, _uvStart.Y)),
                new Vertex(new Vector3(0, 0, 0f), new Vector3(0, 0, 1), new Vector2(_uvStart.X, _uvEnd.Y)),
                new Vertex(new Vector3(0 + Size.X, 0, 0f), new Vector3(0, 0, 1), new Vector2(_uvEnd.X, _uvEnd.Y)),
            };
            
            if (_vbo == null)
                _vbo = new Vbo();
            _vbo.Bind();
            _trianglesCount = vertices.Length;
            _vbo.SetData(vertices);
            _vbo.Unbind();

            if (_vao == null)
                _vao = new Vao<Vertex>();
            _vao.BindVbo(_vbo, Shader, new[] {
                new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes + Vector4.SizeInBytes, 0),
                new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes + Vector4.SizeInBytes, Vector3.SizeInBytes * 2),
            });
        }

        public void Draw()
        {
            var color = new Vector3(Color.R, Color.G, Color.B);
            Shader.SetUniform3("col", ref color);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _trianglesCount);
            _vao.Unbind();
            TextureManager.Disable();
        }
    }
}
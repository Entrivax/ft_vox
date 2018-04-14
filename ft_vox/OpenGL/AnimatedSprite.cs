using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace ft_vox.OpenGL
{
    public class AnimatedSprite : IDisposable
    {
        private readonly Vector2 _size;
        private readonly Vector2 _uvSize;
        public Color4 Color { get; set; } = Color4.White;
        public Texture Texture { get; set; }
        public Vector2 Position { get; set; }
        public Shader Shader { get; set; }

        private Vao<Vertex> _vao;
        private Vbo _vbo;
        private int _trianglesCount;

        public AnimatedSprite(Texture texture, Vector2 position, Vector2 size, Vector2 uvSize, Shader shader, bool reverseX = false, bool reverseY = false)
        {
            _size = size;
            _uvSize = uvSize;
            Texture = texture;
            Position = position;
            Shader = shader;
            Load(reverseX, reverseY);
        }

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
        }

        private void Load(bool reverseX, bool reverseY)
        {
            var chars = new List<Vertex>(6);

            var x1 = reverseX ? _uvSize.X : 0;
            var x2 = reverseX ? 0 : _uvSize.X;
            var y1 = reverseY ? 1 : 0;
            var y2 = reverseY ? 0 : 1;
            chars.AddRange(new Vertex[]
            {
                new Vertex(new Vector3(0, _size.Y, 0f), new Vector3(0, 0, 1), new Vector2(x1, y1)),
                new Vertex(new Vector3(0, 0, 0f), new Vector3(0, 0, 1), new Vector2(x1, y2)),
                new Vertex(new Vector3(_size.X, _size.Y, 0f), new Vector3(0, 0, 1), new Vector2(x2, y1)),

                new Vertex(new Vector3(_size.X, _size.Y, 0f), new Vector3(0, 0, 1), new Vector2(x2, y1)),
                new Vertex(new Vector3(0, 0, 0f), new Vector3(0, 0, 1), new Vector2(x1, y2)),
                new Vertex(new Vector3(_size.X, 0, 0f), new Vector3(0, 0, 1), new Vector2(x2, y2)),
            });
            
            if (_vbo == null)
                _vbo = new Vbo();
            _vbo.Bind();
            var charsArray = chars.ToArray();
            _trianglesCount = charsArray.Length;
            _vbo.SetData(charsArray);
            _vbo.Unbind();

            if (_vao == null)
                _vao = new Vao<Vertex>();
            _vao.BindVbo(_vbo, Shader, new[] {
                new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes + Vector4.SizeInBytes, 0),
                new VertexAttribute("_uv", 2, VertexAttribPointerType.Float, Vector2.SizeInBytes + Vector3.SizeInBytes + Vector3.SizeInBytes + Vector4.SizeInBytes, Vector3.SizeInBytes * 2),
            });
        }

        public void Draw(int frame)
        {
            var color = new Vector3(Color.R, Color.G, Color.B);
            Shader.SetUniformInt("index", frame);
            var v = _uvSize;
            Shader.SetUniform2("uvSize", ref v);
            Shader.SetUniform3("col", ref color);
            TextureManager.Use(Texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _trianglesCount);
            _vao.Unbind();
            TextureManager.Disable();
        }
    }
}
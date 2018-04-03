using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ft_vox.OpenGL
{
    public class Text : IDisposable
    {
        public Color4 Color { get; set; } = Color4.White;
        public Vector2 Position { get; set; }
        public Font Font { get; set; }
        public Shader Shader { get; set; }
        public Alignment TextAlignment { get; set; } = Alignment.LEFT;

        private Vao<Vertex> _vao;
        private Vbo _vbo;

        public enum Alignment
        {
            LEFT,
            MIDDLE,
            RIGHT
        }

        public Text(Vector2 position, Font font, Shader shader, Alignment textAlignment, string str)
        {
            Position = position;
            Font = font;
            Shader = shader;
            TextAlignment = textAlignment;
            Str = str;
        }

        public Text(Vector2 position, Font font, Shader shader, string str) : this(position, font, shader, Alignment.LEFT, str) { }

        public void Dispose()
        {
            _vao.Dispose();
            _vbo.Dispose();
        }

        private string _str;
        private int _trianglesCount;
        public string Str
        {
            get { return _str; }
            set
            {
                _str = value;
                UpdateString();
            }
        }

        private void UpdateString()
        {
            var chars = new List<Vertex>(3 * _str.Length);
            
            var split = _str.Split(new[] { '\n' }, StringSplitOptions.None);
            var prevY = 0f;
            for (int i = 0; i < split.Length; i++)
            {
                var prevX = 0f;
                var leftPadding = TextAlignment == Alignment.LEFT ? 0 : (TextAlignment == Alignment.RIGHT ? -Font.GetStringWidth(split[i]) : -Font.GetStringWidth(split[i]) / 2);
                prevY -= Font.CharHeight;
                foreach (var c in split[i])
                {
                    var u = Font.GetUFor(c);
                    chars.AddRange(new Vertex[]
                    {
                        new Vertex(new Vector3(prevX + leftPadding, prevY + Font.CharHeight, 0f), new Vector3(0, 0, 1), new Vector2(u, 0)),
                        new Vertex(new Vector3(prevX + leftPadding, prevY, 0f), new Vector3(0, 0, 1), new Vector2(u, Font.CharTextureHeight)),
                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY + Font.CharHeight, 0f), new Vector3(0, 0, 1), new Vector2(u + Font.CharTextureLength, 0)),

                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY + Font.CharHeight, 0f), new Vector3(0, 0, 1), new Vector2(u + Font.CharTextureLength, 0)),
                        new Vertex(new Vector3(prevX + leftPadding, prevY, 0f), new Vector3(0, 0, 1), new Vector2(u, Font.CharTextureHeight)),
                        new Vertex(new Vector3(prevX + leftPadding + Font.CharWidth, prevY, 0f), new Vector3(0, 0, 1), new Vector2(u + Font.CharTextureLength, Font.CharTextureHeight)),
                    });

                    prevX += Font.CharWidth;
                }
            }
            
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

        public void Draw()
        {
            var color = new Vector3(Color.R, Color.G, Color.B);
            Shader.SetUniform3("col", ref color);
            TextureManager.Use(Font.Texture);
            _vao.Bind();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _trianglesCount);
            _vao.Unbind();
            TextureManager.Disable();
        }
    }
}

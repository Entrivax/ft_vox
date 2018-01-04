using OpenTK.Graphics.OpenGL4;

namespace ft_vox.OpenGL
{
    public class VertexAttribute
    {
        private readonly string _name;
        private readonly int _size;
        private readonly VertexAttribPointerType _type;
        private readonly bool _normalize;
        private readonly int _stride;
        private readonly int _offset;

        public VertexAttribute(string name, int size, VertexAttribPointerType type,
            int stride, int offset, bool normalize = false)
        {
            _name = name;
            _size = size;
            _type = type;
            _stride = stride;
            _offset = offset;
            _normalize = normalize;
        }

        public void Set(Shader program)
        {
            int index = program.GetAttribLocation(_name);
            
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, _size, _type, _normalize, _stride, _offset);
        }
    }
}

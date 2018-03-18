using OpenTK.Graphics.OpenGL4;
using System;

namespace ft_vox.OpenGL
{
    public class VertexAttribute
    {
        private readonly string _name;
        private readonly int _size;
        private readonly VertexAttribPointerType _type;
        private readonly VertexAttribIntegerType _integerType;
        private readonly bool _normalize;
        private readonly int _stride;
        private readonly int _offset;
        private readonly bool _isInteger;

        public VertexAttribute(string name, int size, VertexAttribPointerType type,
            int stride, int offset, bool normalize = false)
        {
            _name = name;
            _size = size;
            _type = type;
            _stride = stride;
            _offset = offset;
            _normalize = normalize;
            _isInteger = false;
        }

        public VertexAttribute(string name, int size, VertexAttribIntegerType integerType,
            int stride, int offset)
        {
            _name = name;
            _size = size;
            _integerType = integerType;
            _stride = stride;
            _offset = offset;
            _isInteger = true;
        }

        public void Set(Shader program)
        {
            int index = program.GetAttribLocation(_name);
            
            GL.EnableVertexAttribArray(index);
            if (_isInteger)
                GL.VertexAttribIPointer(index, _size, _integerType, _stride, (IntPtr)_offset);
            else
                GL.VertexAttribPointer(index, _size, _type, _normalize, _stride, _offset);
        }
    }
}

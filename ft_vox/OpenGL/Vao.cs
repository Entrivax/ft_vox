using OpenTK.Graphics.OpenGL4;
using System;

namespace ft_vox.OpenGL
{
    public class Vao<T> : IDisposable where T : struct
    {
        public int Array { get; private set; } = -1;

        public Vao()
        {
            int array;
            GL.GenVertexArrays(1, out array);
            Array = array;
        }

        public void BindVbo(Vbo vbo, Shader shader, VertexAttribute[] attributes)
        {
            Bind();
            vbo.Bind();
            
            foreach(var attrib in attributes)
            {
                attrib.Set(shader);
            }

            Unbind();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Bind()
        {
            GL.BindVertexArray(Array);
        }

        public void Unbind()
        {
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            if (Array != -1)
                GL.DeleteVertexArrays(1, new[] { Array });
            Array = -1;
        }
    }
}


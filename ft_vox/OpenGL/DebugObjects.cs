using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace ft_vox.OpenGL
{
    public class DebugObjects : IDisposable
    {
        public struct DebugObject
        {
            public Vector3 Position;
            public int Type;

            public enum DebugObjectType
            {
                Star = 0,
                Landmark = 1,
            }
        }
        
        public List<DebugObject> Objects;
        public bool Loaded { get; private set; }
        public bool Invalidated { get; private set; }

        private int VerticesCount = -1;
        private Vao<DebugObject> _vao;
        private Vbo _vbo;
        private int _lastUsedShader;

        public DebugObjects()
        {
            Loaded = false;
            Invalidated = true;
            Objects = new List<DebugObject>();
        }

        public void Dispose()
        {
            Loaded = false;
            Objects?.Clear();
            Objects = null;
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
            VerticesCount = Objects.Count;
            if (Objects.Count == 0)
                return;
            var objs = Objects.ToArray();
            _vbo = new Vbo();
            _vbo.Bind();
            _vbo.SetData(objs);
            _vbo.Unbind();

            Loaded = true;
            Invalidated = false;
        }

        public void UpdateData()
        {
            if (!Loaded)
                LoadInGl();
            if (!Invalidated)
                return;
            VerticesCount = Objects.Count;
            var objs = Objects.ToArray();
            _vbo.Bind();
            _vbo.SetData(objs);
            _vbo.Unbind();
        }

        public void AddObject(DebugObject obj)
        {
            Invalidated = true;
            Objects.Add(obj);
        }

        public void Clear()
        {
            Invalidated = true;
            Objects.Clear();
        }

        public void BindVao(Shader shader)
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
                _vao = new Vao<DebugObject>();
            if (_lastUsedShader != shader.ProgramId)
            {
                _vao.BindVbo(_vbo, shader, new[] {
                    new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector3.SizeInBytes + 4, 0),
                    new VertexAttribute("_type", 1, VertexAttribIntegerType.Int, Vector3.SizeInBytes + 4, Vector3.SizeInBytes),
                });
                _lastUsedShader = shader.ProgramId;
            }
        }
        
        public void Draw()
        {
            if (VerticesCount <= 0)
                return;
            if (_vao == null)
            {
                throw new InvalidOperationException("Vao must be created before drawing");
            }
            _vao.Bind();
            GL.DrawArrays(PrimitiveType.Points, 0, VerticesCount);
        }
    }
}
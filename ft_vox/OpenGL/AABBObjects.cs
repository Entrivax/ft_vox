using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace ft_vox.OpenGL
{
    public class AABBObjects : IDisposable
    {
        public struct AABBObject
        {
            public Vector3 Position;
            public Vector3 Position2;
            public Vector4 Color;
        }

        private List<AABBObject> Objects;
        public bool Loaded { get; private set; }
        public bool Invalidated { get; private set; }

        private int VerticesCount = -1;
        private Vao<AABBObject> _vao;
        private Vbo _vbo;
        private int _lastUsedShader;

        public AABBObjects()
        {
            Loaded = false;
            Invalidated = true;
            Objects = new List<AABBObject>();
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
            if (_vbo != null)
            {
                var objs = Objects.ToArray();
                _vbo.Bind();
                _vbo.SetData(objs);
                _vbo.Unbind();
            }
        }

        public void AddAABB(AABBObject obj)
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
                _vao = new Vao<AABBObject>();
            if (_lastUsedShader != shader.ProgramId)
            {
                _vao.BindVbo(_vbo, shader, new[] {
                    new VertexAttribute("_pos", 3, VertexAttribPointerType.Float, Vector3.SizeInBytes * 2 + Vector4.SizeInBytes, 0),
                    new VertexAttribute("_pos2", 3, VertexAttribPointerType.Float, Vector3.SizeInBytes * 2 + Vector4.SizeInBytes, Vector3.SizeInBytes),
                    new VertexAttribute("_col", 4, VertexAttribPointerType.Float, Vector3.SizeInBytes * 2 + Vector4.SizeInBytes, Vector3.SizeInBytes * 2),
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
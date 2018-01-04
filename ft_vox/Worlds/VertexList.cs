using System;
using ft_vox.OpenGL;
using System.Collections.Generic;

namespace ft_vox.Worlds
{
    internal class VertexList : IVertexList
    {
        List<Vertex> _vertices;

        public VertexList(List<Vertex> vertices)
        {
            _vertices = vertices;
        }

        public void AddVertex(Vertex vertex)
        {
            vertex.Position = new OpenTK.Vector3((float)Math.Round(vertex.Position.X, 2), (float)Math.Round(vertex.Position.Y, 2), (float)Math.Round(vertex.Position.Z, 2));
            _vertices.Add(vertex);
        }
    }
}

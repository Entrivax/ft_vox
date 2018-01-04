using ft_vox.Properties;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ft_vox.OpenGL
{
    public class Object3D : IDisposable
    {
        public Mesh[] Meshs;

        public Object3D(Mesh[] meshs)
        {
            Meshs = meshs;
        }

        public Object3D(string path, bool invertX, bool invertY, bool invertZ)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Mesh> meshs = new List<Mesh>();
            Dictionary<string, Material> materials = new Dictionary<string, Material>();
            var mesh = new Mesh();
            Meshs = new[] { mesh };
            using (var reader = File.OpenText(Resources.ResourcesDirectory + path))
            {
                var line = "";
                Mesh currentMesh = null;
                Material currentMaterial = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 4 && split[0] == "v")
                    {
                        points.Add(new Vector3(
                            float.Parse(split[1], CultureInfo.InvariantCulture) * (invertX ? -1 : 1),
                            float.Parse(split[2], CultureInfo.InvariantCulture) * (invertY ? -1 : 1),
                            float.Parse(split[3], CultureInfo.InvariantCulture) * (invertZ ? -1 : 1)));
                    }
                    if ((split.Length == 3 || split.Length == 4) && split[0] == "vt")
                    {
                        uvs.Add(new Vector2(
                            float.Parse(split[1], CultureInfo.InvariantCulture),
                            1 - float.Parse(split[2], CultureInfo.InvariantCulture)));
                    }
                    if (split.Length == 4 && split[0] == "f")
                    {
                        var vert1 = split[1].Split(new[] { '/' }, StringSplitOptions.None);
                        var vert2 = split[2].Split(new[] { '/' }, StringSplitOptions.None);
                        var vert3 = split[3].Split(new[] { '/' }, StringSplitOptions.None);
                        
                        //var uvIndex = int.Parse(vert1[1]) - 1;
                        //currentMesh.Vertices.Add(new Vertex(points[int.Parse(vert1[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                        //uvIndex = int.Parse(vert2[1]) - 1;
                        //currentMesh.Vertices.Add(new Vertex(points[int.Parse(vert2[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                        //uvIndex = int.Parse(vert3[1]) - 1;
                        //currentMesh.Vertices.Add(new Vertex(points[int.Parse(vert3[0]) - 1], uvs.Count > uvIndex ? uvs[uvIndex] : new Vector2(0)));
                    }
                    if (split.Length == 2 && split[0] == "o")
                    {
                        currentMesh = new Mesh
                        {
                            Material = currentMaterial
                        };
                        meshs.Add(currentMesh);
                    }
                    if (split.Length == 2 && split[0] == "usemtl")
                    {
                        currentMaterial = materials[split[1]];
                        if (currentMesh != null)
                            currentMesh.Material = materials[split[1]];
                    }
                    if (split.Length == 2 && split[0] == "mtllib")
                    {
                        var mats = LoadMtl(split[1]);
                        mats.ToList().ForEach(mat => materials.Add(mat.Key, mat.Value));
                    }
                }
            }
            Meshs = meshs.ToArray();
        }

        public void LoadInGl(Shader shader)
        {
            foreach(var mesh in Meshs)
            {
                mesh.LoadInGl();
                mesh.BindVao(shader);
            }
        }

        private Dictionary<string, Material> LoadMtl(string path)
        {
            var materials = new Dictionary<string, Material>();
            using (var reader = File.OpenText(Resources.ResourcesDirectory + path))
            {
                var line = "";
                Material currentMaterial = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 2 && split[0] == "newmtl")
                    {
                        currentMaterial = new Material();
                        materials.Add(split[1], currentMaterial);
                    }
                    if (split.Length == 4 && split[0] == "Kd")
                    {
                        currentMaterial.Color = new Color4(float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture), float.Parse(split[3], CultureInfo.InvariantCulture), 1f);
                    }
                    if (split.Length == 5 && split[0] == "Kd")
                    {
                        currentMaterial.Color = new Color4(float.Parse(split[1], CultureInfo.InvariantCulture), float.Parse(split[2], CultureInfo.InvariantCulture), float.Parse(split[3], CultureInfo.InvariantCulture), float.Parse(split[4], CultureInfo.InvariantCulture));
                    }
                    if (split.Length == 2 && split[0] == "map_Kd")
                    {
                        currentMaterial.Texture = TextureManager.Get(split[1]);
                    }
                }
            }
            return materials;
        }

        public void Draw()
        {
            foreach (var mesh in Meshs)
                mesh.Draw();
        }

        public void Dispose()
        {
            foreach (var mesh in Meshs)
                mesh.Dispose();
            Meshs = null;
        }
    }
}

using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ft_vox.OpenGL
{
    public static class ShaderManager
    {
        private static Dictionary<string, Shader> _shaders = new Dictionary<string, Shader>();

        public static int ShaderInUse { get; private set; }

        public static Shader Get(string name)
        {
            if (_shaders.ContainsKey(name))
                return _shaders[name];
            var shader = new Shader($"Shaders/{name}.vs", $"Shaders/{name}.fs");
            _shaders.Add(name, shader);
            return shader;
        }

        public static void Remove(string name)
        {
            if (_shaders.ContainsKey(name))
            {
                _shaders[name].Dispose();
                _shaders.Remove(name);
            }
        }

        public static void Use(Shader shader)
        {
            if (shader == null)
                throw new ArgumentNullException("Shader cannot be null");
            if (shader.ProgramId == -1)
                throw new ArgumentException("Shader is not loaded");
            GL.UseProgram(shader.ProgramId);
            ShaderInUse = shader.ProgramId;
        }

        public static void EndUse()
        {
            GL.UseProgram(0);
            ShaderInUse = 0;
        }

        public static void Disable()
        {
            GL.UseProgram(0);
        }

        public static void Clear()
        {
            foreach(var shader in _shaders)
            {
                shader.Value.Dispose();
            }
            _shaders.Clear();
        }
    }
}

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.IO;

namespace ft_vox.OpenGL
{
    public class Shader : IDisposable
    {
        public int ProgramId { get; private set; }

        private Dictionary<string, int> _uniformLocations;
        private Dictionary<string, int> _attribLocations;

        public Shader(string vertShader, string fragShader)
        {
            _uniformLocations = new Dictionary<string, int>();
            _attribLocations = new Dictionary<string, int>();
            var vertexShader = CompileShader(ShaderType.VertexShader, vertShader);
            var fragmentShader = CompileShader(ShaderType.FragmentShader, fragShader);

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexShader);
            GL.AttachShader(ProgramId, fragmentShader);
            GL.LinkProgram(ProgramId);

            var info = GL.GetProgramInfoLog(ProgramId);
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            Console.ForegroundColor = currentColor;

            GL.DetachShader(ProgramId, vertexShader);
            GL.DetachShader(ProgramId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public Shader(string vertShader, string geoShader, string fragShader)
        {
            _uniformLocations = new Dictionary<string, int>();
            _attribLocations = new Dictionary<string, int>();
            var vertexShader = CompileShader(ShaderType.VertexShader, vertShader);
            var geometryShader = CompileShader(ShaderType.GeometryShader, geoShader);
            var fragmentShader = CompileShader(ShaderType.FragmentShader, fragShader);

            ProgramId = GL.CreateProgram();
            GL.AttachShader(ProgramId, vertexShader);
            GL.AttachShader(ProgramId, geometryShader);
            GL.AttachShader(ProgramId, fragmentShader);

            GL.LinkProgram(ProgramId);

            var info = GL.GetProgramInfoLog(ProgramId);
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            Console.ForegroundColor = currentColor;

            GL.DetachShader(ProgramId, vertexShader);
            GL.DetachShader(ProgramId, geometryShader);
            GL.DetachShader(ProgramId, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(geometryShader);
            GL.DeleteShader(fragmentShader);
        }

        private int CompileShader(ShaderType type, string path)
        {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, File.ReadAllText(path));
            GL.CompileShader(shader);

            var info = GL.GetShaderInfoLog(shader);
            var currentColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.CompileShader [{type}] for shader located at \"{path}\" had info log: {info}");
            Console.ForegroundColor = currentColor;

            return shader;
        }

        public int GetAttribLocation(string attrib)
        {
            if (_attribLocations.ContainsKey(attrib))
                return _attribLocations[attrib];
            var location = GL.GetAttribLocation(ProgramId, attrib);
            _attribLocations.Add(attrib, location);
            return location;
        }

        public int GetUniformLocation(string uniform)
        {
            if (_uniformLocations.ContainsKey(uniform))
                return _uniformLocations[uniform];
            var location = GL.GetUniformLocation(ProgramId, uniform);
            _uniformLocations.Add(uniform, location);
            return location;
        }

        public void Dispose()
        {
            GL.DeleteProgram(ProgramId);
        }

        public void SetVertexAttrib1(string attrib, double value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib1(location, value);
        }

        public void SetVertexAttrib1(string attrib, float value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib1(location, value);
        }

        public void SetVertexAttrib1(string attrib, short value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib1(location, value);
        }

        public void SetVertexAttrib1(string attrib, int value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib1(location, value);
        }

        public void SetVertexAttrib2(string attrib, Vector2 value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib2(location, value);
        }

        public void SetVertexAttrib3(string attrib, Vector3 value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib3(location, value);
        }

        public void SetVertexAttrib4(string attrib, Vector4 value)
        {
            var location = GetAttribLocation(attrib);
            GL.VertexAttrib4(location, value);
        }

        public void SetUniform1(string attrib, double value)
        {
            var location = GetUniformLocation(attrib);
            GL.Uniform1(location, value);
        }

        public void SetUniform2(string attrib, ref Vector2 value)
        {
            var location = GetUniformLocation(attrib);
            GL.Uniform2(location, ref value);
        }

        public void SetUniform3(string attrib, ref Vector3 value)
        {
            var location = GetUniformLocation(attrib);
            GL.Uniform3(location, ref value);
        }

        public void SetUniform3(string attrib, ref Vector4 value)
        {
            var location = GetUniformLocation(attrib);
            GL.Uniform4(location, ref value);
        }

        public void SetUniformMatrix4(string attrib, bool transpose, ref Matrix4 value)
        {
            var location = GetUniformLocation(attrib);
            GL.UniformMatrix4(location, transpose, ref value);
        }

        public void Bind()
        {
            GL.UseProgram(ProgramId);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }
    }
}

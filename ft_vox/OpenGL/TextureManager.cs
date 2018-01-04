using ft_vox.Properties;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace ft_vox.OpenGL
{
    public static class TextureManager
    {
        private static Dictionary<string, Texture> _textures = new Dictionary<string, Texture>();

        public static Texture Get(string name, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            if (_textures.ContainsKey(name))
                return _textures[name];
            var texture = new Texture(Resources.ResourcesDirectory + name, minFilter, magFilter);
            _textures.Add(name, texture);
            return _textures[name];
        }

        public static void DisposeTexture(string name)
        {
            if (!_textures.ContainsKey(name))
                return;
            var texture = _textures[name];
            _textures.Remove(name);
            texture.Dispose();
        }

        public static void Use(Texture texture)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
        }

        public static void Disable()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public static void Clear()
        {
            foreach(var texture in _textures)
            {
                texture.Value.Dispose();
            }
            _textures.Clear();
        }
    }
}

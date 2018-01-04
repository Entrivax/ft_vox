using ft_vox.Properties;
using System;

namespace ft_vox.OpenGL
{
    public class Font : IDisposable
    {
        private string _availableChars;
        
        public float CharTextureLength { get; private set; }
        public float CharTextureHeight { get; private set; }
        public float CharWidth { get; private set; }
        public float CharHeight { get; private set; }

        public Texture Texture { get; private set; }

        public Font(string path, string availableChars, float charWidth, float charHeight)
        {
            _availableChars = availableChars;
            Texture = new Texture(Resources.ResourcesDirectory + path);
            CharTextureLength = ((charWidth * _availableChars.Length) / Texture.Width) / _availableChars.Length;
            CharTextureHeight = charHeight / Texture.Height;

            CharWidth = charWidth;
            CharHeight = charHeight;
        }

        public float GetUFor(char c)
        {
            return _availableChars.IndexOf(c) * CharTextureLength;
        }

        public float GetStringWidth(string str)
        {
            return str.Length * CharWidth;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}

using System.Collections.Generic;

namespace ft_vox.OpenGL
{
    public static class FontManager
    {
        private static Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        public static Font Get(string name)
        {
            if (_fonts.ContainsKey(name))
                return _fonts[name];
            var font = new Font($"{name}.png", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789=*-+[]{}()|\\,./<>?;:'\"!@#$%^&_", 16, 20);
            _fonts.Add(name, font);
            return font;
        }

        public static void Remove(string name)
        {
            if (_fonts.ContainsKey(name))
            {
                _fonts[name].Dispose();
                _fonts.Remove(name);
            }
        }

        public static void Clear()
        {
            foreach(var font in _fonts)
            {
                font.Value.Dispose();
            }
            _fonts.Clear();
        }
    }
}

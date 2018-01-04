using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;

namespace ft_vox.OpenGL
{
    public class SpriteSheet : IDisposable
    {
        public Shader Shader { get; private set; }
        public Texture Texture { get; private set; }
        public Object3D Mesh { get; private set; }
        public string TexturePath { get; private set; }

        public int SpriteWidth { get; private set; }
        public int SpriteHeight { get; private set; }
        private Vector2 _uv;

        public SpriteSheet(Object3D mesh, string path, int spriteWidth, int spriteHeight, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            Mesh = mesh;
            Shader = ShaderManager.Get("SpriteSheet");
            TexturePath = path;
            Texture = TextureManager.Get(TexturePath, minFilter, magFilter);
            SpriteWidth = spriteWidth;
            SpriteHeight = spriteHeight;
            _uv = new Vector2((float)SpriteWidth / Texture.Width, (float)SpriteHeight / Texture.Height);
        }

        public void Draw(int xSprite, int ySprite)
        {
            var uv = new Vector2(_uv.X * xSprite, _uv.Y * ySprite);
            Shader.SetUniform2("sprite", ref uv);
            TextureManager.Use(Texture);
            Mesh.Draw();
        }

        public void Dispose()
        {
            Texture = null;
        }
    }
}

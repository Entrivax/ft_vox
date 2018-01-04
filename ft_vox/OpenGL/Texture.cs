using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ft_vox.OpenGL
{
    public class Texture : IDisposable
    {
        public int Id { get; private set; } = -1;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture(int width, int height)
        {
            int texId;
            GL.GenTextures(1, out texId);
            Id = texId;
            GL.BindTexture(TextureTarget.Texture2D, Id);

            Width = width;
            Height = height;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, (IntPtr)0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public Texture(string file, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException($"Texture not found : {file}", file);
            var bitmap = new Bitmap(file);
            LoadBitmap(bitmap, true, minFilter, magFilter);
            bitmap = null;
        }

        public Texture(Bitmap bitmap, bool disposeBitmap = false, TextureMinFilter minFilter = TextureMinFilter.Linear, TextureMagFilter magFilter = TextureMagFilter.Linear)
        {
            LoadBitmap(bitmap, disposeBitmap, minFilter, magFilter);
            bitmap = null;
        }

        private void LoadBitmap(Bitmap bitmap, bool disposeBitmap, TextureMinFilter minFilter, TextureMagFilter magFilter)
        {
            int texId;
            GL.GenTextures(1, out texId);
            Id = texId;
            GL.BindTexture(TextureTarget.Texture2D, Id);

            Width = bitmap.Width;
            Height = bitmap.Height;
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bitmap.UnlockBits(data);

            if (disposeBitmap)
                bitmap.Dispose();

            bitmap = null;

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            if(Id != -1)
                GL.DeleteTexture(Id);
        }
    }
}

using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ft_vox.OpenGL
{
    public class Framebuffer : IDisposable
    {
        public int Id { get; private set; } = -1;
        public int RenderBufferId { get; private set; } = -1;
        public Texture Texture { get; private set; }

        public Framebuffer(int width, int height)
        {
            Id = GL.GenFramebuffer();
            RenderBufferId = GL.GenRenderbuffer();
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, RenderBufferId);
            UpdateSize(width, height);
        }

        public void UpdateSize(int width, int height)
        {
            if (Texture != null)
                Texture.Dispose();
            Texture = new Texture(width, height);

            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RenderBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent, width, height);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Texture.Id, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Id);
        }

        public void Unbind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteRenderbuffer(RenderBufferId);
            RenderBufferId = -1;
            GL.DeleteFramebuffer(Id);
            Id = -1;
            if (Texture != null)
                Texture.Dispose();
        }
    }
}

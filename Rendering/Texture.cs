using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.IO;

namespace Assignment_4.Rendering
{
    public class Texture : IDisposable
    {
        private int _handle;
        // Add this to your Texture class
        public int GetTextureId() => _handle;
        public Texture(string path)
        {
            _handle = GL.GenTexture();
            Use();

            // Load image using StbImageSharp
            StbImage.stbi_set_flip_vertically_on_load(1); // Flip texture vertically

            ImageResult image;
            using (var stream = File.OpenRead(path))
            {
                image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
            }

            if (image == null)
            {
                throw new Exception($"Failed to load texture: {path}");
            }

            // Upload texture to GPU
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

            // Generate mipmaps for better quality at distance
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Set anisotropic filtering for better quality
            float maxAniso;
            GL.GetFloat((GetPName)0x84FF, out maxAniso); // GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName)0x84FE, maxAniso); // GL_TEXTURE_MAX_ANISOTROPY_EXT

            Console.WriteLine($"✓ Texture loaded: {Path.GetFileName(path)} ({image.Width}x{image.Height})");
        }
        public Texture(int textureId)
        {
            _handle = textureId;
        }
        public void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, _handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
        }
    }
}
using Assignment_4;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using StbImageSharp;
using System;
using System.IO;

namespace Assignment_4
{
    public class Game : GameWindow
    {
        // GL objects
        private int _vaoHandle, _vboHandle, _eboHandle;
        private int _textureHandle;
        private Shader _shaderProgram;
        private readonly Cube _cube = new Cube();

        // Uniforms
        private int _mvpUniform;

        // State
        private float _spinRadians;

        public Game(GameWindowSettings gw, NativeWindowSettings nw) : base(gw, nw) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Basics
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Enable(EnableCap.DepthTest);

            // GPU buffers & vertex layout
            CreateGeometryBuffers();

            // Build absolute paths from executable directory (bin/Debug/net8.0)
            string basePath = AppContext.BaseDirectory;

            // Shaders
            string vertexPath = Path.Combine(basePath, "Shaders", "shader.vert");
            string fragmentPath = Path.Combine(basePath, "Shaders", "shader.frag");
            _shaderProgram = new Shader(vertexPath, fragmentPath);
            _shaderProgram.Use();

            // Texture
            string texPath = Path.Combine(basePath, "Textures", "checkerboard.jpg");
            _textureHandle = LoadTextureFromFile(texPath);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
            _shaderProgram.SetInt("tex0", 0);

            // Cache uniform location
            _mvpUniform = GL.GetUniformLocation(_shaderProgram.Handle, "mvp");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            _spinRadians += (float)args.Time; // same rotation behavior
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shaderProgram.Use();
            GL.BindVertexArray(_vaoHandle);

            // Matrices: unchanged math/order
            Matrix4 model = Matrix4.CreateRotationY(_spinRadians);
            Matrix4 view = Matrix4.LookAt(new Vector3(2f, 2f, 3f), Vector3.Zero, Vector3.UnitY);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                Size.X / (float)Size.Y,
                0.1f,
                100f
            );

            Matrix4 mvp = model * view * proj; // same multiplication order
            GL.UniformMatrix4(_mvpUniform, false, ref mvp);

            GL.DrawElements(PrimitiveType.Triangles, _cube.Indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

        // --- helpers (structure-only refactor; behavior unchanged) ----------------

        private void CreateGeometryBuffers()
        {
            _vaoHandle = GL.GenVertexArray();
            _vboHandle = GL.GenBuffer();
            _eboHandle = GL.GenBuffer();

            GL.BindVertexArray(_vaoHandle);

            // Vertex data
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer,
                          _cube.Vertices.Length * sizeof(float),
                          _cube.Vertices,
                          BufferUsageHint.StaticDraw);

            // Index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
                          _cube.Indices.Length * sizeof(uint),
                          _cube.Indices,
                          BufferUsageHint.StaticDraw);

            // Layout: vec3 position + vec2 uv (identical to original)
            const int stride = 5 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        private int LoadTextureFromFile(string path)
        {
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);

            using (var stream = File.OpenRead(path))
            {
                var img = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D,
                              0,
                              PixelInternalFormat.Rgba,
                              img.Width,
                              img.Height,
                              0,
                              PixelFormat.Rgba,
                              PixelType.UnsignedByte,
                              img.Data);
            }

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Same sampler parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            return tex;
        }
    }
}

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.IO;

namespace Assignment_4
{
    public class Game : GameWindow
    {
        // GL objects
        private int _vaoHandle, _vboHandle, _eboHandle;
        private Shader _shaderProgram;
        private readonly Cube _cube = new Cube();

        // State
        private float _spinRadians;
        private Vector3 _lightPos = new Vector3(2.0f, 2.0f, 2.0f);
        private Vector3 _cameraPos = new Vector3(2f, 2f, 3f);

        // Camera control
        private bool _firstMove = true;
        private Vector2 _lastMousePos;
        private float _cameraYaw = -90f;
        private float _cameraPitch = 0f;
        private Vector3 _cameraFront = -Vector3.UnitZ;

        public Game(GameWindowSettings gw, NativeWindowSettings nw) : base(gw, nw)
        {
            CursorState = CursorState.Grabbed; // Capture mouse for camera control
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            // Basics
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Enable(EnableCap.DepthTest);

            // GPU buffers & vertex layout
            CreateGeometryBuffers();

            // Build shader paths
            string basePath = AppContext.BaseDirectory;
            string vertexPath = Path.Combine(basePath, "Shaders", "phong.vert");
            string fragmentPath = Path.Combine(basePath, "Shaders", "phong.frag");
            _shaderProgram = new Shader(vertexPath, fragmentPath);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Rotate cube
            _spinRadians += (float)args.Time;

            var input = KeyboardState;
            float cameraSpeed = 2.5f * (float)args.Time;

            // Camera movement (WASD)
            if (input.IsKeyDown(Keys.W))
                _cameraPos += cameraSpeed * _cameraFront;
            if (input.IsKeyDown(Keys.S))
                _cameraPos -= cameraSpeed * _cameraFront;
            if (input.IsKeyDown(Keys.A))
                _cameraPos -= Vector3.Normalize(Vector3.Cross(_cameraFront, Vector3.UnitY)) * cameraSpeed;
            if (input.IsKeyDown(Keys.D))
                _cameraPos += Vector3.Normalize(Vector3.Cross(_cameraFront, Vector3.UnitY)) * cameraSpeed;

            // Light movement (Arrow keys)
            if (input.IsKeyDown(Keys.Up))
                _lightPos.Y += cameraSpeed;
            if (input.IsKeyDown(Keys.Down))
                _lightPos.Y -= cameraSpeed;
            if (input.IsKeyDown(Keys.Left))
                _lightPos.X -= cameraSpeed;
            if (input.IsKeyDown(Keys.Right))
                _lightPos.X += cameraSpeed;

            // Exit on Escape
            if (input.IsKeyDown(Keys.Escape))
                Close();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            if (_firstMove)
            {
                _lastMousePos = new Vector2(e.X, e.Y);
                _firstMove = false;
                return;
            }

            float deltaX = e.X - _lastMousePos.X;
            float deltaY = _lastMousePos.Y - e.Y; // Reversed: y-coordinates go from bottom to top
            _lastMousePos = new Vector2(e.X, e.Y);

            float sensitivity = 0.1f;
            deltaX *= sensitivity;
            deltaY *= sensitivity;

            _cameraYaw += deltaX;
            _cameraPitch += deltaY;

            // Constrain pitch
            if (_cameraPitch > 89.0f)
                _cameraPitch = 89.0f;
            if (_cameraPitch < -89.0f)
                _cameraPitch = -89.0f;

            // Update camera front vector
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(_cameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(_cameraPitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(_cameraPitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(_cameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(_cameraPitch));
            _cameraFront = Vector3.Normalize(front);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shaderProgram.Use();
            GL.BindVertexArray(_vaoHandle);

            // Matrices
            Matrix4 model = Matrix4.CreateRotationY(_spinRadians);
            Matrix4 view = Matrix4.LookAt(_cameraPos, _cameraPos + _cameraFront, Vector3.UnitY);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f),
                Size.X / (float)Size.Y,
                0.1f,
                100f
            );

            // Pass matrices to shader
            _shaderProgram.SetMatrix4("model", model);
            _shaderProgram.SetMatrix4("view", view);
            _shaderProgram.SetMatrix4("projection", projection);

            // Pass lighting parameters
            _shaderProgram.SetVector3("lightPos", _lightPos);
            _shaderProgram.SetVector3("viewPos", _cameraPos);
            _shaderProgram.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shaderProgram.SetVector3("objectColor", new Vector3(0.8f, 0.3f, 0.3f));

            GL.DrawElements(PrimitiveType.Triangles, _cube.Indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }

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

            // Layout: vec3 position (location 0) + vec3 normal (location 1)
            const int stride = 6 * sizeof(float);

            // Position attribute
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            // Normal attribute
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vboHandle);
            GL.DeleteBuffer(_eboHandle);
            GL.DeleteVertexArray(_vaoHandle);
        }
    }
}
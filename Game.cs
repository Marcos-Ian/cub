using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace FirstOpenTK
{
    public class Game : GameWindow
    {
        private int _vbo;           // vertex buffer
        private int _vao;           // vertex array
        private int _ebo;           // element/index buffer
        private int _shader;        // shader program
        private float _time = 0f;

        // Uniforms
        private int _uMvpLoc;

        // Camera / transforms
        private Matrix4 _projection;
        private Matrix4 _view;
        private Matrix4 _model = Matrix4.Identity;
        private float _autoRotationSpeed = 0.8f; // radians/sec
        private float _manualRotY = 0f;
        private float _scale = 1f;

        // Polygon mode toggle
        private bool _wireframe = false;

        public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.12f, 0.13f, 0.16f, 1f);
            GL.Enable(EnableCap.DepthTest);          // show correct faces/edges
            GL.DepthFunc(DepthFunction.Lequal);
            GL.Enable(EnableCap.CullFace);           // optional, improves perf
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);

            // --- Cube geometry (positions + colors) ---
            // Each vertex: vec3 position, vec3 color
            float[] vertices = {
                //  Position              Color
                -0.5f,-0.5f,-0.5f,       1f, 0f, 0f, // 0
                 0.5f,-0.5f,-0.5f,       0f, 1f, 0f, // 1
                 0.5f, 0.5f,-0.5f,       0f, 0f, 1f, // 2
                -0.5f, 0.5f,-0.5f,       1f, 1f, 0f, // 3
                -0.5f,-0.5f, 0.5f,       1f, 0f, 1f, // 4
                 0.5f,-0.5f, 0.5f,       0f, 1f, 1f, // 5
                 0.5f, 0.5f, 0.5f,       1f, 1f, 1f, // 6
                -0.5f, 0.5f, 0.5f,       0.2f,0.7f,0.3f // 7
            };

            // 12 triangles (36 indices)
            uint[] indices = {
    // Back  (-Z)
    0, 3, 2,  2, 1, 0,
    // Front (+Z)
    4, 5, 6,  6, 7, 4,
    // Left  (-X)
    0, 4, 7,  7, 3, 0,
    // Right (+X)
    1, 2, 6,  6, 5, 1,
    // Bottom (-Y)
    0, 1, 5,  5, 4, 0,
    // Top   (+Y)
    3, 7, 6,  6, 2, 3
};


            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // layout (location=0) vec3 aPos
            const int stride = 6 * sizeof(float);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            // layout (location=1) vec3 aColor
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            // --- Shaders (MVP) ---
            const string vert = @"#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
uniform mat4 uMVP;
out vec3 vColor;
void main() {
    vColor = aColor;
    gl_Position = uMVP * vec4(aPos, 1.0);
}";
            const string frag = @"#version 330 core
in vec3 vColor;
out vec4 FragColor;
void main() {
    FragColor = vec4(vColor, 1.0);
}";
            _shader = CreateProgram(vert, frag);
            _uMvpLoc = GL.GetUniformLocation(_shader, "uMVP");

            // --- Camera setup ---
            UpdateProjection();
            _view = Matrix4.LookAt(
                new Vector3(1.8f, 1.6f, 3.0f), // eye
                Vector3.Zero,                  // target
                Vector3.UnitY                  // up
            );

            // Unbind
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            var k = KeyboardState;
            if (k.IsKeyDown(Keys.Escape)) Close();

            // Optional interaction (bonus)
            if (k.IsKeyPressed(Keys.F1))
            {
                _wireframe = !_wireframe;
                GL.PolygonMode(MaterialFace.FrontAndBack, _wireframe ? PolygonMode.Line : PolygonMode.Fill);
            }
            if (k.IsKeyDown(Keys.Left)) _manualRotY -= 1.5f * (float)args.Time;
            if (k.IsKeyDown(Keys.Right)) _manualRotY += 1.5f * (float)args.Time;

            if (k.IsKeyDown(Keys.Up)) _scale = MathHelper.Clamp(_scale + 1.0f * (float)args.Time, 0.2f, 3f);
            if (k.IsKeyDown(Keys.Down)) _scale = MathHelper.Clamp(_scale - 1.0f * (float)args.Time, 0.2f, 3f);

            if (k.IsKeyPressed(Keys.R))  // reset
            {
                _manualRotY = 0f;
                _scale = 1f;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Accumulate elapsed time
            _time += (float)args.Time;

            // Model transform: continuous Y rotation + user adjustments + scale
            float autoAngle = _autoRotationSpeed * _time;
            _model = Matrix4.CreateScale(_scale) *
                     Matrix4.CreateRotationY(autoAngle + _manualRotY);

            // MVP
            Matrix4 mvp = _model * _view * _projection;
            GL.UseProgram(_shader);
            GL.UniformMatrix4(_uMvpLoc, false, ref mvp);

            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            UpdateProjection();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shader);
        }

        private void UpdateProjection()
        {
            float aspect = Size.X / (float)Size.Y;
            _projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f),
                MathHelper.Max(aspect, 0.0001f),
                0.1f,
                100f
            );
        }

        private static int CreateProgram(string vertexSrc, string fragmentSrc)
        {
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertexSrc);
            GL.CompileShader(vs);
            GL.GetShader(vs, ShaderParameter.CompileStatus, out int vOk);
            if (vOk == 0) throw new System.Exception("Vertex shader error:\n" + GL.GetShaderInfoLog(vs));

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragmentSrc);
            GL.CompileShader(fs);
            GL.GetShader(fs, ShaderParameter.CompileStatus, out int fOk);
            if (fOk == 0) throw new System.Exception("Fragment shader error:\n" + GL.GetShaderInfoLog(fs));

            int prog = GL.CreateProgram();
            GL.AttachShader(prog, vs);
            GL.AttachShader(prog, fs);
            GL.LinkProgram(prog);
            GL.GetProgram(prog, GetProgramParameterName.LinkStatus, out int pOk);
            if (pOk == 0) throw new System.Exception("Program link error:\n" + GL.GetProgramInfoLog(prog));

            GL.DetachShader(prog, vs);
            GL.DetachShader(prog, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            return prog;
        }
    }
}

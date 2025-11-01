using System;
using System.Collections.Generic;
using System.IO;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Assignment_4.Managers;
using Assignment_4.Entities;
using Assignment_4.Rendering;
using Assignment_4.Geometry;

namespace Assignment_4
{
    public class Game : GameWindow
    {
        // ──────────────── STATE ────────────────
        public bool HasKeycard { get; set; }
        public bool CardReaderUsed { get; set; }  // used to gate door once reader is used
        private string _hint = "Objective: Restore power.";

        // ──────────────── OBJECTS ────────────────
        private List<StaticInstance> _props = new();
        private StaticInstance[] _labWalls;
        private SecurityDoor _door;
        private ExitTrigger _exit;

        // ──────────────── MANAGERS ────────────────
        private readonly CollisionManager _collision = new(); // your existing collision helper
        private ModelManager _models;                         // NEW: centralized models owner

        // ──────────────── SCENE RESOURCES ────────────────
        private Shader _shader;
        private Texture _texFloor;
        private Texture _texWall, _texPanel, _texDoor, _texCard;
        private Mesh _meshFloor, _meshCube;
        private readonly Cube _cubeData = new Cube();
        private string _assetsPath;

        // ──────────────── CAMERA & LIGHTING ────────────────
        private Camera _camera = new Camera(new Vector3(0f, 1.2f, 4f));
        private bool _firstMove = true;
        private Vector2 _lastMouse;
        private Vector3 _lightPos = new(0f, 2.3f, 0f); // Centered overhead light
        private Vector3 _lightColor = new(1f, 1f, 1f);
        private bool _lightEnabled = true;
        private float _spin;

        // ──────────────── INPUT / TOGGLES ────────────────
        private bool _free3DMode = false;
        private bool _collisionEnabled = true;
        private bool _eWasDownLab = false;
        private bool _fWasDown = false;
        private bool _cWasDown = false;
        private bool _rWasDown = false;
        private bool _bWasDown = false;
        private bool _showCollisionBoxes = false; // use this everywhere (replaces missing _debugCollisionBoxesEnabled)

        // ──────────────── KEYCARD / DOOR ────────────────
        private bool _keycardAvailable = true;
        private readonly Vector3 _keycardSpawn = new(2.4f, 0.89f, -1.4f);
        private const float _keycardPickupRadius = 1f;

        private const float _readerUseRadius = 0.8f;

        private readonly Vector3 _doorClosedPos = new(5f, 0f, -1f);
        private readonly Vector3 _doorHingeOffset = new(0f, 0f, -0.5f);
        private float _doorAnim = 0f;
        private const float _doorAnimSpeed = 3.0f;

        // ───────────── POTION EFFECTS ─────────────
        private bool _flask1Available = true;
        private bool _flask2Available = true;
        private bool _flask3Available = true;

        private readonly Vector3 _flask1SpawnPos = new(-1.4f, 0.83f, -0.3f);
        private readonly Vector3 _flask2SpawnPos = new(0.0f, 0.87f, -1.5f);
        private readonly Vector3 _flask3SpawnPos = new(1.3f, 0.87f, -1.4f);

        private readonly Vector3 _flask1SpawnRot = new(0f, 60f, 0f);
        private readonly Vector3 _flask2SpawnRot = new(-90f, 150f, 0f);
        private readonly Vector3 _flask3SpawnRot = new(0f, -40f, 0f);

        private readonly Vector3 _flask1SpawnScale = new(0.10f, 0.10f, 0.10f);
        private readonly Vector3 _flask2SpawnScale = new(0.05f, 0.05f, 0.05f);
        private readonly Vector3 _flask3SpawnScale = new(0.03f, 0.03f, 0.03f);

        private float _invertColorsT = 0f;
        private float _invertControlsT = 0f;
        private float _cameraFlipT = 0f;

        private const float PotionDuration = 10f; // seconds

        private void ApplyInvertColors() { _invertColorsT = PotionDuration; SetHint("Potion! Colors inverted for 10s."); }
        private void ApplyInvertControls() { _invertControlsT = PotionDuration; SetHint("Potion! Controls inverted for 10s."); }
        private void ApplyCameraFlip() { _cameraFlipT = PotionDuration; SetHint("Potion! Camera flipped for 10s."); }

        // ──────────────── HELPERS ────────────────
        public void SetHint(string s) { _hint = s; Title = $"Mini 3D Explorer — {s}"; }
        public float PlayerDistance(Vector3 p) => (_camera.Position - p).Length;
        public void SetRoomLightsEnabled(bool enabled) { _lightEnabled = enabled; }

        public Game(GameWindowSettings gw, NativeWindowSettings nw) : base(gw, nw)
        {
            VSync = VSyncMode.On;
            CursorState = CursorState.Grabbed;
        }

        // ──────────────── ON LOAD ────────────────
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.08f, 0.09f, 0.10f, 1f);
            GL.Enable(EnableCap.DepthTest);

            // Shader setup
            string basePath = AppContext.BaseDirectory;
            _assetsPath = Path.Combine(basePath, "Assets");
            Directory.CreateDirectory(_assetsPath);
            _shader = new Shader(
                Path.Combine(basePath, "Shaders", "vertex.glsl"),
                Path.Combine(basePath, "Shaders", "fragment.glsl")
            );

            // Mesh setup
            _meshFloor = Mesh.CreatePlane(size: 18f, tiling: 9f);
            _meshCube = new Mesh(_cubeData.Vertices, _cubeData.Indices);

            _shader.Use();
            _shader.SetInt("diffuseTex", 0);
            _shader.SetInt("useTexture", 0);
            _shader.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
            _shader.SetFloat("shininess", 32f);

            // Default opacity (opaque pass)
            _shader.SetFloat("uOpacity", 1.0f);

            // Textures
            _texFloor = new Texture(Path.Combine(_assetsPath, "floor.jpg"));
            _texWall = new Texture(Path.Combine(_assetsPath, "wall.jpg"));
            _texPanel = new Texture(Path.Combine(_assetsPath, "panel.jpg"));

            // === Models via ModelManager ===
            _models = new ModelManager(_assetsPath);
            _models.LoadModels(_keycardSpawn, _doorClosedPos);
            Console.WriteLine("=== Model Loading Complete ===");
            Console.WriteLine($"\n=== Total models loaded: {_models.GetLoadedModelsCount()} ===");

            BuildLab();
            BuildScene();
            _flask1Available = _flask2Available = _flask3Available = true;

            // Set up multi-OBB collisions using manager's references
            _collision.SetupModelCollisions(
                officeDeskModel: _models.OfficeDeskModel,
                labBenchModel: _models.LabBenchModel,
                computerModel: _models.ComputerModel,
                fridgeModel: _models.FridgeModel,
                labChairModel: _models.LabChairModel,
                stoolModel: _models.StoolModel
            );

            SetHint("Objective: Find the keycard. (Press B to toggle collision debug)");
        }

        // ──────────────── UPDATE LOOP ────────────────
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            var kb = KeyboardState;
            var ms = MouseState;

            if (!IsFocused) return;

            // Tick down active potion timers
            if (_invertColorsT > 0f) _invertColorsT = MathF.Max(0f, _invertColorsT - (float)args.Time);
            if (_invertControlsT > 0f) _invertControlsT = MathF.Max(0f, _invertControlsT - (float)args.Time);
            if (_cameraFlipT > 0f) _cameraFlipT = MathF.Max(0f, _cameraFlipT - (float)args.Time);

            // ── Reset simulation (R)
            if (kb.IsKeyDown(Keys.R) && !_rWasDown)
                ResetGame();
            _rWasDown = kb.IsKeyDown(Keys.R);

            // ── Cursor toggle
            if (kb.IsKeyPressed(Keys.Escape))
                CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;

            // ── Collision toggle
            if (kb.IsKeyDown(Keys.C) && !_cWasDown)
            {
                _collisionEnabled = !_collisionEnabled;
                SetHint(_collisionEnabled ? "Collision Enabled" : "Collision Disabled");
            }
            _cWasDown = kb.IsKeyDown(Keys.C);

            // ── Collision debug visualization toggle (B)
            if (kb.IsKeyDown(Keys.B) && !_bWasDown)
            {
                _showCollisionBoxes = !_showCollisionBoxes;
                SetHint(_showCollisionBoxes ? "Collision Debug: ON" : "Collision Debug: OFF");
            }
            _bWasDown = kb.IsKeyDown(Keys.B);

            // ── Movement mode toggle (2D/3D)
            if (kb.IsKeyDown(Keys.F) && !_fWasDown)
            {
                _free3DMode = !_free3DMode;
                SetHint(_free3DMode ? "3D Mode Enabled" : "2D Ground Mode Enabled");
            }
            _fWasDown = kb.IsKeyDown(Keys.F);

            // ── Movement
            float baseSpeed = 5f * (float)args.Time;
            float sign = (_invertControlsT > 0f) ? -1f : 1f;
            float speed = baseSpeed * sign;

            Vector3 front = _camera.GetFront();
            Vector3 right = _camera.Right;

            Vector3 previousPosition = _camera.Position;

            if (_free3DMode)
            {
                if (kb.IsKeyDown(Keys.W)) _camera.Position += front * speed;
                if (kb.IsKeyDown(Keys.S)) _camera.Position -= front * speed;
                if (kb.IsKeyDown(Keys.A)) _camera.Position -= right * speed;
                if (kb.IsKeyDown(Keys.D)) _camera.Position += right * speed;
            }
            else
            {
                Vector3 flatFront = front; flatFront.Y = 0; flatFront.Normalize();
                Vector3 flatRight = right; flatRight.Y = 0; flatRight.Normalize();

                if (kb.IsKeyDown(Keys.W)) _camera.Position += flatFront * speed;
                if (kb.IsKeyDown(Keys.S)) _camera.Position -= flatFront * speed;
                if (kb.IsKeyDown(Keys.A)) _camera.Position -= flatRight * speed;
                if (kb.IsKeyDown(Keys.D)) _camera.Position += flatRight * speed;

                _camera.Position = new Vector3(_camera.Position.X, 1.2f, _camera.Position.Z);
            }

            // Mouse look
            if (CursorState == CursorState.Grabbed)
            {
                if (_firstMove)
                {
                    _lastMouse = new Vector2(ms.X, ms.Y);
                    _firstMove = false;
                }
                else
                {
                    var delta = new Vector2(ms.X, ms.Y) - _lastMouse;
                    _lastMouse = new Vector2(ms.X, ms.Y);

                    float sens = 0.1f;
                    _camera.Yaw += delta.X * sens;
                    _camera.Pitch -= delta.Y * sens;
                    _camera.Pitch = Math.Clamp(_camera.Pitch, -89f, 89f);
                }
            }

            // Zoom
            if (ms.ScrollDelta.Y != 0)
            {
                _camera.Fov -= ms.ScrollDelta.Y * 2f;
                _camera.Fov = Math.Clamp(_camera.Fov, 30f, 90f);
            }

            // Light control
            if (kb.IsKeyDown(Keys.L))
            {
                float lightSpeed = 3f * (float)args.Time;
                if (kb.IsKeyDown(Keys.Up)) _lightPos += new Vector3(0f, 0f, -lightSpeed);
                if (kb.IsKeyDown(Keys.Down)) _lightPos += new Vector3(0f, 0f, lightSpeed);
                if (kb.IsKeyDown(Keys.Left)) _lightPos += new Vector3(-lightSpeed, 0f, 0f);
                if (kb.IsKeyDown(Keys.Right)) _lightPos += new Vector3(lightSpeed, 0f, 0f);
                if (kb.IsKeyDown(Keys.PageUp)) _lightPos += new Vector3(0f, lightSpeed, 0f);
                if (kb.IsKeyDown(Keys.PageDown)) _lightPos += new Vector3(0f, -lightSpeed, 0f);
                SetHint($"Light Position: ({_lightPos.X:F1}, {_lightPos.Y:F1}, {_lightPos.Z:F1})");
            }

            // ── Collision bounds
            if (_collisionEnabled)
            {
                // Room bounds
                _camera.Position = new Vector3(
                    Math.Clamp(_camera.Position.X, -4.8f, 4.8f),
                    _free3DMode ? _camera.Position.Y : 1.2f,
                    Math.Clamp(_camera.Position.Z, -4.8f, 4.8f)
                );

                // Door collision (kept simple)
                if (!_door._open)
                {
                    var p = _camera.Position;
                    var a = _door.WorldAabb;
                    if (a.Contains(p))
                    {
                        _camera.Position = previousPosition;
                    }
                }

                // OBB COLLISION: resolve against each model OBB using circle-vs-OBB
                float playerRadius = 0.35f; // keep your current value
                Vector2 camXZ = new Vector2(_camera.Position.X, _camera.Position.Z);
                camXZ = _collision.ResolveCircleVsScene(camXZ, playerRadius);
                _camera.Position = new Vector3(camXZ.X, _camera.Position.Y, camXZ.Y);
            }

            // ── Interactions (E)
            if (kb.IsKeyPressed(Keys.E))
            {
                // Keycard pickup
                if (_keycardAvailable && _models.KeycardModel != null &&
                    PlayerDistance(_models.KeycardModel.Position) <= _keycardPickupRadius)
                {
                    HasKeycard = true;
                    _keycardAvailable = false;
                    SetHint("Keycard acquired — find the card reader.");
                }

                // Potion interactions (use each flask as a different effect)
                const float pickupRadius = 1f;

                if (_models.FlaskModel1 != null && _flask1Available &&
                    PlayerDistance(_models.FlaskModel1.Position) <= pickupRadius)
                {
                    ApplyInvertColors();
                    _flask1Available = false;
                }

                if (_models.FlaskModel2 != null && _flask2Available &&
                    PlayerDistance(_models.FlaskModel2.Position) <= pickupRadius)
                {
                    ApplyInvertControls();
                    _flask2Available = false;
                }

                if (_models.FlaskModel3 != null && _flask3Available &&
                    PlayerDistance(_models.FlaskModel3.Position) <= pickupRadius)
                {
                    ApplyCameraFlip();
                    _flask3Available = false;
                }

                // Card reader interaction
                if (_models.CardReaderModel != null &&
                    PlayerDistance(_models.CardReaderModel.Position) <= _readerUseRadius)
                {
                    if (HasKeycard)
                    {
                        CardReaderUsed = true;
                        _door.Interact(this);
                        SetHint("Access granted — door opened.");
                    }
                    else
                    {
                        SetHint("Access denied — keycard required.");
                    }
                }

                // Door interaction
                if (_door.CanInteract(this))
                {
                    if (CardReaderUsed)
                    {
                        _door.Interact(this);
                        SetHint("Door opened.");
                    }
                    else
                    {
                        SetHint("Door is locked — use the card reader first.");
                    }
                }
            }

            // Animate the visual door model using SecurityDoor state
            float target = _door._open ? 1f : 0f;
            _doorAnim = MathHelper.Lerp(_doorAnim, target, (float)args.Time * _doorAnimSpeed);

            if (_models.DoorModel != null)
            {
                float rotationAngle = _doorAnim * 90f;
                _models.DoorModel.Position = _doorClosedPos;
                _models.DoorModel.Rotation = new Vector3(-90f, -90f + rotationAngle, 0f);
            }

            _exit.Update(this);
        }

        // ──────────────── RENDER LOOP ────────────────
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            // Build view with optional upside-down roll
            Matrix4 view = _camera.GetViewMatrix();
            if (_cameraFlipT > 0f)
            {
                view = view * Matrix4.CreateRotationX(MathHelper.Pi);
            }

            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", _camera.GetProjection(Size.X / (float)Size.Y));
            _shader.SetVector3("lightPos", _lightPos);
            _shader.SetVector3("viewPos", _camera.Position);
            _shader.SetVector3("lightColor", _lightColor);
            _shader.SetVector3("objectColor", Vector3.One);
            _shader.SetFloat("lightEnabled", _lightEnabled ? 1f : 0f);
            _shader.SetFloat("shininess", 32f);

            // make sure depth test is on each frame
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            // ===== OPAQUE PASS =====
            GL.Disable(EnableCap.Blend);
            GL.DepthMask(true);

            // FLOOR (force texture ON just for the floor)
            _shader.SetFloat("uOpacity", 1.0f);
            _shader.SetInt("useTexture", 1);
            _texFloor.Use();
            _shader.SetMatrix4("model", Matrix4.Identity);
            _meshFloor.Draw();
            // reset so untextured colored things aren’t accidentally textured
            _shader.SetInt("useTexture", 0);
            _shader.SetInt("uInvertColors", (_invertColorsT > 0f) ? 1 : 0);

            // Static props & walls
            foreach (var p in _props) p.Draw(_shader);
            foreach (var w in _labWalls) w.Draw(_shader);

            // Opaque models via manager
            _models.DrawOpaqueModels(_shader, _keycardAvailable);

            // ===== TRANSPARENT PASS =====
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.DepthMask(false);

            _models.DrawTransparentModels(_shader, _camera, _flask1Available, _flask2Available, _flask3Available);

            // restore defaults
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);

            // Draw collision debug boxes (use the correct flag)
            if (_showCollisionBoxes)
            {
                _collision.DrawCollisionDebug(_shader, _meshCube);
            }

            // Exit marker (if any)
            var exitModel = Matrix4.CreateScale(0.7f) * Matrix4.CreateTranslation(_exit.Position + new Vector3(0f, 0.35f, 0f));
            _shader.SetMatrix4("model", exitModel);

            SwapBuffers();
        }

        // ──────────────── RESET / BUILDERS ────────────────
        private void ResetGame()
        {
            _camera = new Camera(new Vector3(0f, 1.2f, 4f));
            _lightPos = new Vector3(0f, 2.3f, 0f);
            _lightEnabled = true;
            _spin = 0;
            HasKeycard = false;
            CardReaderUsed = false;
            _eWasDownLab = _fWasDown = _cWasDown = _rWasDown = _bWasDown = false;
            _keycardAvailable = true;
            _showCollisionBoxes = false;

            if (_models.KeycardModel != null)
            {
                _models.KeycardModel.Position = _keycardSpawn;
                _models.KeycardModel.Rotation = new Vector3(0f, 280f, 0f);
            }

            // Potions become available again
            _flask1Available = _flask2Available = _flask3Available = true;

            // Put them back at their spawn transforms
            if (_models.FlaskModel1 != null)
            {
                _models.FlaskModel1.Position = _flask1SpawnPos;
                _models.FlaskModel1.Rotation = _flask1SpawnRot;
                _models.FlaskModel1.Scale = _flask1SpawnScale;
            }
            if (_models.FlaskModel2 != null)
            {
                _models.FlaskModel2.Position = _flask2SpawnPos;
                _models.FlaskModel2.Rotation = _flask2SpawnRot;
                _models.FlaskModel2.Scale = _flask2SpawnScale;
            }
            if (_models.FlaskModel3 != null)
            {
                _models.FlaskModel3.Position = _flask3SpawnPos;
                _models.FlaskModel3.Rotation = _flask3SpawnRot;
                _models.FlaskModel3.Scale = _flask3SpawnScale;
            }

            // Clear effects (so reset starts clean)
            _invertColorsT = _invertControlsT = _cameraFlipT = 0f;

            BuildLab();
            BuildScene();
            _collision.SetupModelCollisions(
                officeDeskModel: _models.OfficeDeskModel,
                labBenchModel: _models.LabBenchModel,
                computerModel: _models.ComputerModel,
                fridgeModel: _models.FridgeModel,
                labChairModel: _models.LabChairModel,
                stoolModel: _models.StoolModel
            );

            SetHint("Objective: Find the keycard. (Press B to toggle collision debug)");
            CursorState = CursorState.Grabbed;
        }

        private void BuildLab()
        {
            var wallBox = new Aabb(new Vector3(-0.5f), new Vector3(0.5f));
            _labWalls = new[]
            {
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(10f,2.5f,0.2f)*Matrix4.CreateTranslation(0,1.25f,-5f), wallBox),
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(10f,2.5f,0.2f)*Matrix4.CreateTranslation(0,1.25f, 5f), wallBox),
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(0.2f,2.5f,10f)*Matrix4.CreateTranslation(-5f,1.25f,0), wallBox),
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(0.2f,2.5f,4f)*Matrix4.CreateTranslation(5f,1.25f,-3f), wallBox),
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(0.2f,2.5f,4.8f)*Matrix4.CreateTranslation(5f,1.25f, 2.7f), wallBox),
                new StaticInstance(_meshCube, _texWall, Matrix4.CreateScale(0.2f,0.5f,1.3f)*Matrix4.CreateTranslation(5f,2.25f,-0.35f), wallBox)
            };

            _door = new SecurityDoor(_meshCube, _texDoor, new Vector3(5f, 1.1f, 0f));
            _exit = new ExitTrigger(new Vector3(0f, 0f, 4.5f));
        }

        private void BuildScene()
        {
            _props.Clear();
            var mesh = _meshCube;

            var texMetal = _texPanel;

            float ceilingHeight = 2.5f;

            _props.Add(PropFactory.Create(mesh, texMetal, new Vector3(0f, ceilingHeight, 0f), new Vector3(10f, 0.1f, 10f)));

            float lightY = ceilingHeight - 0.12f;
            _ = lightY; // currently unused, keep for future ceiling lights
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _meshFloor?.Dispose();
            _meshCube?.Dispose();

            _models?.Dispose(); // dispose all models centrally

            GL.UseProgram(0);
        }

        // ─────────────────────────────────────────────────────────
        // OBB helper type for XZ-plane collisions (kept for future use)
        private readonly struct Obb2D
        {
            public readonly Vector2 Center;       // XZ center
            public readonly Vector2 HalfExtents;  // half width (X) and half depth (Z)
            public readonly float RotationRad;    // yaw
            public readonly float MinY, MaxY;     // vertical span

            public Obb2D(Vector2 center, Vector2 halfExtents, float rotationRad, float minY, float maxY)
            {
                Center = center;
                HalfExtents = halfExtents;
                RotationRad = rotationRad;
                MinY = minY;
                MaxY = maxY;
            }
        }
    }
}

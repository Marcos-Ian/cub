// ModelManager.cs
using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;




using Assignment_4.Rendering;


namespace Assignment_4.Managers
{
    /// <summary>
    /// Centralizes model ownership, loading, and draw passes.
    /// Keeps original logic/paths/parameters intact.
    /// </summary>
    public sealed class ModelManager : IDisposable
    {
        private readonly string _assetsPath;

        // ──────────────── IMPORTED 3D MODELS ────────────────
        private Model _officeDeskModel;
        private Model _phMeterModel;
        private Model _microscopeModel;
        private Model _flaskModel;
        private Model _labChairModel;
        private Model _spcetroModel;
        private Model _testTubeRackModel;
        private Model _computerModel;
        private Model _stoolModel;
        private Model _labBenchModel;
        private Model _fireExtinguisherModel;
        private Model _safetyGogglesModel;
        private Model _fridgeModel;
        private Model _pcrMachineModel;
        private Model _ceilingLightModel;
        private Model _keycardModel;
        private Model _flaskModel1;
        private Model _flaskModel2;
        private Model _flaskModel3;
        private Model _cardReaderModel;
        private Model _doorModel;

        // ──────────────── PUBLIC ACCESSORS (needed by Game) ────────────────
        public Model OfficeDeskModel => _officeDeskModel;
        public Model LabBenchModel => _labBenchModel;
        public Model ComputerModel => _computerModel;
        public Model FridgeModel => _fridgeModel;
        public Model LabChairModel => _labChairModel;
        public Model StoolModel => _stoolModel;

        public Model KeycardModel => _keycardModel;
        public Model CardReaderModel => _cardReaderModel;
        public Model DoorModel => _doorModel;

        public Model FlaskModel1 => _flaskModel1;
        public Model FlaskModel2 => _flaskModel2;
        public Model FlaskModel3 => _flaskModel3;

        public ModelManager(string assetsPath)
        {
            _assetsPath = assetsPath ?? throw new ArgumentNullException(nameof(assetsPath));
        }

        // ──────────────── LOAD MODELS (original logic preserved) ────────────────
        public void LoadModels(Vector3 keycardSpawn, Vector3 doorClosedPos)
        {
            Console.WriteLine("=== Loading 3D Models ===");

            TryLoadModel(ref _stoolModel, "bar-stool/bar_stool_01.fbx",
                position: new Vector3(0f, 0f, 1f),
                scale: new Vector3(0.017f, 0.017f, 0.017f),
                rotation: new Vector3(0f, -90f, 0f),
                textureFolder: "bar-stool/textures");

            TryLoadModel(ref _labChairModel, "lab-chair/chair_low_triangulated.fbx",
                position: new Vector3(-3.5f, 0.1f, 3.7f),
                scale: new Vector3(0.025f, 0.025f, 0.025f),
                rotation: new Vector3(0f, -90f, 0f),
                textureFolder: "lab-chair/textures");

            TryLoadModel(ref _computerModel, "desk-low-poly/Desk.fbx",
                position: new Vector3(-3.5f, 0f, 4.3f),
                scale: new Vector3(0.4f, 0.4f, 0.4f),
                rotation: new Vector3(270f, 90f, 0f));

            TryLoadModel(ref _testTubeRackModel, "test-tube-mutations/Phials_Collection.fbx",
                position: new Vector3(-1.2f, 1f, 1.2f),
                scale: new Vector3(0.2f, 0.2f, 0.2f),
                rotation: new Vector3(270f, 90f, 0f));

            TryLoadModel(ref _spcetroModel, "water-bath/Water_Bath.fbx",
                position: new Vector3(0.7f, 1f, 0f),
                scale: new Vector3(0.06f, 0.06f, 0.06f),
                rotation: new Vector3(0f, 0f, 0f));

            TryLoadModel(ref _labBenchModel, "chemistry-lab-table/Table_02.fbx",
                position: new Vector3(0f, 0f, 0f),
                scale: new Vector3(0.8f, 0.8f, 0.8f),
                rotation: new Vector3(270f, 0f, 0f));

            TryLoadModel(ref _flaskModel, "free-conical-flask-laboratory-low-poly/SM_Conical_flask_NakedSingularity.fbx",
                position: new Vector3(-1f, 0.87f, 0f),
                scale: new Vector3(0.1f, 0.1f, 0.1f),
                rotation: new Vector3(-90f, 90f, 0f),
                textureFolder: "free-conical-flask-laboratory-low-poly/textures");

            TryLoadModel(ref _phMeterModel, "digital-timer-programmer/Temporizador.fbx",
                position: new Vector3(-1.4f, 0.9f, -1.4f),
                scale: new Vector3(0.05f, 0.05f, 0.05f),
                rotation: new Vector3(-90f, -40f, 0f),
                textureFolder: "digital-timer-programmer/textures");

            TryLoadModel(ref _officeDeskModel, "office_desk/office_desk.fbx",
                position: new Vector3(4.2f, 0f, 3.7f),
                scale: new Vector3(0.8f, 0.8f, 0.8f),
                rotation: new Vector3(270f, 0f, 0f),
                textureFolder: "office_desk/textures");

            TryLoadModel(ref _microscopeModel, "microscope/microscope.fbx",
                position: new Vector3(-1.2f, 0.87f, 2f),
                scale: new Vector3(0.013f, 0.013f, 0.013f),
                rotation: new Vector3(-90f, -90f, 0f),
                textureFolder: "microscope/textures");

            TryLoadModel(ref _fireExtinguisherModel, "fire-extinguisher/Fire Extinguisher Textured.fbx",
                position: new Vector3(3.8f, 1f, -4.8f),
                scale: new Vector3(0.0004f, 0.0004f, 0.0004f),
                rotation: new Vector3(0f, 180f, 0f),
                textureFolder: "fire-extinguisher/textures");

            TryLoadModel(ref _safetyGogglesModel, "safety-goggles/safety_goggles.fbx",
                position: new Vector3(1f, 0.92f, -1f),
                scale: new Vector3(0.01f, 0.01f, 0.01f),
                rotation: new Vector3(0f, 67f, 0f));

            TryLoadModel(ref _fridgeModel, "not-too-modern-fridge/temp_export.fbx",
                position: new Vector3(-3.8f, 0f, -4.1f),
                scale: new Vector3(0.009f, 0.009f, 0.009f),
                rotation: new Vector3(0f, -180f, 0f),
                textureFolder: "not-too-modern-fridge/textures");

            TryLoadModel(ref _pcrMachineModel, "pcr-machine/PCR012.fbx",
                position: new Vector3(0.4f, 0.9f, -1.4f),
                scale: new Vector3(0.02f, 0.02f, 0.02f),
                rotation: new Vector3(-90f, -100f, 0f),
                textureFolder: "pcr-machine/textures");

            TryLoadModel(ref _ceilingLightModel, "light-fixture-ceiling-recessed/LightFixtureRecessed.fbx",
                position: new Vector3(0f, 2.45f, 0f),
                scale: new Vector3(1f, 1f, 1f),
                rotation: new Vector3(-90f, 0f, 0f),
                textureFolder: "light-fixture-ceiling-recessed/textures");

            TryLoadModel(ref _keycardModel, "keycard-model/Keycard_Model.fbx",
                position: keycardSpawn,
                scale: new Vector3(0.02f, 0.02f, 0.02f),
                rotation: new Vector3(0f, 280f, 0f),
                textureFolder: "keycard-model/textures");

            TryLoadModel(ref _cardReaderModel, "card-security-reader/LeitorFASE1.fbx",
                position: new Vector3(4.9f, 1.2f, -1.5f),
                scale: new Vector3(0.2f, 0.2f, 0.2f),
                rotation: new Vector3(0f, 90f, 0f),
                textureFolder: "card-security-reader/textures");

            TryLoadModel(ref _doorModel, "prison-hallway-door-2048px2/Jail_Door.fbx",
                position: doorClosedPos,
                scale: new Vector3(1.3f, 1.5f, 1f),
                rotation: new Vector3(90f, -90f, 0f),
                textureFolder: "prison-hallway-door-2048px2/textures");

            // --- Extra flasks on the lab bench ---
            TryLoadModel(ref _flaskModel1, "magic-flask/Kolba.fbx",
                position: new Vector3(-1.4f, 0.83f, -0.3f),
                scale: new Vector3(0.1f, 0.1f, 0.1f),
                rotation: new Vector3(0f, 60f, 0f));

            TryLoadModel(ref _flaskModel2, "magic-flask/flask.fbx",
                position: new Vector3(0.0f, 0.87f, -1.5f),
                scale: new Vector3(0.05f, 0.05f, 0.05f),
                rotation: new Vector3(-90f, 150f, 0f));

            TryLoadModel(ref _flaskModel3, "magic-flask/potion.fbx",
                position: new Vector3(1.3f, 0.87f, -1.4f),
                scale: new Vector3(0.03f, 0.03f, 0.03f),
                rotation: new Vector3(0f, -40f, 0f));

            Console.WriteLine($"\n=== Total models loaded: {GetLoadedModelsCount()} ===");
        }

        // ──────────────── TRY-LOAD (original logic preserved) ────────────────
        private void TryLoadModel(ref Model model, string relativePath, Vector3 position, Vector3 scale, Vector3 rotation, string textureFolder = null)
        {
            try
            {
                string fullPath = Path.Combine(_assetsPath, relativePath);
                if (File.Exists(fullPath))
                {
                    model = new Model(fullPath);
                    model.Position = position;
                    model.Scale = scale;
                    model.Rotation = rotation;

                    if (!string.IsNullOrEmpty(textureFolder))
                    {
                        string texFolderPath = Path.Combine(_assetsPath, textureFolder);
                        model.LoadTexturesFromFolder(texFolderPath);
                    }

                    Console.WriteLine($"✓ Loaded: {relativePath}");
                }
                else
                {
                    Console.WriteLine($"✗ Not found: {relativePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error loading {relativePath}: {ex.Message}");
            }
        }

        // ──────────────── DRAW PASSES (original logic preserved) ────────────────
        public void DrawOpaqueModels(Shader shader, bool keycardAvailable)
        {
            shader.SetFloat("uOpacity", 1.0f);

            if (_phMeterModel != null) _phMeterModel.Draw(shader);
            if (_officeDeskModel != null) _officeDeskModel.Draw(shader);
            if (_ceilingLightModel != null) _ceilingLightModel.Draw(shader);
            if (_microscopeModel != null) _microscopeModel.Draw(shader);
            if (_labChairModel != null) _labChairModel.Draw(shader);
            if (_spcetroModel != null) _spcetroModel.Draw(shader);
            if (_testTubeRackModel != null) _testTubeRackModel.Draw(shader);
            if (_computerModel != null) _computerModel.Draw(shader);
            if (_stoolModel != null) _stoolModel.Draw(shader);
            if (_labBenchModel != null) _labBenchModel.Draw(shader);
            if (_fireExtinguisherModel != null) _fireExtinguisherModel.Draw(shader);
            if (_safetyGogglesModel != null) _safetyGogglesModel.Draw(shader);
            if (_fridgeModel != null) _fridgeModel.Draw(shader);
            if (_pcrMachineModel != null) _pcrMachineModel.Draw(shader);
            if (_keycardModel != null && keycardAvailable) _keycardModel.Draw(shader);
            if (_cardReaderModel != null) _cardReaderModel.Draw(shader);
            if (_doorModel != null) _doorModel.Draw(shader);
        }

        public void DrawTransparentModels(Shader shader, Camera camera,
                                          bool flask1Available, bool flask2Available, bool flask3Available)
        {
            var glass = new List<Model>();
            if (_flaskModel != null) glass.Add(_flaskModel);
            if (_flaskModel1 != null && flask1Available) glass.Add(_flaskModel1);
            if (_flaskModel2 != null && flask2Available) glass.Add(_flaskModel2);
            if (_flaskModel3 != null && flask3Available) glass.Add(_flaskModel3);

            // Sort back-to-front using camera distance (Length)
            glass.Sort((a, b) =>
            {
                float da = (camera.Position - a.Position).Length;
                float db = (camera.Position - b.Position).Length;
                return db.CompareTo(da);
            });

            shader.SetFloat("uOpacity", 0.35f);

            foreach (var m in glass)
            {
                // First pass: back faces only
                GL.CullFace(CullFaceMode.Front);
                GL.Enable(EnableCap.CullFace);
                m.Draw(shader);

                // Second pass: front faces only
                GL.CullFace(CullFaceMode.Back);
                m.Draw(shader);
                GL.Disable(EnableCap.CullFace);
            }
        }

        // ──────────────── COUNT (original selection preserved) ────────────────
        public int GetLoadedModelsCount()
        {
            int count = 0;
            if (_phMeterModel != null) count++;
            if (_officeDeskModel != null) count++;
            if (_ceilingLightModel != null) count++;
            if (_microscopeModel != null) count++;
            if (_flaskModel != null) count++;
            if (_labChairModel != null) count++;
            if (_spcetroModel != null) count++;
            if (_testTubeRackModel != null) count++;
            if (_computerModel != null) count++;
            if (_stoolModel != null) count++;
            if (_labBenchModel != null) count++;
            if (_keycardModel != null) count++;
            return count;
        }

        // ──────────────── DISPOSE ────────────────
        public void Dispose()
        {
            _phMeterModel?.Dispose();
            _officeDeskModel?.Dispose();
            _microscopeModel?.Dispose();
            _flaskModel?.Dispose();
            _labChairModel?.Dispose();
            _spcetroModel?.Dispose();
            _testTubeRackModel?.Dispose();
            _computerModel?.Dispose();
            _stoolModel?.Dispose();
            _labBenchModel?.Dispose();
            _fireExtinguisherModel?.Dispose();
            _safetyGogglesModel?.Dispose();
            _fridgeModel?.Dispose();
            _pcrMachineModel?.Dispose();
            _ceilingLightModel?.Dispose();
            _keycardModel?.Dispose();
            _cardReaderModel?.Dispose();
            _doorModel?.Dispose();
            _flaskModel1?.Dispose();
            _flaskModel2?.Dispose();
            _flaskModel3?.Dispose();
        }
    }
}

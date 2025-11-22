// ModelManager.cs 
using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Assignment_4.Rendering;

namespace Assignment_4.Managers
{
    /// <summary>
    /// Centralizes model ownership, loading, and draw passes.
    /// Version with ONLY card reader and door models.
    /// </summary>
    public sealed class ModelManager : IDisposable
    {
        private readonly string _assetsPath;

        // ──────────────── IMPORTED 3D MODELS ────────────────
        private Model _deskModel;

        private Model _ceilingLightModel;
        private Model _bedModel;
        private Model _doorModel;
        private Model _wardrobeModel;
        private Model _sidetableModel;
        // ──────────────── PUBLIC ACCESSORS (needed by Game) ────────────────

        public Model DeskModel => _deskModel;

        public Model SideTableModel => _sidetableModel;
        public Model BedModel => _bedModel;
        public Model DoorModel => _doorModel;
        public Model WardrobeModel => _wardrobeModel;
        public ModelManager(string assetsPath)
        {
            _assetsPath = assetsPath ?? throw new ArgumentNullException(nameof(assetsPath));
        }

        // ──────────────── LOAD MODELS ────────────────
        // NOTE: keycardSpawn is unused now but kept to avoid changing Game.cs
        public void LoadModels(Vector3 keycardSpawn, Vector3 doorClosedPos)
        {
            Console.WriteLine("=== Loading 3D Models ===");

            TryLoadModel(ref _doorModel, "door/DOOR.fbx",
                position: doorClosedPos,
                scale: new Vector3(0.037f, 0.03f, 0.03f),
                rotation: new Vector3(-90f, 0f, 90f));

            TryLoadModel(ref _bedModel, "bed-simple/Bed_Set1.fbx",
               position: new Vector3(0f, 0f, -3f),
               scale: new Vector3(0.0015f, 0.0015f, 0.0015f),
               rotation: new Vector3(-90f, 0f, 0f),
               textureFolder: "bed-simple/textures");

            TryLoadModel(ref _deskModel, "low-poly-computer-desk/Mini_CompDesk_01.fbx",
               position: new Vector3(-3.5f, 0f, 3.8f),
               scale: new Vector3(21f, 21f, 21f),
               rotation: new Vector3(-90f, 90f, 0f),
               textureFolder: "low-poly-computer-desk/textures");

            TryLoadModel(ref _wardrobeModel, "wardrobe/Wardrobe.fbx",
            position: new Vector3(-4f, 2.15f, -4.2f),
            scale: new Vector3(0.009f, 0.009f, 0.009f),
            rotation: new Vector3(0f, 0f, 0f),
            textureFolder: "wardrobe/textures");


            TryLoadModel(ref _sidetableModel, "bedside-table/Bedside_Table_LP.fbx",
            position: new Vector3(2f, 0.01f, -4f),
            scale: new Vector3(0.4f, 0.4f, 0.4f),
            rotation: new Vector3(-90f, 0f, 0f),
            textureFolder: "bedside-table/textures");
            Console.WriteLine($"\n=== Total models loaded: {GetLoadedModelsCount()} ===");

            TryLoadModel(ref _ceilingLightModel, "light-fixture-ceiling-recessed/LightFixtureRecessed.fbx",
           position: new Vector3(0f, 2.45f, 0f),
           scale: new Vector3(1f, 1f, 1f),
           rotation: new Vector3(-90f, 0f, 0f),
           textureFolder: "light-fixture-ceiling-recessed/textures");
        }

        // ──────────────── TRY-LOAD ────────────────
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

        // ──────────────── DRAW PASSES ────────────────
        // keycardAvailable kept for signature compatibility, but unused now
        public void DrawOpaqueModels(Shader shader, bool keycardAvailable)
        {
            shader.SetFloat("uOpacity", 1.0f);
            if (_ceilingLightModel != null) _ceilingLightModel.Draw(shader);
            if (_bedModel != null) _bedModel.Draw(shader);

            if (_doorModel != null) _doorModel.Draw(shader);
            if (_deskModel != null) _deskModel.Draw(shader);
            if (_wardrobeModel != null) _wardrobeModel.Draw(shader);
            if (_sidetableModel != null) _sidetableModel.Draw(shader);
        }

        // No transparent models anymore – method kept so Game.cs doesn’t break
        public void DrawTransparentModels(Shader shader, Camera camera,
                                          bool flask1Available, bool flask2Available, bool flask3Available)
        {
            // Intentionally empty: no transparent models (flasks removed)
        }

        // ──────────────── COUNT ────────────────
        public int GetLoadedModelsCount()
        {
            int count = 0;
            if (_ceilingLightModel != null) count++;
            if (_bedModel != null) count++;

            if (_doorModel != null) count++;
            if (_deskModel != null) count++;
            if (_wardrobeModel != null) count++;
            if (_sidetableModel != null) count++;
            return count;
        }

        // ──────────────── DISPOSE ────────────────
        public void Dispose()
        {
            _sidetableModel?.Dispose();
            _wardrobeModel?.Dispose();
            _deskModel?.Dispose();
            _bedModel?.Dispose();
            _doorModel?.Dispose();
            _ceilingLightModel?.Dispose();
        }
    }
}

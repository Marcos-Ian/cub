using Assimp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using GLPrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using AiTextureType = Assimp.TextureType;


using Assignment_4.Managers;
using Assignment_4.Entities;
using Assignment_4.Geometry;

using Assignment_4.Rendering;

namespace Assignment_4.Managers
{
    public class Model : IDisposable
    {
        private readonly List<ModelMesh> _meshes = new List<ModelMesh>();
        private string _sourceDir = "";

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; } = Vector3.One;

        // NEW: Mesh index-based texture assignment
        public void SetTextureForMeshIndex(int index, Texture texture)
        {
            if (index >= 0 && index < _meshes.Count)
            {
                _meshes[index].SetTexture(texture);
                Console.WriteLine($"  ✓ Applied texture to mesh index {index}");
            }
            else
            {
                Console.WriteLine($"  ✗ Invalid mesh index {index} (model has {_meshes.Count} meshes)");
            }
        }

        // NEW: Get mesh count helper
        public int GetMeshCount() => _meshes.Count;

        // Try to retrieve any "color" texture, preferring modern PBR names if present.
        private bool TryGetAnyColorTexture(Material mat, out TextureSlot texSlot)
        {
            var order = new List<AiTextureType>();

            if (Enum.TryParse<AiTextureType>("BaseColor", out var baseColor)) order.Add(baseColor);
            if (Enum.TryParse<AiTextureType>("Albedo", out var albedo)) order.Add(albedo);

            order.AddRange(new[]
            {
                AiTextureType.Diffuse,
                AiTextureType.Unknown,
                AiTextureType.Emissive,
                AiTextureType.Ambient,
                AiTextureType.Specular
            });

            foreach (var t in order)
            {
                if (mat.GetMaterialTextureCount(t) > 0 &&
                    mat.GetMaterialTexture(t, 0, out texSlot))
                    return true;
            }

            texSlot = default;
            return false;
        }

        public Model(string path)
        {
            _sourceDir = Path.GetDirectoryName(Path.GetFullPath(path)) ?? "";
            Console.WriteLine($"\n=== Loading Model: {Path.GetFileName(path)} ===");
            Console.WriteLine($"Model source directory: {_sourceDir}");
            LoadModel(path);
        }

        private void LoadModel(string path)
        {
            var importer = new AssimpContext();

            var scene = importer.ImportFile(path,
                PostProcessSteps.Triangulate |
                PostProcessSteps.GenerateNormals |
                PostProcessSteps.CalculateTangentSpace
            );

            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
                throw new Exception($"Error loading model: {path}");

            Console.WriteLine($"✓ Model loaded: {scene.MeshCount} meshes, {scene.MaterialCount} materials, {scene.TextureCount} embedded textures");

            if (scene.TextureCount > 0)
            {
                Console.WriteLine("\n--- Embedded Textures ---");
                for (int i = 0; i < scene.TextureCount; i++)
                {
                    var tex = scene.Textures[i];
                    Console.WriteLine($"  [{i}] Format: {tex.CompressedFormatHint}, Compressed: {tex.HasCompressedData}, " +
                                    $"Size: {tex.Width}x{tex.Height}, Data: {tex.CompressedData?.Length ?? 0} bytes");
                }
            }

            Console.WriteLine("\n--- Materials ---");
            for (int i = 0; i < scene.MaterialCount; i++)
            {
                var mat = scene.Materials[i];
                Console.WriteLine($"  [{i}] {mat.Name}");
                Console.WriteLine($"      HasTextureDiffuse: {mat.HasTextureDiffuse}");
                if (mat.HasTextureDiffuse)
                {
                    Console.WriteLine($"      Diffuse path: '{mat.TextureDiffuse.FilePath}'");
                }
                Console.WriteLine($"      HasColorDiffuse: {mat.HasColorDiffuse}");
                if (mat.HasColorDiffuse)
                {
                    var c = mat.ColorDiffuse;
                    Console.WriteLine($"      Diffuse color: ({c.R:F2}, {c.G:F2}, {c.B:F2})");
                }
            }

            Console.WriteLine("\n--- Processing Meshes ---");
            ProcessNode(scene.RootNode, scene);
            Console.WriteLine($"=== Model Loading Complete: {_meshes.Count} meshes processed ===\n");
        }

        public void LoadTexturesFromFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"  Texture folder not found: {folderPath}");
                return;
            }

            var imageFiles = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                           f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            Console.WriteLine($"  Found {imageFiles.Length} texture files in folder");

            foreach (var mesh in _meshes)
            {
                if (mesh.HasTexture()) continue;

                string bestMatch = FindBestTextureMatch(imageFiles);
                if (!string.IsNullOrEmpty(bestMatch))
                {
                    mesh.SetTexture(new Texture(bestMatch));
                    Console.WriteLine($"    Applied: {Path.GetFileName(bestMatch)}");
                }
            }
        }

        private string FindBestTextureMatch(string[] textureFiles)
        {
            string[] priorities = {
                "BaseColor", "Albedo", "Diffuse", "Color",
                "base_color", "albedo", "diffuse", "color"
            };

            foreach (var priority in priorities)
            {
                var match = textureFiles.FirstOrDefault(f =>
                    Path.GetFileName(f).Contains(priority, StringComparison.OrdinalIgnoreCase));
                if (match != null) return match;
            }

            return textureFiles.Length > 0 ? textureFiles[0] : null;
        }

        private void ProcessNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                var mesh = scene.Meshes[node.MeshIndices[i]];
                _meshes.Add(ProcessMesh(mesh, scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
                ProcessNode(node.Children[i], scene);
        }

        private ModelMesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            Console.WriteLine($"\n  Mesh: {mesh.Name ?? "unnamed"} (MaterialIndex: {mesh.MaterialIndex})");

            var vertices = new List<float>();
            var indices = new List<uint>();

            bool hasUVs = mesh.HasTextureCoords(0);
            Console.WriteLine($"    HasTextureCoords: {hasUVs}, VertexCount: {mesh.VertexCount}");

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                vertices.Add(mesh.Vertices[i].X);
                vertices.Add(mesh.Vertices[i].Y);
                vertices.Add(mesh.Vertices[i].Z);

                if (mesh.HasNormals)
                {
                    vertices.Add(mesh.Normals[i].X);
                    vertices.Add(mesh.Normals[i].Y);
                    vertices.Add(mesh.Normals[i].Z);
                }
                else
                {
                    vertices.Add(0f); vertices.Add(1f); vertices.Add(0f);
                }

                if (hasUVs)
                {
                    var uv = mesh.TextureCoordinateChannels[0][i];
                    vertices.Add(uv.X);
                    vertices.Add(uv.Y);
                }
                else
                {
                    vertices.Add(0f); vertices.Add(0f);
                }
            }

            for (int f = 0; f < mesh.FaceCount; f++)
            {
                var face = mesh.Faces[f];
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add((uint)face.Indices[j]);
            }

            var meshObj = new ModelMesh(vertices.ToArray(), indices.ToArray());

            bool textureLoaded = false;

            try
            {
                if (mesh.MaterialIndex >= 0 && mesh.MaterialIndex < scene.MaterialCount)
                {
                    var mat = scene.Materials[mesh.MaterialIndex];
                    Console.WriteLine($"    Processing material: {mat.Name}");

                    TextureSlot slot;
                    if (TryGetAnyColorTexture(mat, out slot))
                    {
                        string texPath = slot.FilePath?.Trim() ?? "";

                        if (string.IsNullOrEmpty(texPath) || texPath.StartsWith("*"))
                        {
                            Console.WriteLine($"    Attempting embedded texture load (path was '{texPath}')...");
                            for (int i = 0; i < scene.TextureCount && !textureLoaded; i++)
                                textureLoaded = TryLoadEmbeddedTexture(meshObj, scene.Textures[i]);

                            if (!textureLoaded)
                            {
                                var guessed = TryGuessTextureByMaterialName(mat.Name);
                                if (!string.IsNullOrEmpty(guessed) && File.Exists(guessed))
                                {
                                    meshObj.SetTexture(new Texture(guessed));
                                    textureLoaded = true;
                                    Console.WriteLine($"    ✓ Guessed texture: {guessed}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine($"    Searching for external texture: {texPath}");
                            string found = FindTexturePath(texPath);
                            if (!string.IsNullOrEmpty(found) && File.Exists(found))
                            {
                                meshObj.SetTexture(new Texture(found));
                                textureLoaded = true;
                                Console.WriteLine("    ✓ External texture loaded");
                            }
                            else
                            {
                                Console.WriteLine("    ✗ Texture file not found (will try material-name guess)");
                                var guessed = TryGuessTextureByMaterialName(mat.Name);
                                if (!string.IsNullOrEmpty(guessed) && File.Exists(guessed))
                                {
                                    meshObj.SetTexture(new Texture(guessed));
                                    textureLoaded = true;
                                    Console.WriteLine($"    ✓ Guessed texture: {guessed}");
                                }
                            }
                        }
                    }

                    if (!textureLoaded)
                    {
                        Console.WriteLine("    Using fallback color");
                        SetFallbackColor(meshObj, mat);
                    }
                    else
                    {
                        Console.WriteLine("    ✓✓✓ TEXTURE SUCCESSFULLY LOADED ✓✓✓");
                    }

                    if (mat.HasOpacity && mat.Opacity < 1.0f)
                        meshObj.SetTransparent(true);
                }
                else
                {
                    Console.WriteLine("    No material - using neutral gray");
                    meshObj.SetFlatColor(new Vector3(0.8f, 0.8f, 0.8f));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ⚠ Material error: {ex.Message}");
                meshObj.SetFlatColor(new Vector3(0.8f, 0.8f, 0.8f));
            }

            return meshObj;
        }

        private string TryGuessTextureByMaterialName(string materialName)
        {
            if (string.IsNullOrWhiteSpace(materialName)) return null;

            string[] suffixes = {
                "_BaseColor", "_baseColor", "_Albedo", "_albedo",
                "_Diffuse", "_diffuse", "_Color", "_color"
            };
            string[] exts = { ".png", ".jpg", ".jpeg", ".tga", ".bmp" };

            string[] roots = { Path.Combine(_sourceDir, "textures"), _sourceDir };
            foreach (var root in roots)
            {
                if (!Directory.Exists(root)) continue;
                foreach (var s in suffixes)
                    foreach (var e in exts)
                    {
                        string p = Path.Combine(root, materialName + s + e);
                        if (File.Exists(p)) return p;
                    }
            }
            return null;
        }

        private string FindTexturePath(string originalPath)
        {
            originalPath = originalPath.Trim();
            string filename = Path.GetFileName(originalPath);

            string texturesFolder = Path.Combine(_sourceDir, "textures");
            if (Directory.Exists(texturesFolder))
            {
                string texPath = Path.Combine(texturesFolder, filename);
                if (File.Exists(texPath))
                {
                    return texPath;
                }

                foreach (var file in Directory.GetFiles(texturesFolder))
                {
                    if (Path.GetFileName(file).Equals(filename, StringComparison.OrdinalIgnoreCase))
                    {
                        return file;
                    }
                }

                string baseNameWithoutExt = Path.GetFileNameWithoutExtension(filename);
                string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tga" };
                foreach (var ext in extensions)
                {
                    texPath = Path.Combine(texturesFolder, baseNameWithoutExt + ext);
                    if (File.Exists(texPath))
                    {
                        return texPath;
                    }
                }
            }

            string directPath = Path.Combine(_sourceDir, filename);
            if (File.Exists(directPath))
            {
                return directPath;
            }

            if (Directory.Exists(_sourceDir))
            {
                var foundFiles = Directory.GetFiles(_sourceDir, "*.*", SearchOption.AllDirectories);
                foreach (var file in foundFiles)
                {
                    if (Path.GetFileName(file).Equals(filename, StringComparison.OrdinalIgnoreCase))
                    {
                        return file;
                    }
                }
            }

            return null;
        }

        private bool TryLoadEmbeddedTexture(ModelMesh meshObj, EmbeddedTexture embeddedTex)
        {
            try
            {
                byte[] data = null;

                if (embeddedTex.HasCompressedData && embeddedTex.CompressedData != null && embeddedTex.CompressedData.Length > 0)
                {
                    data = embeddedTex.CompressedData;
                    Console.WriteLine($"      Compressed data found: {data.Length} bytes, format: {embeddedTex.CompressedFormatHint}");
                }
                else if (embeddedTex.HasNonCompressedData && embeddedTex.Width > 0 && embeddedTex.Height > 0)
                {
                    Console.WriteLine($"      Loading uncompressed embedded texture: {embeddedTex.Width}x{embeddedTex.Height}");

                    int texHandle = GL.GenTexture();
                    GL.BindTexture(TextureTarget.Texture2D, texHandle);

                    GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                        embeddedTex.Width, embeddedTex.Height, 0,
                        PixelFormat.Rgba, PixelType.UnsignedByte, embeddedTex.NonCompressedData);

                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)OpenTK.Graphics.OpenGL4.TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

                    meshObj.SetTexture(new Texture(texHandle));
                    Console.WriteLine("      ✓✓✓ Loaded uncompressed RGBA texture successfully!");
                    return true;
                }

                if (data == null || data.Length == 0)
                {
                    Console.WriteLine($"      ✗ No valid texture data");
                    return false;
                }

                string ext = ".png";
                if (!string.IsNullOrEmpty(embeddedTex.CompressedFormatHint))
                {
                    string hint = embeddedTex.CompressedFormatHint.ToLower();
                    if (hint == "jpg" || hint == "jpeg") ext = ".jpg";
                    else if (hint == "png") ext = ".png";
                    else ext = "." + hint;
                }

                string tempPath = Path.Combine(Path.GetTempPath(), $"embedded_tex_{Guid.NewGuid()}{ext}");
                Console.WriteLine($"      Writing temp file: {tempPath}");
                File.WriteAllBytes(tempPath, data);

                var texture = new Texture(tempPath);
                meshObj.SetTexture(texture);

                try { File.Delete(tempPath); } catch { }

                Console.WriteLine($"      ✓✓✓ Embedded texture loaded successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"      ✗ Exception loading embedded texture: {ex.Message}");
                return false;
            }
        }

        private void SetFallbackColor(ModelMesh meshObj, Material mat)
        {
            Vector3 color = new Vector3(0.7f, 0.7f, 0.7f);

            string matName = mat.Name?.ToLower() ?? "";

            if (matName.Contains("black") || (matName.Contains("plastic") && matName.Contains("black")))
            {
                color = new Vector3(0.15f, 0.15f, 0.15f);
            }
            else if (matName.Contains("metal"))
            {
                color = new Vector3(0.7f, 0.7f, 0.75f);
            }
            else if (matName.Contains("glass"))
            {
                color = new Vector3(0.85f, 0.9f, 0.95f);
                meshObj.SetTransparent(true);
            }
            else if (mat.HasColorDiffuse)
            {
                var c = mat.ColorDiffuse;
                float minBrightness = 0.15f;
                color = new Vector3(
                    Math.Max(c.R, minBrightness),
                    Math.Max(c.G, minBrightness),
                    Math.Max(c.B, minBrightness)
                );
                Console.WriteLine($"      Using material diffuse color: ({color.X:F2}, {color.Y:F2}, {color.Z:F2})");
            }

            meshObj.SetFlatColor(color);
        }

        public void SetTextureOverride(Texture texture)
        {
            foreach (var mesh in _meshes)
            {
                mesh.SetTexture(texture);
            }
            Console.WriteLine($"  Applied texture override to {_meshes.Count} meshes");
        }

        public void Draw(Shader shader)
        {
            Matrix4 model =
                Matrix4.CreateScale(Scale) *
                Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z)) *
                Matrix4.CreateTranslation(Position);

            shader.SetMatrix4("model", model);

            foreach (var mesh in _meshes)
                mesh.Draw(shader);
        }

        public void Dispose()
        {
            foreach (var mesh in _meshes)
                mesh.Dispose();
        }
    }

    public class ModelMesh : IDisposable
    {
        private int _vao, _vbo, _ebo;
        private int _indexCount;

        private Texture _texture;
        private Vector3? _flatColor;
        private bool _transparent = false;

        public ModelMesh(float[] vertices, uint[] indices)
        {
            _indexCount = indices.Length;

            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            _ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = 8 * sizeof(float);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void SetTexture(Texture texture)
        {
            _texture = texture;
            Console.WriteLine($"        Texture set on mesh (ID: {texture?.GetTextureId() ?? 0})");
        }

        public void SetFlatColor(Vector3 color) => _flatColor = color;
        public void SetTransparent(bool enable) => _transparent = enable;
        public bool HasTexture() => _texture != null;

        public void Draw(Shader shader)
        {
            if (_transparent)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }

            if (_texture != null)
            {
                _texture.Use(TextureUnit.Texture0);
                shader.SetInt("useTexture", 1);
                shader.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
            }
            else if (_flatColor.HasValue)
            {
                shader.SetInt("useTexture", 0);
                shader.SetVector3("objectColor", _flatColor.Value);
            }
            else
            {
                shader.SetInt("useTexture", 0);
                shader.SetVector3("objectColor", new Vector3(0.8f, 0.8f, 0.8f));
            }

            GL.BindVertexArray(_vao);
            GL.DrawElements(GLPrimitiveType.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            if (_transparent)
                GL.Disable(EnableCap.Blend);
        }

        public void Dispose()
        {
            _texture?.Dispose();
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
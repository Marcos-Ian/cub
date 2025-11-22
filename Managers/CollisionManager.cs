// collisionManager.cs
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

using Assignment_4.Rendering;
namespace Assignment_4.Managers
{
    /// <summary>
    /// Manages model OBBs, circle-vs-OBB collision resolution, and optional debug drawing.
    /// Logic is lifted verbatim from Game.cs collision helpers to avoid behavior changes.
    /// </summary>
    public class CollisionManager
    {
        // Public read-only access to generated OBBs
        private readonly List<Obb2D> _modelObbColliders = new();
        public IReadOnlyList<Obb2D> ModelObbColliders => _modelObbColliders;

        /// <summary>
        /// Clears all current OBB colliders.
        /// </summary>
        public void Clear() => _modelObbColliders.Clear();

        /// <summary>
        /// Rebuilds OBBs for the scene models. Matches the original SetupModelCollisions logic.
        /// </summary>
        public void SetupModelCollisions(
            Model bedModel,
            Model deskModel,
            Model wardrobeModel,
            Model sidetableModel
           )
        {
            _modelObbColliders.Clear();
            Console.WriteLine("=== Setting up collisions (OBB) ===");

            // BED - simple box in the center of the bed
            if (bedModel != null)
            {
                Console.WriteLine("Bed (1 box):");
                AddCompoundModelCollider(bedModel, new List<(Vector3, Vector3)>
                {
                    // Bed is scaled 0.0015, so visual sizes need to be large
                    (new Vector3(0f, 0f, 0f), new Vector3(1400f, 600f, 2000f)),
                });
            }

            // DESK - computer desk (scale 21)
            if (deskModel != null)
            {
                Console.WriteLine("Computer Desk (1 box):");
                AddCompoundModelCollider(deskModel, new List<(Vector3, Vector3)>
                {
                    (new Vector3(0f, 0f, 0f), new Vector3(0.06f, 0.02f, 0.08f)),
                });
            }

            // WARDROBE - tall cabinet with depth
            if (wardrobeModel != null)
            {
                Console.WriteLine("Wardrobe (1 box):");
                AddCompoundModelCollider(wardrobeModel, new List<(Vector3, Vector3)>
                {
                    // Wardrobe is scaled 0.009, so collision needs to scale accordingly
                    (new Vector3(60f, 1f, 9f), new Vector3(170f, 220f, 70f)),
                });
            }

            // SIDETABLE - small bedside table
            if (sidetableModel != null)
            {
                Console.WriteLine("Sidetable (1 box):");
                AddCompoundModelCollider(sidetableModel, new List<(Vector3, Vector3)>
                {
                    // Sidetable is scaled 0.4
                    (new Vector3(0f, 0f, 0f), new Vector3(2f, 1.5f, 2f)),
                });
            }

            Console.WriteLine($"=== Total OBB colliders: {_modelObbColliders.Count} ===");
        }

        /// <summary>
        /// Adds multiple OBB parts for a single model. Logic unchanged.
        /// </summary>
        public void AddCompoundModelCollider(Model model, List<(Vector3 localOffset, Vector3 size)> colliderParts)
        {
            if (model == null) return;

            Console.WriteLine($"  Adding collider to model at {model.Position} with scale {model.Scale}");

            float yawRad = MathHelper.DegreesToRadians(model.Rotation.Y);
            float cos = MathF.Cos(yawRad);
            float sin = MathF.Sin(yawRad);

            foreach (var (localOffset, size) in colliderParts)
            {
                // Scale the offset and size by model scale
                Vector3 scaledOffset = new Vector3(
                    localOffset.X * model.Scale.X,
                    localOffset.Y * model.Scale.Y,
                    localOffset.Z * model.Scale.Z
                );

                // Rotate offset around model origin to match model rotation
                Vector2 rotatedOffset = new Vector2(
                    scaledOffset.X * cos - scaledOffset.Z * sin,
                    scaledOffset.X * sin + scaledOffset.Z * cos
                );

                // Final world position = model position + rotated offset
                Vector2 centerXZ = new Vector2(model.Position.X, model.Position.Z) + rotatedOffset;

                // Scale the collision box size
                Vector3 worldSize = new Vector3(
                    size.X * MathF.Abs(model.Scale.X),
                    size.Y * MathF.Abs(model.Scale.Y),
                    size.Z * MathF.Abs(model.Scale.Z)
                );

                var halfExtentsXZ = new Vector2(worldSize.X * 0.5f, worldSize.Z * 0.5f);
                float minY = 0f;
                float maxY = MathF.Max(0.01f, worldSize.Y); // ensure positive height

                _modelObbColliders.Add(new Obb2D(centerXZ, halfExtentsXZ, yawRad, minY, maxY));
                Console.WriteLine($"  ✓ OBB at XZ({centerXZ.X:F2}, {centerXZ.Y:F2}) size({worldSize.X:F2}×{worldSize.Z:F2}) Y({minY:F2}-{maxY:F2})");
            }
        }

        /// <summary>
        /// Adds a single OBB for a model given a visual footprint size. Logic unchanged.
        /// </summary>
        public void AddModelObbCollider(Model model, Vector3 visualSize)
        {
            if (model == null) return;

            // Scale visual size by model.Scale to get world-space footprint
            Vector3 worldSize = new Vector3(
                visualSize.X * MathF.Abs(model.Scale.X),
                visualSize.Y * MathF.Abs(model.Scale.Y),
                visualSize.Z * MathF.Abs(model.Scale.Z)
            );

            // We treat collision as 2D in XZ plane
            var centerXZ = new Vector2(model.Position.X, model.Position.Z);
            var halfExtentsXZ = new Vector2(worldSize.X * 0.5f, worldSize.Z * 0.5f);

            // Use model.Rotation.Y (degrees) as the yaw in world-space
            float yawRad = MathHelper.DegreesToRadians(model.Rotation.Y);

            // Clamp Y for collision if needed
            float minY = 0f;
            float maxY = MathF.Max(0.01f, worldSize.Y);

            _modelObbColliders.Add(new Obb2D(centerXZ, halfExtentsXZ, yawRad, minY, maxY));
        }

        /// <summary>
        /// Performs circle-vs-OBB test with push-out vector (world space). Logic unchanged.
        /// </summary>
        public static bool CircleOverlapsObb(Vector2 circleCenterWorld, float radius, Obb2D obb, out Vector2 pushOutWorld)
        {
            // Transform circle center into the OBB's local space (rotate by -θ)
            float cos = MathF.Cos(-obb.RotationRad);
            float sin = MathF.Sin(-obb.RotationRad);
            Vector2 rel = circleCenterWorld - obb.Center;
            Vector2 local = new Vector2(
                rel.X * cos - rel.Y * sin,
                rel.X * sin + rel.Y * cos
            );

            // Find the closest point on the box to the circle center in local space
            Vector2 closest = new Vector2(
                Math.Clamp(local.X, -obb.HalfExtents.X, obb.HalfExtents.X),
                Math.Clamp(local.Y, -obb.HalfExtents.Y, obb.HalfExtents.Y)
            );

            Vector2 diff = local - closest;
            float distSq = diff.LengthSquared;

            if (distSq >= radius * radius)
            {
                pushOutWorld = Vector2.Zero;
                return false;
            }

            // Compute local push-out
            float dist = MathF.Sqrt(MathF.Max(distSq, 1e-8f));
            Vector2 nLocal;
            float penetration;

            if (dist > 1e-5f)
            {
                nLocal = diff / dist;
                penetration = radius - dist;
            }
            else
            {
                // Center is on/in box; push along least-penetration axis
                float dx = (obb.HalfExtents.X - MathF.Abs(local.X));
                float dz = (obb.HalfExtents.Y - MathF.Abs(local.Y));
                if (dx < dz)
                    nLocal = new Vector2(MathF.Sign(local.X), 0f);
                else
                    nLocal = new Vector2(0f, MathF.Sign(local.Y));
                penetration = radius;
            }

            Vector2 pushLocal = nLocal * penetration;

            // Rotate push back to world space (+θ)
            float cosW = MathF.Cos(obb.RotationRad);
            float sinW = MathF.Sin(obb.RotationRad);
            pushOutWorld = new Vector2(
                pushLocal.X * cosW - pushLocal.Y * sinW,
                pushLocal.X * sinW + pushLocal.Y * cosW
            );

            return true;
        }

        /// <summary>
        /// Applies minimal push-outs against every OBB (same loop used in Game.OnUpdateFrame).
        /// Returns the corrected cam XZ position after resolving all overlaps.
        /// </summary>
        public Vector2 ResolveCircleVsScene(Vector2 camXZ, float playerRadius)
        {
            foreach (var obb in _modelObbColliders)
            {
                if (CircleOverlapsObb(camXZ, playerRadius, obb, out Vector2 pushOutWorld))
                {
                    camXZ += pushOutWorld; // minimal push-out (slides along edges)
                }
            }
            return camXZ;
        }

        /// <summary>
        /// Draws wireframe boxes for OBBs. Caller should set view/projection and any other uniforms.
        /// </summary>
        public void DrawCollisionDebug(Shader shader, Mesh meshCube)
        {
            if (shader == null || meshCube == null) return;

            // Switch to wireframe mode
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Disable(EnableCap.DepthTest); // Draw on top

            shader.SetVector3("objectColor", new Vector3(0f, 1f, 0f)); // Green
            shader.SetInt("useTexture", 0);

            foreach (var obb in _modelObbColliders)
            {
                // Create a box at the OBB position with correct rotation
                // Height is based on Y span from the OBB (minY to maxY)
                float height = obb.MaxY - obb.MinY;
                float centerY = (obb.MinY + obb.MaxY) * 0.5f;

                Matrix4 model =
                    Matrix4.CreateScale(obb.HalfExtents.X * 2f, height, obb.HalfExtents.Y * 2f) *
                    Matrix4.CreateRotationY(obb.RotationRad) *
                    Matrix4.CreateTranslation(obb.Center.X, centerY, obb.Center.Y);

                shader.SetMatrix4("model", model);
                meshCube.Draw();

                Console.WriteLine($"Debug OBB: center=({obb.Center.X:F2},{centerY:F2},{obb.Center.Y:F2}) size=({obb.HalfExtents.X * 2:F2},{height:F2},{obb.HalfExtents.Y * 2:F2})");
            }

            // Restore normal rendering
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Enable(EnableCap.DepthTest);
            shader.SetVector3("objectColor", Vector3.One);
        }

        // ─────────────────────────────────────────────────────────
        // OBB helper type for XZ-plane collisions (public so Game can use it)
        public readonly struct Obb2D
        {
            public readonly Vector2 Center;       // XZ center
            public readonly Vector2 HalfExtents;  // half width (X) and half depth (Z)
            public readonly float RotationRad;    // yaw
            public readonly float MinY, MaxY;     // vertical span (reserved for future use)

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
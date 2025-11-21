using OpenTK.Mathematics;

using Assignment_4.Managers;
using Assignment_4.Entities;
using Assignment_4.Rendering;
using Assignment_4.Geometry;

namespace Assignment_4.Entities
{
    public static class PropFactory
    {
        // Base builder: you can pass size, rotation (in degrees), and position
        public static StaticInstance Create(Mesh mesh, Texture tex, Vector3 pos, Vector3 size, float rotationY = 0f)
        {
            var rotation = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotationY));
            var model = Matrix4.CreateScale(size) * rotation * Matrix4.CreateTranslation(pos);
            var local = new Aabb(new Vector3(-0.5f), new Vector3(0.5f));
            return new StaticInstance(mesh, tex, model, local);
        }

        // Simple helpers (optional shortcuts)
        public static StaticInstance Box(Mesh mesh, Texture tex, Vector3 pos, Vector3 size) =>
            Create(mesh, tex, pos, size);

        public static StaticInstance Panel(Mesh mesh, Texture tex, Vector3 pos, Vector2 sizeXY, float depth = 0.1f, float rotationY = 0f) =>
            Create(mesh, tex, pos, new Vector3(sizeXY.X, sizeXY.Y, depth), rotationY);

        public static StaticInstance Crate(Mesh mesh, Texture tex, Vector3 pos, float size = 0.6f) =>
            Create(mesh, tex, pos, new Vector3(size));

        public static StaticInstance TableTop(Mesh mesh, Texture tex, Vector3 pos, Vector2 sizeXZ, float thickness = 0.08f) =>
            Create(mesh, tex, pos, new Vector3(sizeXZ.X, thickness, sizeXZ.Y));

        public static StaticInstance TableLeg(Mesh mesh, Texture tex, Vector3 pos, float height = 0.7f, float leg = 0.06f) =>
            Create(mesh, tex, new Vector3(pos.X, pos.Y + height * 0.5f, pos.Z), new Vector3(leg, height, leg));
    }
}

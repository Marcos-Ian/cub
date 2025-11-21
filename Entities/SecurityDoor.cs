using OpenTK.Mathematics;


using Assignment_4.Rendering;
using Assignment_4.Geometry;
namespace Assignment_4.Entities
{
    public class SecurityDoor
    {
        public Vector3 Position;
        public bool _open = false;

        public Mesh Mesh;
        public Texture Texture;

        // Rotate door 90 degrees around Y so it faces the gap correctly
        private static readonly Matrix4 DoorRot = Matrix4.CreateRotationY(MathF.PI / 2f);

        // Closed and open positions (slides along Z after rotation)
        public Matrix4 ModelClosed =>
            Matrix4.CreateScale(1.2f, 2.2f, 0.2f) *
            DoorRot *
            Matrix4.CreateTranslation(Position);

        public Matrix4 ModelOpen =>
            Matrix4.CreateScale(1.2f, 2.2f, 0.2f) *
            DoorRot *
            Matrix4.CreateTranslation(Position + new Vector3(0f, 0f, 1.6f));

        public Aabb ClosedAabbLocal = new Aabb(new Vector3(-0.6f, -1.1f, -0.1f), new Vector3(0.6f, 1.1f, 0.1f));
        public Aabb WorldAabb;

        public SecurityDoor(Mesh mesh, Texture tex, Vector3 pos)
        {
            Mesh = mesh;
            Texture = tex;
            Position = pos;
            WorldAabb = ClosedAabbLocal.Transform(ModelClosed);
        }

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", _open ? ModelOpen : ModelClosed);
            Texture.Use();
            Mesh.Draw();
        }
    }
}

using OpenTK.Mathematics;


using Assignment_4.Rendering;
using Assignment_4.Geometry;
namespace Assignment_4.Entities
{
    public class SecurityDoor : IInteractable
    {
        public string Prompt => _open ? "Door OPEN" :
                                _locked ? "Requires KEYCARD" : "Press E to OPEN DOOR";
        public Vector3 Position;
        public bool _locked = true;
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

        public bool CanInteract(Game game)
        {
            return (game.PlayerDistance(Position) < 1.6f) && (!_locked || game.HasKeycard);
        }

        public bool Interact(Game game)
        {
            if (!CanInteract(game)) return false;

            if (_locked && game.HasKeycard)
                _locked = false;

            if (!_open)
            {
                _open = true;
                game.SetHint("Door opened.");
            }
            else
            {
                _open = false;
                game.SetHint("Door closed.");
            }

            // Update blocking AABB
            WorldAabb = _open
                ? new Aabb(new Vector3(0), new Vector3(0))
                : ClosedAabbLocal.Transform(ModelClosed);

            return true;
        }

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", _open ? ModelOpen : ModelClosed);
            Texture.Use();
            Mesh.Draw();
        }
    }
}

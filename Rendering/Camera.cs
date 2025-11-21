using OpenTK.Mathematics;
using System;

namespace Assignment_4.Rendering
{
    public class Camera
    {
        public Vector3 Position;
        public float Yaw = -90f;
        public float Pitch = 0f;

        public float Fov = 60f;
        public float Near = 0.1f;
        public float Far = 100f;

        public Camera(Vector3 startPos)
        {
            Position = startPos;
        }

        public Matrix4 GetViewMatrix()
        {
            var front = GetFront();
            return Matrix4.LookAt(Position, Position + front, Vector3.UnitY);
        }

        public Matrix4 GetProjection(float aspect)
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), aspect, Near, Far);
        }

        public Vector3 GetFront()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            return Vector3.Normalize(front);
        }

        public Vector3 Right => Vector3.Normalize(Vector3.Cross(GetFront(), Vector3.UnitY));
        public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, GetFront()));
    }
}

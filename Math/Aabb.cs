using OpenTK.Mathematics;

namespace Assignment_4.Geometry
{
    public struct Aabb
    {
        public Vector3 Min, Max;
        public Aabb(Vector3 min, Vector3 max) { Min = min; Max = max; }

        public bool Contains(Vector3 p) =>
            p.X >= Min.X && p.X <= Max.X &&
            p.Y >= Min.Y && p.Y <= Max.Y &&
            p.Z >= Min.Z && p.Z <= Max.Z;

        public Aabb Transform(Matrix4 m)
        {
            // Expand by transforming the 8 corners (safe for scaled boxes)
            var corners = new Vector3[]
            {
                new(Min.X,Min.Y,Min.Z), new(Max.X,Min.Y,Min.Z),
                new(Min.X,Max.Y,Min.Z), new(Max.X,Max.Y,Min.Z),
                new(Min.X,Min.Y,Max.Z), new(Max.X,Min.Y,Max.Z),
                new(Min.X,Max.Y,Max.Z), new(Max.X,Max.Y,Max.Z),
            };
            Vector3 newMin = new(float.PositiveInfinity), newMax = new(float.NegativeInfinity);
            foreach (var c in corners)
            {
                var v = Vector3.TransformPosition(c, m);
                newMin = Vector3.ComponentMin(newMin, v);
                newMax = Vector3.ComponentMax(newMax, v);
            }
            return new Aabb(newMin, newMax);
        }
    }
}

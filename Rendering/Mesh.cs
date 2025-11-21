using OpenTK.Graphics.OpenGL4;
using System;

namespace Assignment_4.Rendering
{
    public class Mesh : IDisposable
    {
        public int VAO { get; private set; }
        public int VBO { get; private set; }
        public int EBO { get; private set; }
        public int IndexCount { get; private set; }

        public Mesh(float[] interleaved, uint[] indices, int strideFloats = 8)
        {
            IndexCount = indices.Length;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, interleaved.Length * sizeof(float), interleaved, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            int stride = strideFloats * sizeof(float);
            // layout(location=0) position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride, 0);
            GL.EnableVertexAttribArray(0);
            // layout(location=1) normal
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride, 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            // layout(location=2) texcoord
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride, 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(VAO);
            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        public static Mesh CreatePlane(float size = 10f, float tiling = 10f)
        {
            float s = size * 0.5f;
            // y=0 plane, up normal (0,1,0)
            float[] v =
            {
                -s,0,-s,  0,1,0,   0,0,
                 s,0,-s,  0,1,0,   tiling,0,
                 s,0, s,  0,1,0,   tiling,tiling,
                -s,0, s,  0,1,0,   0,tiling
            };
            uint[] i = { 0, 1, 2, 2, 3, 0 };
            return new Mesh(v, i);
        }

        public static Mesh CreatePyramid()
        {
            // Simple 4-side pyramid centered at origin, base on y= -0.5, tip at y=0.5
            float h = 0.5f;
            float b = 0.5f;
            float[] v = {
                // Base quad (two tris) normal down
                -b,-h,-b, 0,-1,0, 0,0,
                 b,-h,-b, 0,-1,0, 1,0,
                 b,-h, b, 0,-1,0, 1,1,
                -b,-h, b, 0,-1,0, 0,1,

                // Tip vertex duplicated per face to assign proper normals/uvs
                // Front face
                -b,-h, b,  0,0,1,  0,0,
                 b,-h, b,  0,0,1,  1,0,
                 0, h, 0,  0,0,1,  0.5f,1,

                // Right
                 b,-h, b,  1,0,0,  0,0,
                 b,-h,-b,  1,0,0,  1,0,
                 0, h, 0,  1,0,0,  0.5f,1,

                // Back
                 b,-h,-b, 0,0,-1,  0,0,
                -b,-h,-b, 0,0,-1,  1,0,
                 0, h, 0, 0,0,-1,  0.5f,1,

                // Left
                -b,-h,-b, -1,0,0,  0,0,
                -b,-h, b, -1,0,0,  1,0,
                 0, h, 0, -1,0,0,  0.5f,1,
            };

            uint[] i = {
                // base
                0,1,2, 2,3,0,
                // front
                4,5,6,
                // right
                7,8,9,
                // back
                10,11,12,
                // left
                13,14,15
            };
            return new Mesh(v, i);
        }

        public void Dispose()
        {
            if (EBO != 0) GL.DeleteBuffer(EBO);
            if (VBO != 0) GL.DeleteBuffer(VBO);
            if (VAO != 0) GL.DeleteVertexArray(VAO);
            EBO = VBO = VAO = 0;
        }
    }
}

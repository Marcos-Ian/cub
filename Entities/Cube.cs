namespace Assignment_4.Entities
{
    public class Cube
    {
        // Interleaved: position (3) | normal (3) | texcoord (2)
        public float[] Vertices =
        {
            // Front (0,0,1)
            -0.5f,-0.5f, 0.5f,   0f,0f,1f,   0f,0f,
             0.5f,-0.5f, 0.5f,   0f,0f,1f,   1f,0f,
             0.5f, 0.5f, 0.5f,   0f,0f,1f,   1f,1f,
            -0.5f, 0.5f, 0.5f,   0f,0f,1f,   0f,1f,

            // Back (0,0,-1)
             0.5f,-0.5f,-0.5f,   0f,0f,-1f,  0f,0f,
            -0.5f,-0.5f,-0.5f,   0f,0f,-1f,  1f,0f,
            -0.5f, 0.5f,-0.5f,   0f,0f,-1f,  1f,1f,
             0.5f, 0.5f,-0.5f,   0f,0f,-1f,  0f,1f,

            // Left (-1,0,0)
            -0.5f,-0.5f,-0.5f,  -1f,0f,0f,   0f,0f,
            -0.5f,-0.5f, 0.5f,  -1f,0f,0f,   1f,0f,
            -0.5f, 0.5f, 0.5f,  -1f,0f,0f,   1f,1f,
            -0.5f, 0.5f,-0.5f,  -1f,0f,0f,   0f,1f,

            // Right (1,0,0)
             0.5f,-0.5f, 0.5f,   1f,0f,0f,   0f,0f,
             0.5f,-0.5f,-0.5f,   1f,0f,0f,   1f,0f,
             0.5f, 0.5f,-0.5f,   1f,0f,0f,   1f,1f,
             0.5f, 0.5f, 0.5f,   1f,0f,0f,   0f,1f,

            // Top (0,1,0)
            -0.5f, 0.5f, 0.5f,   0f,1f,0f,   0f,0f,
             0.5f, 0.5f, 0.5f,   0f,1f,0f,   1f,0f,
             0.5f, 0.5f,-0.5f,   0f,1f,0f,   1f,1f,
            -0.5f, 0.5f,-0.5f,   0f,1f,0f,   0f,1f,

            // Bottom (0,-1,0)
            -0.5f,-0.5f,-0.5f,   0f,-1f,0f,  0f,0f,
             0.5f,-0.5f,-0.5f,   0f,-1f,0f,  1f,0f,
             0.5f,-0.5f, 0.5f,   0f,-1f,0f,  1f,1f,
            -0.5f,-0.5f, 0.5f,   0f,-1f,0f,  0f,1f,
        };

        public uint[] Indices =
        {
            // Front
            0,1,2, 2,3,0,
            // Back
            4,5,6, 6,7,4,
            // Left
            8,9,10, 10,11,8,
            // Right
            12,13,14, 14,15,12,
            // Top
            16,17,18, 18,19,16,
            // Bottom
            20,21,22, 22,23,20
        };
    }
}

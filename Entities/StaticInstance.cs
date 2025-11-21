using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

using Assignment_4.Managers;
using Assignment_4.Entities;
using Assignment_4.Rendering;
using Assignment_4.Geometry;

namespace Assignment_4.Entities
{
    public class StaticInstance
    {
        public Mesh Mesh;
        public Texture Texture;
        public Matrix4 Model;
        public Aabb WorldAabb;

        public StaticInstance(Mesh mesh, Texture tex, Matrix4 model, Aabb localAabb)
        {
            Mesh = mesh;
            Texture = tex;
            Model = model;
            WorldAabb = localAabb.Transform(model);
        }

        public void Draw(Shader shader)
        {
            shader.SetMatrix4("model", Model);

            if (Texture != null)
            {
                Texture.Use(TextureUnit.Texture0);
                shader.SetInt("useTexture", 1);
                shader.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
            }
            else
            {
                shader.SetInt("useTexture", 0);
                shader.SetVector3("objectColor", new Vector3(0.8f, 0.8f, 0.8f));
            }

            Mesh.Draw();
        }
    }
}
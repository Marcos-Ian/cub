using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace Assignment_4
{
    public class Shader
    {
        public int Handle;

        public Shader(string vertPath, string fragPath)
        {
            string vertSource = File.ReadAllText(vertPath);
            string fragSource = File.ReadAllText(fragPath);

            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, vertSource);
            GL.CompileShader(vert);

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragSource);
            GL.CompileShader(frag);

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vert);
            GL.DetachShader(Handle, frag);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetInt(string name, int value) =>
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
    }
}
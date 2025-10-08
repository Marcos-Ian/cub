using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
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
            CheckShaderCompilation(vert, "VERTEX");

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag, fragSource);
            GL.CompileShader(frag);
            CheckShaderCompilation(frag, "FRAGMENT");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vert);
            GL.AttachShader(Handle, frag);
            GL.LinkProgram(Handle);
            CheckProgramLinking(Handle);

            GL.DetachShader(Handle, vert);
            GL.DetachShader(Handle, frag);
            GL.DeleteShader(vert);
            GL.DeleteShader(frag);
        }

        public void Use() => GL.UseProgram(Handle);

        public void SetInt(string name, int value) =>
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

        public void SetFloat(string name, float value) =>
            GL.Uniform1(GL.GetUniformLocation(Handle, name), value);

        public void SetVector3(string name, Vector3 value) =>
            GL.Uniform3(GL.GetUniformLocation(Handle, name), value);

        public void SetMatrix4(string name, Matrix4 value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref value);
        }

        private void CheckShaderCompilation(int shader, string type)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                Console.WriteLine($"ERROR::SHADER::{type}::COMPILATION_FAILED\n{infoLog}");
            }
        }

        private void CheckProgramLinking(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine($"ERROR::PROGRAM::LINKING_FAILED\n{infoLog}");
            }
        }
    }
}
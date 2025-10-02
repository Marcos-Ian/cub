Textures in OpenGL, OpenTK Assignment 4.

This project demonstrates the ability to map a 2D texture to a 3D cube with OpenGL and OpenTK in C#. The cube is described in terms of vertices, indices and texture coordinates (Cube.cs). Transformations and sampling is done in shaders (Shader.cs, shader.vert, shader.frag). A texture picture is read using StbImageSharp and displayed on the cube.

Game.cs is the main loop that initializes buffers, loads the texture and applies shaders and draws the rotating cube. Program.cs will draw up the game window. The cube is rotating around the Y axis continuously with proper view and projection matrices and depth testing on.

To run, install OpenTK and StbImageSharp using NuGet, put the texture in a Textures folder and shaders in a Shaders folder and then run with dotnet run.

All of the source code, shaders, a texture image and a snapshot of the final cube are included in the submission. The program fulfills all the assignment requirements and has the optional rotation enhancement.
<img width="1834" height="839" alt="image" src="https://github.com/user-attachments/assets/c3f4e33a-e3c3-403c-94ca-37864c14f813" />

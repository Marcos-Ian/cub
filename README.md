OpenTK Rotating Cube

Library Used
In this project, OpenTK (Open Toolkit) a C# overlay of OpenGL graphics rendering is used.

How the Cube Was Rendered
The cube is constructed out of 8 vertices with each having a position and a RGB color.
The EBO defines 12 triangles (36 indices) that comprise the 6 faces of the cube.
The program sets up:
  - Vertex state in a Vertex Array Object (VAO).
  - Vertex data on a Vertex Buffer Object (VBO)
- A Vertex Index Buffer Object (VIBO) holding indications of vertex connectivity.
  - An Element Buffer Object (EBO) of indices.
Two GLSL shaders are used:
  - Vertex shader: rotates/scales/positions the cube by applying the Model-View-Projection (MVP) transformation.
  - Fragment shader: colors the cube depending on vertex colors.
A view matrix and a perspective camera projection are used to ensure that the cube is viewed in 3 dimensional space.
On each frame:
  - The cube automatically spins about the Y-axis.
  - The rotation and scaling can be carried out manually using arrow keys.
  - GL.DrawElements is used to draw the cube with depth testing on.

Start up the program and it will give you a spinning cube in 3D space that is colored in.
<img width="474" height="371" alt="Screenshot 2025-09-24 112754" src="https://github.com/user-attachments/assets/4091b069-274a-4570-96cd-fed86d7c4998" />

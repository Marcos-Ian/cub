# Assignment 4 - Phong Lighting Model

## Overview
This project implements the Phong lighting model using OpenTK and GLSL shaders in C#. The application renders a 3D cube with realistic lighting including ambient, diffuse, and specular components.

## What is Phong Lighting?

The Phong lighting model combines three components:
- **Ambient**: Base illumination (simulates indirect light)
- **Diffuse**: Light scattering based on surface angle (matte surfaces)
- **Specular**: Shiny highlights based on viewing angle (glossy surfaces)

Formula: `Final Color = (Ambient + Diffuse + Specular) × Object Color`

## Key Changes from Texture Version

1. **Cube.cs**: Changed from position+UV (5 floats) to position+normal (6 floats). Normals are required for lighting calculations.

2. **Shader.cs**: Added `SetVector3()` and `SetMatrix4()` methods to pass lighting parameters and separate transformation matrices.

3. **Game.cs**: 
   - Removed texture loading
   - Pass model, view, projection matrices separately (needed for normal transformation)
   - Added lighting uniforms: light position, camera position, light color, object color
   - Added camera controls (mouse + WASD) and light controls (arrow keys)

4. **phong.vert**: Transforms normals using normal matrix `transpose(inverse(model))` to handle non-uniform scaling correctly.

5. **phong.frag**: Implements Phong lighting with ambient (10% strength), diffuse (dot product of normal and light direction), and specular (reflection-based highlights with shininess=32).

## Controls

- **Mouse**: Look around
- **WASD**: Move camera
- **Arrow Keys**: Move light
- **ESC**: Exit

## How to Run

1. Place `phong.vert` and `phong.frag` in `Shaders/` folder in output directory
2. Build and run
3. You should see a rotating cube with dynamic lighting that responds to camera and light movement

## Implementation Notes

- Normals transformed with `transpose(inverse(model))` to preserve perpendicularity
- All lighting calculations done in world space
- Ambient strength: 0.1, Specular strength: 0.5, Shininess: 32
- <img width="1914" height="974" alt="image" src="https://github.com/user-attachments/assets/22fe4659-e861-474b-ae1e-39f1a0f840f9" />

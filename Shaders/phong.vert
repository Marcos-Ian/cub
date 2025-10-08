#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;

out vec3 FragPos;
out vec3 Normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    // Transform vertex position to world space
    FragPos = vec3(model * vec4(aPosition, 1.0));
    
    // Transform normal to world space using normal matrix
    // (inverse transpose of model matrix)
    Normal = mat3(transpose(inverse(model))) * aNormal;
    
    // Final position in clip space
    gl_Position = projection * view * vec4(FragPos, 1.0);
}
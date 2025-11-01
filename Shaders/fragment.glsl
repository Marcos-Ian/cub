#version 330 core
in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform sampler2D diffuseTex;
uniform vec3 objectColor;
uniform int useTexture;

uniform vec3 lightPos;
uniform vec3 lightColor;
uniform vec3 viewPos;
uniform float lightEnabled;
uniform float shininess;

uniform float uOpacity;      // used by your passes
uniform int   uInvertColors; // invert toggle

void main()
{
    vec3 albedo = (useTexture == 1)
        ? texture(diffuseTex, TexCoord).rgb
        : objectColor;

    vec3 N = normalize(Normal);
    vec3 L = normalize(lightPos - FragPos);
    vec3 V = normalize(viewPos - FragPos);

    float on = lightEnabled > 0.5 ? 1.0 : 0.0;

    vec3 ambient  = 0.15 * albedo * on;
    float diff    = max(dot(N, L), 0.0);
    vec3 diffuse  = diff * albedo * lightColor * on;

    vec3 R        = reflect(-L, N);
    float spec    = pow(max(dot(V, R), 0.0), shininess);
    vec3 specular = spec * lightColor * 0.5 * on;

    vec3 color = ambient + diffuse + specular;

    if (uInvertColors == 1)
        color = vec3(1.0) - color;

    FragColor = vec4(color, uOpacity);
}

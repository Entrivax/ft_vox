#version 400

in vec2 vertexUv;
in vec3 normal;
in vec3 tint;

uniform vec3 col;
uniform sampler2D tex;

out vec4 color;

vec3 lightDir = vec3(0.7, -1, 0.9);

void main(void)
{
//color = vec4(1, 1, 1, 1);
    vec4 texColor = texture(tex, vertexUv);
    if (texColor.w < 1)
        discard;
	float intensity = 0.6 + clamp((dot(normal, -lightDir) * 0.3), 0, 1);
	color = texColor * intensity * vec4(tint, 1);
    //color = texColor;
}

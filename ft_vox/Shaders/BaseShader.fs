#version 400

in vec3 norm;
in vec2 uv;

uniform vec3 col;
uniform sampler2D tex;

out vec4 color;

vec3 lightDir = vec3(2, -1, 1);

void main(void)
{
//color = vec4(1, 1, 1, 1);
    vec4 texColor = texture(tex, uv);
    if (texColor.w < 1)
        discard;
	float intensity = 0.6 + clamp((dot(norm, -lightDir) * 0.3), 0, 1);
	color = texColor * intensity;
}

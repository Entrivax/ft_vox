#version 400

in vec2 uv;

uniform vec3 col;
uniform sampler2D tex;

out vec4 color;

vec3 lightDir = vec3(2, -1, 1);

void main(void)
{
//color = vec4(1, 1, 1, 1);
	color = texture(tex, uv) * vec4(col, 1);
}

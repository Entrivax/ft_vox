#version 400

in vec2 uv;

uniform vec3 col;
uniform sampler2D tex;

out vec4 color;

void main(void)
{
	color = texture(tex, uv) * vec4(col, 1);
}
#version 400

in vec3 _pos;
in vec3 _pos2;
in vec4 _col;

out vec3 pos2;
out vec4 col;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	pos2 = _pos2;
	col = _col;
	gl_Position = vec4(_pos, 1.0);
}
#version 400

in vec3 _pos;
in int _type;

out int type;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	type = _type;
	gl_Position = vec4(_pos, 1.0);
}
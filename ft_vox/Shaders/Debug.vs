#version 400

in vec3 _pos;
in vec3 _pos2;
in int _type;

flat out int type;
flat out vec3 pos2;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	type = _type;
	pos2 = _pos2;
	gl_Position = vec4(_pos, 1.0);
}
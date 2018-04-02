#version 400

in vec3 _pos;
in int _blockIdAndVisibility;

out int blockIdAndVisibility;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	blockIdAndVisibility = _blockIdAndVisibility;
	gl_Position = vec4(_pos, 1.0);
}
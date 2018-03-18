#version 400

in vec3 _pos;
in int _blockIdAndVisibility;

out int blockIdAndVisibility;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	//vec3 posCorrected = vec3(_pos.x, _pos.y, _pos.z);
	//vec4 viewspace = view * vec4(posCorrected, 1.0);
	blockIdAndVisibility = _blockIdAndVisibility;
	gl_Position = vec4(_pos, 1.0);
	//norm = _norm;
	//uv = _uv;
	//recolor = _color;
}
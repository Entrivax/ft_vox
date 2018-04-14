#version 400

in vec3 _pos;
in vec2 _uv;

out vec2 uv;

uniform vec2 uvSize;
uniform int index;
uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	vec3 posCorrected = vec3(_pos.x, _pos.y, _pos.z);
	vec4 viewspace = view * vec4(posCorrected, 1.0);
	gl_Position = proj * viewspace;
	uv = _uv + vec2((uvSize.x * index), 0);
}
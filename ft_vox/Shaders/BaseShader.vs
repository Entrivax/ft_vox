#version 400

in vec3 _pos;
in vec3 _norm;
in vec2 _uv;

out vec3 norm;
out vec2 uv;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	vec3 posCorrected = vec3(_pos.x, _pos.y, _pos.z);
	vec4 viewspace = view * vec4(posCorrected, 1.0);
	gl_Position = proj * viewspace;
	norm = _norm;
	uv = _uv;
}
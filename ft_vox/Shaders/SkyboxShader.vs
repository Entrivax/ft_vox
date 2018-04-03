#version 400

in vec3 _pos;
in vec2 _uv;

out vec2 uv;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	vec4 viewspace = view * vec4(_pos, 1.0);
	gl_Position = proj * viewspace;
	uv = _uv;
}
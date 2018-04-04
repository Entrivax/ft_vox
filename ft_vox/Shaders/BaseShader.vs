#version 400

in vec3 _pos;
in int _blockIdAndVisibility;
in int _humidityAndTemperature;

out int blockIdAndVisibility;
out int humidityAndTemperature;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	blockIdAndVisibility = _blockIdAndVisibility;
	humidityAndTemperature = _humidityAndTemperature;
	gl_Position = vec4(_pos, 1.0);
}
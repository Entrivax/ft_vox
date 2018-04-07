#version 400

in vec3 _pos;
in int _blockIdAndVisibilityAndMetadata;
in int _humidityAndTemperature;

out int blockIdAndVisibilityAndMetadata;
out int humidityAndTemperature;

uniform mat4 proj;
uniform mat4 view;

void main(void)
{
	blockIdAndVisibilityAndMetadata = _blockIdAndVisibilityAndMetadata;
	humidityAndTemperature = _humidityAndTemperature;
	gl_Position = vec4(_pos, 1.0);
}
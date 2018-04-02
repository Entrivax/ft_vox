#version 400

in vec3 col;
out vec4 color;

void main(void)
{
    color = vec4(col, 1);
}

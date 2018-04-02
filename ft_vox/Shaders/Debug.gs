#version 400

layout(points) in;
layout(line_strip, max_vertices = 6) out;

uniform mat4 proj;
uniform mat4 view;
uniform vec3 cameraPosition;

in int type[];

out vec3 col;

mat4 projView;

void main()
{
	int t = type[0];
	projView = proj * view;

    if (t == 0)
    {
        col = vec3(1, 0, 0);
        gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(-1, 0, 0, 0));
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, -1, 0, 0));
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, -1, 0));
        EmitVertex();
        EndPrimitive();
    }
    else if (t == 1)
    {
        gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
        col = vec3(1, 0, 0);
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
        col = vec3(1, 0, 0);
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
        col = vec3(0, 1, 0);
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
        col = vec3(0, 1, 0);
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
        col = vec3(0, 0, 1);
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
        col = vec3(0, 0, 1);
        EmitVertex();
        EndPrimitive();
    }
}
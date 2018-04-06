#version 400

layout(points) in;
layout(line_strip, max_vertices = 17) out;

uniform mat4 proj;
uniform mat4 view;
uniform vec3 cameraPosition;

in vec3 pos2[];
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
    else if (t == 2)
    {
        col = vec3(1, 1, 0);
        gl_Position = projView * (gl_in[0].gl_Position);
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, gl_in[0].gl_Position.yzw));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, pos2[0].y, gl_in[0].gl_Position.zw));
        EmitVertex();
        gl_Position = projView * (vec4(gl_in[0].gl_Position.x, pos2[0].y, gl_in[0].gl_Position.zw));
        EmitVertex();
        gl_Position = projView * (gl_in[0].gl_Position);
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (gl_in[0].gl_Position);
        EmitVertex();
        gl_Position = projView * (vec4(gl_in[0].gl_Position.xy, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, gl_in[0].gl_Position.y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, gl_in[0].gl_Position.yzw));
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (vec4(gl_in[0].gl_Position.xy, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(gl_in[0].gl_Position.x, pos2[0].y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, pos2[0].y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, gl_in[0].gl_Position.y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (vec4(gl_in[0].gl_Position.x, pos2[0].y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(gl_in[0].gl_Position.x, pos2[0].y, gl_in[0].gl_Position.zw));
        EmitVertex();
        EndPrimitive();
        
        gl_Position = projView * (vec4(pos2[0].x, pos2[0].y, pos2[0].z, gl_in[0].gl_Position.w));
        EmitVertex();
        gl_Position = projView * (vec4(pos2[0].x, pos2[0].y, gl_in[0].gl_Position.zw));
        EmitVertex();
        EndPrimitive();
    }
}
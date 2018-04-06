#version 400

layout(points) in;
layout(line_strip, max_vertices = 17) out;

uniform mat4 proj;
uniform mat4 view;
uniform vec3 cameraPosition;

in vec3 pos2[];
in vec4 col[];

out vec4 fcol;

mat4 projView;

void main()
{
	projView = proj * view;
    fcol = col[0];
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
#version 400

#define IS_TOP_VISIBLE(bVi) ((bVi & 256) > 0 && cameraPosition.y > gl_in[0].gl_Position.y)
#define IS_BOTTOM_VISIBLE(bVi) ((bVi & 512) > 0 && cameraPosition.y < gl_in[0].gl_Position.y)
#define IS_FRONT_VISIBLE(bVi) ((bVi & 1024) > 0 && cameraPosition.z < gl_in[0].gl_Position.z)
#define IS_BACK_VISIBLE(bVi) ((bVi & 2048) > 0 && cameraPosition.z > gl_in[0].gl_Position.z)
#define IS_LEFT_VISIBLE(bVi) ((bVi & 4096) > 0 && cameraPosition.x < gl_in[0].gl_Position.x)
#define IS_RIGHT_VISIBLE(bVi) ((bVi & 8192) > 0 && cameraPosition.x > gl_in[0].gl_Position.x)

layout(points) in;
layout(triangle_strip, max_vertices = 36) out;

in int blockIdAndVisibility[];

out vec2 vertexUv;
out vec3 normal;

uniform mat4 proj;
uniform mat4 view;
uniform vec3 cameraPosition;

mat4 projView;
const float uvWidth = 1.0 / 16.0;

void drawGrassBlock(int blockVisibility) {
	// TOP FACE
	if(IS_TOP_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(0, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(0, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(uvWidth, uvWidth);
		EmitVertex();
		EndPrimitive();
	}

	// BOTTOM FACE
	if(IS_BOTTOM_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, -1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(3 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(2 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(3 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(2 * uvWidth, uvWidth);
		EmitVertex();
		EndPrimitive();
	}

	// FRONT FACE
	if(IS_FRONT_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, -1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(4 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(4 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(3 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(3 * uvWidth, 0);
		EmitVertex();
		EndPrimitive();
	}

	// BACK FACE
	if(IS_BACK_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, 1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(3 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(4 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(3 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(4 * uvWidth, 0);
		EmitVertex();
		EndPrimitive();
	}

	// LEFT FACE
	if(IS_LEFT_VISIBLE(blockVisibility))
	{
	    normal = vec3(-1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(3 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(4 * uvWidth, uvWidth) ;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(3 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(4 * uvWidth, 0);
		EmitVertex();
		EndPrimitive();
	}

	// RIGHT FACE
	if(IS_RIGHT_VISIBLE(blockVisibility))
	{
	    normal = vec3(1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(4 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(4 * uvWidth, 0);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(3 * uvWidth, uvWidth);
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(3 * uvWidth, 0);
		EmitVertex();
		EndPrimitive();
	}
}

void drawClassicBlock(int id, int blockVisibility) {
	if (id == 0)
		return;
	vec2 offset = vec2(0, 0);
	switch (id)
	{
		case 1:
			offset = vec2(uvWidth, 0);
			break;
		case 3:
			offset = vec2(uvWidth * 2, 0);
			break;
	}
	vec2 topLeftUv = vec2(0, 0) + offset;
	vec2 topRightUv = vec2(uvWidth, 0) + offset;
	vec2 bottomLeftUv = vec2(0, uvWidth) + offset;
	vec2 bottomRightUv = vec2(uvWidth, uvWidth) + offset;

	// TOP FACE
	if(IS_TOP_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = topRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		EndPrimitive();
	}

	// BOTTOM FACE
	if(IS_BOTTOM_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, -1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = topRightUv;
		EmitVertex();
		EndPrimitive();
	}

	// FRONT FACE
	if(IS_FRONT_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, -1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = topRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		EndPrimitive();
	}

	// BACK FACE
	if(IS_BACK_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, 1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = topRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		EndPrimitive();
	}

	// LEFT FACE
	if(IS_LEFT_VISIBLE(blockVisibility))
	{
	    normal = vec3(-1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = topRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		EndPrimitive();
	}

	// RIGHT FACE
	if(IS_RIGHT_VISIBLE(blockVisibility))
	{
	    normal = vec3(1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = bottomRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = bottomLeftUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = topRightUv;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = topLeftUv;
		EmitVertex();
		EndPrimitive();
	}
}

//0.1464
//0.8536
void drawTallGrassBlock() {
    vec2 offset = vec2(uvWidth * 7, uvWidth * 2);
	vec2 topLeftUv = vec2(0, 0) + offset;
	vec2 topMiddleUv = vec2(uvWidth / 2, 0) + offset;
	vec2 topRightUv = vec2(uvWidth, 0) + offset;
	vec2 bottomLeftUv = vec2(0, uvWidth) + offset;
	vec2 bottomMiddleUv = vec2(uvWidth / 2, uvWidth) + offset;
	vec2 bottomRightUv = vec2(uvWidth, uvWidth) + offset;
	
    normal = vec3(0, 1, 0);

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 0, 0.1464, 0));
    vertexUv = bottomLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 1, 0.1464, 0));
    vertexUv = topLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    EndPrimitive();

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 0, 0.8536, 0));
    vertexUv = bottomRightUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 1, 0.8536, 0));
    vertexUv = topRightUv;
    EmitVertex();
    EndPrimitive();

    

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 0, 0.8536, 0));
    vertexUv = bottomLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 1, 0.8536, 0));
    vertexUv = topLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    EndPrimitive();

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 0, 0.1464, 0));
    vertexUv = bottomRightUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 1, 0.1464, 0));
    vertexUv = topRightUv;
    EmitVertex();
    EndPrimitive();

    

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 0, 0.1464, 0));
    vertexUv = bottomLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 1, 0.1464, 0));
    vertexUv = topLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    EndPrimitive();

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 0, 0.8536, 0));
    vertexUv = bottomRightUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 1, 0.8536, 0));
    vertexUv = topRightUv;
    EmitVertex();
    EndPrimitive();

    

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 0, 0.8536, 0));
    vertexUv = bottomLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.8536, 1, 0.8536, 0));
    vertexUv = topLeftUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    EndPrimitive();

    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 0, 0.5, 0));
    vertexUv = bottomMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 0, 0.1464, 0));
    vertexUv = bottomRightUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.5, 1, 0.5, 0));
    vertexUv = topMiddleUv;
    EmitVertex();
    gl_Position = projView * (gl_in[0].gl_Position + vec4(0.1464, 1, 0.1464, 0));
    vertexUv = topRightUv;
    EmitVertex();
    EndPrimitive();
}

void main()
{
	int id = blockIdAndVisibility[0] & 255;
	projView = proj * view;

	if (id == 2)
	{
		drawGrassBlock(blockIdAndVisibility[0]);
	}
	if (id == 31)
	{
        drawTallGrassBlock();
	}
	else
	{
		drawClassicBlock(id, blockIdAndVisibility[0]);
	}
}
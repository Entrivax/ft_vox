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
in int humidityAndTemperature[];

out vec2 vertexUv;
out vec3 tint;
out vec3 normal;

uniform mat4 proj;
uniform mat4 view;
uniform vec3 cameraPosition;

mat4 projView;
const float uvWidth = 1.0 / 16.0;

void drawGrassBlock(int blockVisibility, vec3 grassColor) {
	// TOP FACE
	if(IS_TOP_VISIBLE(blockVisibility))
	{
	    tint = grassColor;
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
	
	tint = vec3(1, 1, 1);

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

void drawMultiTexturedBlock(int blockVisibility, vec2 topOffset, vec2 bottomOffset, vec2 sideOffset) {
	
	// TOP FACE
	if(IS_TOP_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(0, 0) + topOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(0, uvWidth) + topOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(uvWidth, 0) + topOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(uvWidth, uvWidth) + topOffset;
		EmitVertex();
		EndPrimitive();
	}

	// BOTTOM FACE
	if(IS_BOTTOM_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, -1, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(uvWidth, 0) + bottomOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(0, 0) + bottomOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(uvWidth, uvWidth) + bottomOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(0, uvWidth) + bottomOffset;
		EmitVertex();
		EndPrimitive();
	}

	// FRONT FACE
	if(IS_FRONT_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, -1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(uvWidth, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(uvWidth, 0) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(0, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(0, 0) + sideOffset;
		EmitVertex();
		EndPrimitive();
	}

	// BACK FACE
	if(IS_BACK_VISIBLE(blockVisibility))
	{
	    normal = vec3(0, 0, 1);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(0, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(uvWidth, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(0, 0) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(uvWidth, 0) + sideOffset;
		EmitVertex();
		EndPrimitive();
	}

	// LEFT FACE
	if(IS_LEFT_VISIBLE(blockVisibility))
	{
	    normal = vec3(-1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 0, 0));
		vertexUv = vec2(0, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 0, 1, 0));
		vertexUv = vec2(uvWidth, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 0, 0));
		vertexUv = vec2(0, 0) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(0, 1, 1, 0));
		vertexUv = vec2(uvWidth, 0) + sideOffset;
		EmitVertex();
		EndPrimitive();
	}

	// RIGHT FACE
	if(IS_RIGHT_VISIBLE(blockVisibility))
	{
	    normal = vec3(1, 0, 0);
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 0, 0));
		vertexUv = vec2(uvWidth, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 0, 0));
		vertexUv = vec2(uvWidth, 0) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 0, 1, 0));
		vertexUv = vec2(0, uvWidth) + sideOffset;
		EmitVertex();
		gl_Position = projView * (gl_in[0].gl_Position + vec4(1, 1, 1, 0));
		vertexUv = vec2(0, 0) + sideOffset;
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
		case 12:
		    offset = vec2(uvWidth * 2, uvWidth);
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
void drawTallGrassBlock(vec3 grassColor) {
    vec2 offset = vec2(uvWidth * 7, uvWidth * 2);
	vec2 topLeftUv = vec2(0, 0) + offset;
	vec2 topMiddleUv = vec2(uvWidth / 2, 0) + offset;
	vec2 topRightUv = vec2(uvWidth, 0) + offset;
	vec2 bottomLeftUv = vec2(0, uvWidth) + offset;
	vec2 bottomMiddleUv = vec2(uvWidth / 2, uvWidth) + offset;
	vec2 bottomRightUv = vec2(uvWidth, uvWidth) + offset;
	
    normal = vec3(0, 1, 0);
    tint = grassColor;

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

vec3 map(vec3 value, vec3 inMin, vec3 inMax, vec3 outMin, vec3 outMax) {
  return outMin + (outMax - outMin) * (value - inMin) / (inMax - inMin);
}

vec3 map2D(vec2 pos, vec3 downleft, vec3 upleft, vec3 downright)
{
    return (map(vec3(pos.x), vec3(0), vec3(1), downleft, downright)
     + map(vec3(pos.y), vec3(0), vec3(1), downleft, upleft)) / 2.0;
}

void main()
{
	int id = blockIdAndVisibility[0] & 255;
	int temperature = humidityAndTemperature[0] & 255;
	int humidity = (humidityAndTemperature[0] >> 8) & 255;
	vec3 grassColor = map2D(vec2(temperature / 255.0, humidity / 255.0),
        vec3(0.749, 0.714, 0.361),
        vec3(0.302, 0.796, 0.247),
        vec3(0.505, 0.702, 0.596)) + 0.1;
	projView = proj * view;

	tint = vec3(1, 1, 1);
	if (id == 2)
	{
		drawGrassBlock(blockIdAndVisibility[0], grassColor);
	}
	else if (id == 31)
	{
        drawTallGrassBlock(grassColor);
	}
	else if (id == 24)
	{
	    drawMultiTexturedBlock(blockIdAndVisibility[0], vec2(0, uvWidth * 11), vec2(0, uvWidth * 13), vec2(0, uvWidth * 12)); 
	}
	else
	{
		drawClassicBlock(id, blockIdAndVisibility[0]);
	}
}
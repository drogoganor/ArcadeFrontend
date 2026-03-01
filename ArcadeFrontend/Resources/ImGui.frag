#version 450

layout(location = 0) in vec2 fsin_texCoord;
layout(location = 1) in vec4 fsin_color;
layout(location = 2) in vec4 fsin_position;

layout(location = 0) out vec4 fsout_color;

layout(set = 2, binding = 0) uniform texture2D Texture; // 2,0 seems to be correct
layout(set = 2, binding = 0) uniform sampler Sampler;  // 2,0 seems to be correct??

void main()
{
    fsout_color = texture(sampler2D(Texture, Sampler), fsin_texCoord) * fsin_color;
}

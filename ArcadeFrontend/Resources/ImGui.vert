#version 450

layout(set = 1, binding = 0) uniform UniformBlock
{
    mat4 Matrix;
};

layout(location = 0) in vec2 Position;
layout(location = 1) in vec2 TexCoord;
layout(location = 2) in vec4 Color;

layout(location = 0) out vec2 fsin_texCoord;
layout(location = 1) out vec4 fsin_color;
layout(location = 2) out vec4 fsin_position;

void main()
{
    fsin_texCoord = TexCoord;
    fsin_color = Color;
    fsin_position = Matrix * vec4(Position, 0.0, 1.0);
    gl_Position = fsin_position;
}
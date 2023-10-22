#version 450

layout(location = 0) in vec3 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;

layout(set = 1, binding = 1) uniform texture2DArray SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;

// By Lucius - Only for pixel-level coords
vec2 bilinearSharpness(vec2 uv, float sharpness)
{
    // Determine the edges
    if (sharpness == 1.0)
    {
        vec2 w = fwidth(uv);
        float negMip = -0.5 * log2(dot(w, w));    // ranges from 1 at 200% scale and 0 at 100% scale
        sharpness = clamp(negMip, 0.0, 1.0)*0.5 + 0.5;
    }
    // Sharpness == 0 is the same as standard bilinear.
    if (sharpness == 0.0) { return uv; }
    float e0 = 0.0, e1 = 1.0;
    if (sharpness > 0.5)
    {
        // Adjust the endpoints of the curve inward toward the center.
        e0 = 0.4 * (sharpness - 0.5) * 2.0;
        e1 = 1.0 - e0;
    }
    vec2 offset = vec2(0.5);
    // Deconstruct the uv coordinate into integer and fractional components.
    uv += offset;
    vec2 iuv = floor(uv);
    vec2 fuv = uv - iuv;

    // Adjusted uv using an s-curve where S(0.5) = 0.5
    vec2 uvAdj = smoothstep(e0, e1, fuv);
    // Blend between the standard linear and adjusted coordinates.
    float blendFactor = clamp(sharpness * 2.0, 0.0, 1.0); // 0 @ sharpness = 0, 1 @ sharpess >= 0.5
    fuv = mix(fuv, uvAdj, blendFactor);
    
    // Reconstruct the uv coordinate.
    return iuv + fuv - offset;
}

void main()
{
    fsout_color =  texture(sampler2DArray(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}
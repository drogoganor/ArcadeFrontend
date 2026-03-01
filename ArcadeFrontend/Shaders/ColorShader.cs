using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Data;
using static SDL3.SDL;
using static ArcadeFrontend.Shaders.ShaderCommon;

namespace ArcadeFrontend.Shaders;

public class ColorShader : IShader
{
    public nint SdlPipeline { get; private set; }

    private VertexFragmentShaderPair shader;

    private readonly IApplicationWindow window;
    private readonly IFileSystem fileSystem;

    public ColorShader(
        IApplicationWindow window,
        IFileSystem fileSystem)
    {
        this.window = window;
        this.fileSystem = fileSystem;
    }

    public unsafe void Load()
    {
        shader = InitShader(window.Device, "WorldColor");

        var vertexBuffDesc = stackalloc SDL_GPUVertexBufferDescription[1]
        {
            new()
            {
                slot = 0,
                pitch = VertexPositionColor.SizeInBytes,
                input_rate = SDL_GPUVertexInputRate.SDL_GPU_VERTEXINPUTRATE_VERTEX,
                instance_step_rate = 0
            }
        };

        var vertexAttr = stackalloc SDL_GPUVertexAttribute[2]
        {
            // Position : float3
            new()
            {
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT3,
                location = 0,
                offset = 0
            },

            // Color : float4
            new()
            {
                format = SDL_GPUVertexElementFormat.SDL_GPU_VERTEXELEMENTFORMAT_FLOAT4,
                location = 1,
                offset = sizeof(float) * 3
            }
        };

        var colorTargetDesc = stackalloc SDL_GPUColorTargetDescription[1]
        {
            new()
            {
                format = SDL_GetGPUSwapchainTextureFormat(window.Device, window.Window),
                blend_state = new()
                {
                    src_color_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_SRC_ALPHA,
                    dst_color_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_ONE_MINUS_SRC_ALPHA,
                    color_blend_op = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD,
                    src_alpha_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_ONE,
                    dst_alpha_blendfactor = SDL_GPUBlendFactor.SDL_GPU_BLENDFACTOR_ONE_MINUS_SRC_ALPHA,
                    alpha_blend_op = SDL_GPUBlendOp.SDL_GPU_BLENDOP_ADD,
                    enable_blend = true,
                    enable_color_write_mask = false,
                }
            }
        };

        SdlPipeline = SDL_CreateGPUGraphicsPipeline(window.Device, new SDL_GPUGraphicsPipelineCreateInfo
        {
            vertex_shader = shader.VertexShader,
            fragment_shader = shader.FragmentShader,
            vertex_input_state = new()
            {
                vertex_buffer_descriptions = vertexBuffDesc,
                num_vertex_buffers = 1,
                vertex_attributes = vertexAttr,
                num_vertex_attributes = 2,
            },
            primitive_type = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST,
            rasterizer_state = new()
            {
                cull_mode = SDL_GPUCullMode.SDL_GPU_CULLMODE_NONE,
            },
            multisample_state = new(),
            depth_stencil_state = new()
            {
                enable_depth_test = true,
                enable_depth_write = true,
                compare_op = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_LESS_OR_EQUAL,
            },
            target_info = new()
            {
                num_color_targets = 1,
                color_target_descriptions = colorTargetDesc,
                has_depth_stencil_target = true,
                depth_stencil_format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_D24_UNORM_S8_UINT
            }
        });
    }

    public void Unload()
    {
        SDL_ReleaseGPUShader(window.Device, shader.VertexShader);
        SDL_ReleaseGPUShader(window.Device, shader.FragmentShader);
        SDL_ReleaseGPUGraphicsPipeline(window.Device, SdlPipeline);
    }
}

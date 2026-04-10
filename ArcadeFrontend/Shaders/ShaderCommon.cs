using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using System.Text;
using static SDL3.SDL;

namespace ArcadeFrontend.Shaders;

public class VertexFragmentShaderPair
{
    public nint VertexShader { get; set; }
    public nint FragmentShader { get; set; }
}

public static class ShaderCommon
{
    public static unsafe VertexFragmentShaderPair InitShader(
        nint device,
        string shaderName,
        uint numVertexUniformBuffers = 3,
        uint numFragmentUniformBuffers = 0,
        uint numSamplers = 0,
        uint numStorageTextures = 0)
    {
        // get shader language
        var driver = SDL_GetGPUDeviceDriver(device);
        var shaderFormat = driver switch
        {
            "private" => SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_PRIVATE,
            "vulkan" => SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV,
            "direct3d12" => SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL,
            "metal" => SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL,
            _ => throw new NotImplementedException($"Unknown Shader Format for Driver '{driver}'")
        };

        var shaderExt = shaderFormat switch
        {
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_PRIVATE => "spv",
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV => "spv",
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL => "dxil",
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL => "msl",
            _ => throw new NotImplementedException($"Unimplemented Shader Format '{shaderFormat}'")
        };

        var result = new VertexFragmentShaderPair();

        var vertexCode = GetEmbeddedBytes($"{shaderName}.vert.{shaderExt}");
        //var vertexEntry = Encoding.UTF8.GetBytes("vertex_main");
        var vertexEntry = Encoding.UTF8.GetBytes("main"); // "vertex_main"

        var fragmentCode = GetEmbeddedBytes($"{shaderName}.frag.{shaderExt}");
        //var fragmentEntry = Encoding.UTF8.GetBytes("fragment_main");
        var fragmentEntry = Encoding.UTF8.GetBytes("main"); // "fragment_main"

        fixed (byte* vertexCodePtr = vertexCode)
        fixed (byte* vertexEntryPtr = vertexEntry)
        {
            result.VertexShader = SDL_CreateGPUShader(device, new()
            {
                code_size = (uint)vertexCode.Length,
                code = vertexCodePtr,
                entrypoint = vertexEntryPtr,
                format = shaderFormat,
                stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX,
                num_samplers = 0,
                num_storage_textures = 0,
                num_storage_buffers = 0,
                num_uniform_buffers = numVertexUniformBuffers
            });

            if (result.VertexShader == nint.Zero)
                throw new Exception($"{nameof(SDL_CreateGPUShader)} Failed: {SDL_GetError()}");
        }

        fixed (byte* fragmentCodePtr = fragmentCode)
        fixed (byte* fragmentEntryPtr = fragmentEntry)
        {
            result.FragmentShader = SDL_CreateGPUShader(device, new()
            {
                code_size = (uint)fragmentCode.Length,
                code = fragmentCodePtr,
                entrypoint = fragmentEntryPtr,
                format = shaderFormat,
                stage = SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT,
                num_samplers = numSamplers,
                num_storage_textures = numStorageTextures,
                num_storage_buffers = 0,
                num_uniform_buffers = numFragmentUniformBuffers
            });

            if (result.FragmentShader == nint.Zero)
                throw new Exception($"{nameof(SDL_CreateGPUShader)} Failed: {SDL_GetError()}");
        }

        return result;
    }

    /// <summary>
    /// Shaders are stored as Embedded files (see ImGuiSDL.csproj)
    /// </summary>
    private static byte[] GetEmbeddedBytes(string file)
    {
        var assembly = typeof(Sdl3ImGuiRenderer).Assembly;
        using var stream = assembly.GetManifestResourceStream(file);
        if (stream != null)
        {
            var result = new byte[stream.Length];
            stream.ReadExactly(result);
            return result;
        }

        throw new Exception($"Failed to load Embedded file '{file}'");
    }

    public static unsafe nint BuildVertexBuffer<T>(nint device, uint dataSize, ref ReadOnlySpan<T> data, bool isIndex = false) where T : unmanaged
    {
        var cmd = SDL_AcquireGPUCommandBuffer(device);

        // begin GPU copy pass (upload buffers)
        var copy = SDL_BeginGPUCopyPass(cmd);

        var vertexBuffer = SDL_CreateGPUBuffer(device, new()
        {
            usage = !isIndex ? SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_VERTEX : SDL_GPUBufferUsageFlags.SDL_GPU_BUFFERUSAGE_INDEX,
            size = (uint)dataSize
        });

        if (vertexBuffer == nint.Zero)
            throw new Exception($"{nameof(SDL_CreateGPUBuffer)} Failed: {SDL_GetError()}");

        // TODO: cache this! reuse transfer buffer!
        var transferBuffer = SDL_CreateGPUTransferBuffer(device, new()
        {
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
            size = dataSize
        });

        // copy data
        fixed (T* src = data)
        {
            byte* dst = (byte*)SDL_MapGPUTransferBuffer(device, transferBuffer, false);
            Buffer.MemoryCopy(src, dst, dataSize, dataSize);
            SDL_UnmapGPUTransferBuffer(device, transferBuffer);
        }

        // submit to the GPU
        SDL_UploadToGPUBuffer(copy,
            source: new()
            {
                transfer_buffer = transferBuffer,
                offset = 0
            },
            destination: new()
            {
                buffer = vertexBuffer,
                offset = 0,
                size = (uint)dataSize
            },
            cycle: false
        );

        // release transfer buffer
        SDL_ReleaseGPUTransferBuffer(device, transferBuffer);
        SDL_EndGPUCopyPass(copy);

        SDL_SubmitGPUCommandBuffer(cmd);

        return vertexBuffer;
    }

    /*
    public static unsafe nint LoadImageSharpImage(IApplicationWindow window, Image<Rgba32> image, out int width, out int height)
    {
        var span = new Span<Rgba32>(new Rgba32[image.Width * image.Height]);
        image.CopyPixelDataTo(span);

        SDL_Surface* surface;
        fixed (Rgba32* pixelPtr = span)
        {
            surface = SDL_CreateSurfaceFrom(image.Width, image.Height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888, (nint)pixelPtr, image.Width * 4);
        }

        if (surface == null)
        {
            throw new Exception($"Couldn't load image BMP data in LoadImageSharpImage");
        }

        //SDL_PIXELFORMAT_ABGR8888

        width = surface->w;
        height = surface->h;

        var size = surface->w * surface->h * 4;

        var texture = SDL_CreateGPUTexture(window.Device, new()
        {
            type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM,
            usage = SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_SAMPLER,
            width = (uint)surface->w,
            height = (uint)surface->h,
            layer_count_or_depth = 1,
            num_levels = 1,
            sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1
        });

        // upload texture data
        var transferBuffer = SDL_CreateGPUTransferBuffer(window.Device, new()
        {
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
            size = (uint)size
        });

        var transferPtr = SDL_MapGPUTransferBuffer(window.Device, transferBuffer, false);
        Buffer.MemoryCopy((void*)surface->pixels, (void*)transferPtr, size, size);
        SDL_UnmapGPUTransferBuffer(window.Device, transferBuffer);

        var cmd = SDL_AcquireGPUCommandBuffer(window.Device);
        var pass = SDL_BeginGPUCopyPass(cmd);

        SDL_UploadToGPUTexture(pass,
            source: new()
            {
                transfer_buffer = transferBuffer,
                offset = 0,
            },
            destination: new()
            {
                texture = texture,
                w = (uint)surface->w,
                h = (uint)surface->h,
                d = 1
            },
            cycle: false
        );

        SDL_EndGPUCopyPass(pass);
        SDL_SubmitGPUCommandBuffer(cmd);
        SDL_ReleaseGPUTransferBuffer(window.Device, transferBuffer);

        SDL_DestroySurface((nint)surface);

        return texture;
    }
    */

    public static unsafe nint LoadImage(IApplicationWindow window, string filename, int desiredChannels, out int width, out int height)
    {
        var surface = SDL_LoadPNG(filename);
        if (surface == null)
        {
            throw new Exception($"Couldn't load PNG {filename}");
        }

        var convertedSurface = SDL_ConvertSurface((nint)surface, SDL_PixelFormat.SDL_PIXELFORMAT_ABGR8888);

        width = convertedSurface->w;
        height = convertedSurface->h;

        var size = convertedSurface->w * convertedSurface->h * 4;

        var texture = SDL_CreateGPUTexture(window.Device, new()
        {
            type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
            format = SDL_GPUTextureFormat.SDL_GPU_TEXTUREFORMAT_R8G8B8A8_UNORM,
            usage = SDL_GPUTextureUsageFlags.SDL_GPU_TEXTUREUSAGE_SAMPLER,
            width = (uint)convertedSurface->w,
            height = (uint)convertedSurface->h,
            layer_count_or_depth = 1,
            num_levels = 1,
            sample_count = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1
        });

        // upload texture data
        var transferBuffer = SDL_CreateGPUTransferBuffer(window.Device, new()
        {
            usage = SDL_GPUTransferBufferUsage.SDL_GPU_TRANSFERBUFFERUSAGE_UPLOAD,
            size = (uint)size
        });

        var transferPtr = SDL_MapGPUTransferBuffer(window.Device, transferBuffer, false);
        Buffer.MemoryCopy((void*)convertedSurface->pixels, (void*)transferPtr, size, size);
        SDL_UnmapGPUTransferBuffer(window.Device, transferBuffer);

        var cmd = SDL_AcquireGPUCommandBuffer(window.Device);
        var pass = SDL_BeginGPUCopyPass(cmd);

        SDL_UploadToGPUTexture(pass,
            source: new()
            {
                transfer_buffer = transferBuffer,
                offset = 0,
            },
            destination: new()
            {
                texture = texture,
                w = (uint)convertedSurface->w,
                h = (uint)convertedSurface->h,
                d = 1
            },
            cycle: false
        );

        SDL_EndGPUCopyPass(pass);
        SDL_SubmitGPUCommandBuffer(cmd);
        SDL_ReleaseGPUTransferBuffer(window.Device, transferBuffer);

        SDL_DestroySurface((nint)convertedSurface);
        SDL_DestroySurface((nint)surface);

        return texture;
    }

    /*

    SDL_Surface* LoadImage(const char* imageFilename, int desiredChannels)
    {
	    char fullPath[256];
	    SDL_Surface *result;
	    SDL_PixelFormat format;

	    SDL_snprintf(fullPath, sizeof(fullPath), "%sContent/Images/%s", BasePath, imageFilename);

	    result = SDL_LoadBMP(fullPath);
	    if (result == NULL)
	    {
		    SDL_Log("Failed to load BMP: %s", SDL_GetError());
		    return NULL;
	    }

	    if (desiredChannels == 4)
	    {
		    format = SDL_PIXELFORMAT_ABGR8888;
	    }
	    else
	    {
		    SDL_assert(!"Unexpected desiredChannels");
		    SDL_DestroySurface(result);
		    return NULL;
	    }
	    if (result->format != format)
	    {
		    SDL_Surface *next = SDL_ConvertSurface(result, format);
		    SDL_DestroySurface(result);
		    result = next;
	    }

	    return result;
    }
    */
}

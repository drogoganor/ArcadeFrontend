using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Shaders;
using System.Numerics;
using System.Runtime.InteropServices;
using static SDL3.SDL;

namespace ArcadeFrontend.Render;

public class WorldRenderer : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly ColorShader colorShader;
    private readonly TextureShader textureShader;
    private readonly FrontendSettingsProvider frontendSettingsProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly Camera camera;

    public WorldRenderer(
        IApplicationWindow window,
        ColorShader colorShader,
        TextureShader textureShader,
        Camera camera,
        FrontendSettingsProvider frontendSettingsProvider,
        FrontendStateProvider frontendStateProvider)
    {
        this.window = window;
        this.colorShader = colorShader;
        this.textureShader = textureShader;
        this.camera = camera;
        this.frontendSettingsProvider = frontendSettingsProvider;
        this.frontendStateProvider = frontendStateProvider;

    }

    public unsafe void Draw(float deltaSeconds)
    {
        /*
        var settings = frontendSettingsProvider.Settings;
        var state = frontendStateProvider.State;
        var shader = colorShader;
        var cl = shader.CommandList;
        var gd = graphicsDeviceProvider.GraphicsDevice;

        cl.Begin();
        cl.UpdateBuffer(shader.ProjectionBuffer, 0, camera.ProjectionMatrix);
        cl.UpdateBuffer(shader.ViewBuffer, 0, camera.ViewMatrix);
        cl.UpdateBuffer(shader.WorldBuffer, 0, Matrix4x4.Identity);
        cl.SetFramebuffer(graphicsDeviceProvider.Framebuffer);

        if (!settings.UseBackgroundImage || !state.BackgroundImageAvailable)
            cl.ClearColorTarget(0, new RgbaFloat(settings.BackgroundColor));
        else
            cl.ClearColorTarget(0, RgbaFloat.Black); // Background color bleeds into the background image at the edges slightly so use black

        cl.ClearDepthStencil(1f);
        cl.SetPipeline(shader.Pipeline);

        cl.End();
        gd.SubmitCommands(cl);
        */


        var pass = SDL_BeginGPURenderPass(window.Command, [window.ClearColorTargetInfo], 1, window.ClearDepthStencilTargetInfo);

        // bind pipeline
        SDL_BindGPUGraphicsPipeline(pass, textureShader.SdlPipeline);

        /*
        var vertexBufferBinding = new SDL_GPUBufferBinding()
        {
            buffer = vb,
        };

        // bind buffers
        SDL_BindGPUVertexBuffers(pass, 0, [vertexBufferBinding], 1);

        // set matrix uniform
        {
            var projectionMatrix = camera.ProjectionMatrix;
            var viewMatrix = camera.ViewMatrix;
            var worldMatrix = Matrix4x4.Identity;

            // https://wiki.libsdl.org/SDL3/SDL_CreateGPUShader
            // See Remarks section
            SDL_PushGPUVertexUniformData(window.Command, 0, new nint(&projectionMatrix), (uint)Marshal.SizeOf<Matrix4x4>());
            SDL_PushGPUVertexUniformData(window.Command, 1, new nint(&viewMatrix), (uint)Marshal.SizeOf<Matrix4x4>());
            SDL_PushGPUVertexUniformData(window.Command, 2, new nint(&worldMatrix), (uint)Marshal.SizeOf<Matrix4x4>());

            var glb = lightsProvider.GlobalLightBuffer;

            SDL_PushGPUFragmentUniformData(window.Command, 0, new nint(&glb), 32); // (uint)Marshal.SizeOf<GlobalLightInfoBuffer>()

            var plbSpan = new ReadOnlySpan<PointLightInfoBuffer>(lightsProvider.PointLightBuffer);
            fixed (PointLightInfoBuffer* plb = plbSpan)
            {
                SDL_PushGPUFragmentUniformData(window.Command, 1, (nint)plb, 32 * 32);
            }
        }

        var textureSamplerBinding = new SDL_GPUTextureSamplerBinding
        {
            sampler = window.PointSampler,
            texture = textureResourcesProvider.Texture
        };

        SDL_BindGPUFragmentSamplers(pass, 0, [textureSamplerBinding], 1);

        SDL_DrawGPUPrimitives(
            render_pass: pass,
            num_vertices: (uint)mapVertexBufferProvider.Vertices.Length,
            num_instances: 1,
            first_vertex: 0,
            first_instance: 0
        );
        */

        SDL_EndGPURenderPass(pass);
    }
}

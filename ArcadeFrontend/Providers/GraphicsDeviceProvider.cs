using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace ArcadeFrontend.Providers
{
    public class GraphicsDeviceProvider : IGraphicsDeviceProvider, ILoad
    {
        private readonly Sdl2WindowProvider sdl2WindowProvider;
        private readonly FrontendSettingsProvider frontendSettingsProvider;

        private GraphicsDevice graphicsDevice;
        public GraphicsDevice GraphicsDevice => graphicsDevice;

        private DisposeCollectorResourceFactory resourceFactory;
        public ResourceFactory ResourceFactory => resourceFactory;

        private Framebuffer framebuffer;
        public Framebuffer Framebuffer => framebuffer;

        public GraphicsDeviceProvider(
            Sdl2WindowProvider sdl2WindowProvider,
            FrontendSettingsProvider frontendSettingsProvider)
        {
            this.sdl2WindowProvider = sdl2WindowProvider;
            this.frontendSettingsProvider = frontendSettingsProvider;
        }

        public void Load()
        {
            var settings = frontendSettingsProvider.Settings.Video;

            var backend = (GraphicsBackend)settings.BackendType;

            var options = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: PixelFormat.R32_Float,
                syncToVerticalBlank: settings.VSync,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: true);
#if DEBUG
            options.Debug = true;
#endif
            graphicsDevice = VeldridStartup.CreateGraphicsDevice(sdl2WindowProvider.Window, options, backend);
            resourceFactory = new DisposeCollectorResourceFactory(graphicsDevice.ResourceFactory);
            framebuffer = graphicsDevice.MainSwapchain.Framebuffer;
        }

        public void Unload()
        {
            if (graphicsDevice != null)
            {
                graphicsDevice.Dispose();
                graphicsDevice = null;
            }

            resourceFactory = null;
        }

        public void SetScreenshotFramebuffer(Framebuffer screenshotFramebuffer)
        {
            framebuffer = screenshotFramebuffer;
        }

        public void ResetFramebuffer()
        {
            graphicsDevice.WaitForIdle();
            framebuffer = graphicsDevice.MainSwapchain.Framebuffer;
        }
    }
}

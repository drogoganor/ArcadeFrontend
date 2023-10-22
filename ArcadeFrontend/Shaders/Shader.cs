using ArcadeFrontend.Interfaces;
using Veldrid;

namespace ArcadeFrontend.Shaders
{
    public abstract class Shader : ILoad
    {
        private readonly IGraphicsDeviceProvider graphicsDeviceProvider;

        protected IApplicationWindow Window { get; private set; }
        protected GraphicsDevice GraphicsDevice { get; private set; }
        protected ResourceFactory ResourceFactory { get; private set; }
        protected Swapchain MainSwapchain { get; private set; }

        public Shader(
            IApplicationWindow window,
            IGraphicsDeviceProvider graphicsDeviceProvider)
        {
            Window = window;
            this.graphicsDeviceProvider = graphicsDeviceProvider;
        }

        public virtual void Load()
        {
            GraphicsDevice = graphicsDeviceProvider.GraphicsDevice;
            ResourceFactory = graphicsDeviceProvider.ResourceFactory;
            MainSwapchain = graphicsDeviceProvider.GraphicsDevice.MainSwapchain;
        }

        public virtual void Unload()
        {
            GraphicsDevice = null;
            ResourceFactory = null;
            MainSwapchain = null;
        }
    }
}

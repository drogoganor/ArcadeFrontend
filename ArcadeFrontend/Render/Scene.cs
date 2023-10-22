using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using System.Runtime.Serialization;
using Veldrid;

namespace ArcadeFrontend.Render
{
    /// <summary>
    /// Renders the world geometry
    /// </summary>
    public class Scene : IRenderable
    {
        private readonly IApplicationWindow window;
        private readonly GraphicsDeviceProvider graphicsDeviceProvider;
        private readonly Camera camera;
        private readonly IWorld world;

        public Scene(
            IApplicationWindow window,
            GraphicsDeviceProvider graphicsDeviceProvider,
            Camera camera,
            IWorld world)
        {
            this.window = window;
            this.graphicsDeviceProvider = graphicsDeviceProvider;
            this.camera = camera;
            this.world = world;

            window.Resized += HandleWindowResize;
        }

        public void Draw(float deltaSeconds)
        {
            var gd = graphicsDeviceProvider.GraphicsDevice;

            world.Draw(deltaSeconds);

            gd.SwapBuffers(gd.MainSwapchain);
            gd.WaitForIdle();
        }

        private void HandleWindowResize()
        {
            camera.WindowResized(window.Width, window.Height);
        }
    }
}

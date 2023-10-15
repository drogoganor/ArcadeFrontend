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
        private readonly ScreenshotProvider screenshotProvider;
        private readonly HotKeyProvider hotKeyProvider;

        public Scene(
            IApplicationWindow window,
            GraphicsDeviceProvider graphicsDeviceProvider,
            Camera camera,
            IWorld world,
            ScreenshotProvider screenshotProvider,
            HotKeyProvider hotKeyProvider)
        {
            this.window = window;
            this.graphicsDeviceProvider = graphicsDeviceProvider;
            this.camera = camera;
            this.world = world;
            this.screenshotProvider = screenshotProvider;
            this.hotKeyProvider = hotKeyProvider;

            window.Resized += HandleWindowResize;
        }

        public void Draw(float deltaSeconds)
        {
            // Screenshot
            //var screenshot = InputTracker.GetKeyDown(Key.F12);
            var screenshot = hotKeyProvider.GetInputDown(InputBindingType.TakeScreenshot);
            if (screenshot)
                screenshotProvider.StartScreenshot();

            var gd = graphicsDeviceProvider.GraphicsDevice;

            world.Draw(deltaSeconds);

            if (screenshot)
                screenshotProvider.EndScreenshot();

            gd.SwapBuffers(gd.MainSwapchain);
            gd.WaitForIdle();
        }

        private void HandleWindowResize()
        {
            camera.WindowResized(window.Width, window.Height);
        }
    }
}

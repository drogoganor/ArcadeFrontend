using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;

namespace ArcadeFrontend.Render;

/// <summary>
/// Renders the world geometry
/// </summary>
public class Scene : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly Camera camera;
    private readonly IWorld world;

    public Scene(
        IApplicationWindow window,
        Camera camera,
        IWorld world)
    {
        this.window = window;
        this.camera = camera;
        this.world = world;

        window.Resized += HandleWindowResize;
    }

    public void Draw(float deltaSeconds)
    {
        world.Draw(deltaSeconds);
    }

    public void DrawUI(float deltaSeconds)
    {
        world.DrawUI(deltaSeconds);
    }

    public void PostDraw(float deltaSeconds)
    {
        world.PostDraw(deltaSeconds);

        //var screenshot = hotKeyProvider.GetInputDown(InputBindingType.TakeScreenshot);
        //if (screenshot)
        //    screenshotProvider.TakeScreenshot();
    }

    private void HandleWindowResize()
    {
        camera.WindowResized(window.Width, window.Height);
        //windowResizeProvider.WindowResized();
    }
}

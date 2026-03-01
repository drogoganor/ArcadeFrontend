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

    private void HandleWindowResize()
    {
        camera.WindowResized(window.Width, window.Height);
        //windowResizeProvider.WindowResized();
    }
}

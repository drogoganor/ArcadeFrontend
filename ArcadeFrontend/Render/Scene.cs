using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;

namespace ArcadeFrontend.Render;

/// <summary>
/// Renders the world geometry
/// </summary>
public class Scene : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly Camera camera;
    private readonly IWorld world;
    private readonly Sdl3ImGuiRenderer sdl3ImGuiRenderer;

    public Scene(
        IApplicationWindow window,
        Camera camera,
        IWorld world,
        Sdl3ImGuiRenderer sdl3ImGuiRenderer)
    {
        this.window = window;
        this.camera = camera;
        this.world = world;
        this.sdl3ImGuiRenderer = sdl3ImGuiRenderer;

        window.Resized += HandleWindowResize;
        window.BeforeDispose += HandleBeforeDispose;
    }

    public void Draw(float deltaSeconds)
    {
        sdl3ImGuiRenderer.NewFrame();
        ImGui.NewFrame();

        world.DrawUI(deltaSeconds);

        ImGui.EndFrame();

        world.Draw(deltaSeconds);

        sdl3ImGuiRenderer.Render(window.Command, window.Swapchain, (int)window.Width, (int)window.Height, null);
    }

    private void HandleWindowResize()
    {
        camera.WindowResized(window.Width, window.Height);
        //windowResizeProvider.WindowResized();
    }

    private void HandleBeforeDispose()
    {
        window.Resized -= HandleWindowResize;
        window.BeforeDispose -= HandleBeforeDispose;

        sdl3ImGuiRenderer.Dispose();
    }
}

using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Render;

namespace ArcadeFrontend.Worlds;

/// <summary>
/// Tick and draw game components
/// </summary>
public class ArcadeWorld : IWorld
{
    private readonly IMenuProvider menuProvider;
    private readonly WorldRenderer worldRenderer;
    private readonly InputProvider inputProvider;
    private readonly NextTickActionProvider nextTickActionProvider;

    public ArcadeWorld(
        ILoadProvider loadProvider,
        IMenuProvider menuProvider,
        WorldRenderer worldRenderer,
        InputProvider inputProvider,
        NextTickActionProvider nextTickActionProvider)
    {
        this.menuProvider = menuProvider;
        this.worldRenderer = worldRenderer;
        this.inputProvider = inputProvider;
        this.nextTickActionProvider = nextTickActionProvider;

        loadProvider.Load();
    }

    public void Draw(float deltaSeconds)
    {
        worldRenderer.Draw(deltaSeconds);
    }

    public void DrawUI(float deltaSeconds)
    {
        menuProvider.Draw(deltaSeconds);
    }

    public void Tick(float deltaSeconds)
    {
        nextTickActionProvider.Tick(deltaSeconds);

        inputProvider.Tick(deltaSeconds);
    }
}

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
    //private readonly OrthoSpriteRenderer orthoSpriteRenderer;
    private readonly WorldRenderer worldRenderer;
    private readonly InputProvider inputProvider;

    public ArcadeWorld(
        ILoadProvider loadProvider,
        IMenuProvider menuProvider,
        WorldRenderer worldRenderer,
        InputProvider inputProvider
        //GameSpriteRenderer gameSpriteRenderer,
        //OrthoSpriteRenderer orthoSpriteRenderer
        )
    {
        this.menuProvider = menuProvider;
        this.worldRenderer = worldRenderer;
        this.inputProvider = inputProvider;
        //this.gameSpriteRenderer = gameSpriteRenderer;
        //this.orthoSpriteRenderer = orthoSpriteRenderer;
    }

    public void Draw(float deltaSeconds)
    {
        worldRenderer.Draw(deltaSeconds);

        menuProvider.Draw(deltaSeconds);
    }

    public void Tick(float deltaSeconds)
    {
        inputProvider.Tick(deltaSeconds);
    }
}

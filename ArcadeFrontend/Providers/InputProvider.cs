using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers;

public class InputProvider : ITick, ILoad
{
    private readonly GameCommandsProvider gameCommandsProvider;
    private readonly FrontendStateProvider frontendStateProvider;

    public InputProvider(
        GameCommandsProvider gameCommandsProvider,
        FrontendStateProvider frontendStateProvider)
    {
        this.gameCommandsProvider = gameCommandsProvider;
        this.frontendStateProvider = frontendStateProvider;
    }

    public void Load()
    {
    }

    public void Unload()
    {
    }

    public void Tick(float deltaSeconds)
    {
        var state = frontendStateProvider.State;

        if (state.CurrentView == ViewType.Big)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Left) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadLeft))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Right) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadRight))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.List)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Up) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadUp))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Down) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadDown))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.System)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Up) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadUp))
            {
                gameCommandsProvider.PreviousSystem();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Down) || Sdl3InputTracker.GetButtonDown(SdlButton.DPadDown))
            {
                gameCommandsProvider.NextSystem();
            }
        }

        if (state.CurrentView != ViewType.System)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Enter) || Sdl3InputTracker.GetButtonDown(SdlButton.South))
            {
                gameCommandsProvider.LaunchGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.X) || Sdl3InputTracker.GetButtonDown(SdlButton.North))
            {
                gameCommandsProvider.ToggleView();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Z) || Sdl3InputTracker.GetButtonDown(SdlButton.West))
            {
                gameCommandsProvider.ShowSystems();
            }
        }
        else
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Enter) || Sdl3InputTracker.GetButtonDown(SdlButton.South) ||
                Sdl3InputTracker.GetKeyDown(SdlKey.Z) || Sdl3InputTracker.GetButtonDown(SdlButton.West))
            {
                gameCommandsProvider.ShowGames();
            }
        }
    }
}

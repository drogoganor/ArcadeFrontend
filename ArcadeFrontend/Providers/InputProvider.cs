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
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Left) || XInputTracker.GetButtonDown(XButton.DPadLeft))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Right) || XInputTracker.GetButtonDown(XButton.DPadRight))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.List)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Up) || XInputTracker.GetButtonDown(XButton.DPadUp))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Down) || XInputTracker.GetButtonDown(XButton.DPadDown))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.System)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Up) || XInputTracker.GetButtonDown(XButton.DPadUp))
            {
                gameCommandsProvider.PreviousSystem();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Down) || XInputTracker.GetButtonDown(XButton.DPadDown))
            {
                gameCommandsProvider.NextSystem();
            }
        }

        if (state.CurrentView != ViewType.System)
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Enter) || XInputTracker.GetButtonDown(XButton.A))
            {
                gameCommandsProvider.LaunchGame();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.X) || XInputTracker.GetButtonDown(XButton.Y))
            {
                gameCommandsProvider.ToggleView();
            }
            else if (Sdl3InputTracker.GetKeyDown(SdlKey.Z) || XInputTracker.GetButtonDown(XButton.X))
            {
                gameCommandsProvider.ShowSystems();
            }
        }
        else
        {
            if (Sdl3InputTracker.GetKeyDown(SdlKey.Enter) || XInputTracker.GetButtonDown(XButton.A) ||
                Sdl3InputTracker.GetKeyDown(SdlKey.Z) || XInputTracker.GetButtonDown(XButton.X))
            {
                gameCommandsProvider.ShowGames();
            }
        }
    }
}

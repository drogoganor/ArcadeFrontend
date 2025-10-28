using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using Veldrid;

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
            if (InputTracker.GetKeyDown(Key.Left) || XInputTracker.GetButtonDown(XButton.DPadLeft))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (InputTracker.GetKeyDown(Key.Right) || XInputTracker.GetButtonDown(XButton.DPadRight))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.List)
        {
            if (InputTracker.GetKeyDown(Key.Up) || XInputTracker.GetButtonDown(XButton.DPadUp))
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (InputTracker.GetKeyDown(Key.Down) || XInputTracker.GetButtonDown(XButton.DPadDown))
            {
                gameCommandsProvider.NextGame();
            }
        }
        else if (state.CurrentView == ViewType.System)
        {
            if (InputTracker.GetKeyDown(Key.Up) || XInputTracker.GetButtonDown(XButton.DPadUp))
            {
                gameCommandsProvider.PreviousSystem();
            }
            else if (InputTracker.GetKeyDown(Key.Down) || XInputTracker.GetButtonDown(XButton.DPadDown))
            {
                gameCommandsProvider.NextSystem();
            }
        }

        if (state.CurrentView != ViewType.System)
        {
            if (InputTracker.GetKeyDown(Key.Enter) || XInputTracker.GetButtonDown(XButton.A))
            {
                gameCommandsProvider.LaunchGame();
            }
            else if (InputTracker.GetKeyDown(Key.Z) || XInputTracker.GetButtonDown(XButton.X))
            {
                gameCommandsProvider.ToggleView();
            }
            else if (InputTracker.GetKeyDown(Key.X) || XInputTracker.GetButtonDown(XButton.Y))
            {
                gameCommandsProvider.ShowSystems();
            }
        }
        else
        {
            if (InputTracker.GetKeyDown(Key.Enter) || XInputTracker.GetButtonDown(XButton.A) ||
                InputTracker.GetKeyDown(Key.X) || XInputTracker.GetButtonDown(XButton.Y))
            {
                gameCommandsProvider.ShowGames();
            }
        }
    }
}

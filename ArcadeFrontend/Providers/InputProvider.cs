using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using Veldrid;
using XInput.Wrapper;

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
        //if (X.IsAvailable)
        //{
        //    var gamepad = X.Gamepad_1;

        //    X.StartPolling(gamepad);
        //}
    }

    public void Unload()
    {
        //if (X.IsAvailable)
        //{
        //    X.StopPolling();
        //}
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

        if (InputTracker.GetKeyDown(Key.Enter) || XInputTracker.GetButtonDown(XButton.A))
        {
            gameCommandsProvider.LaunchGame();
        }
        else if (InputTracker.GetKeyDown(Key.Z) || XInputTracker.GetButtonDown(XButton.X))
        {
            gameCommandsProvider.ToggleView();
        }
    }
}

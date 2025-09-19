using ArcadeFrontend.Interfaces;
using Veldrid;
using XInput.Wrapper;

namespace ArcadeFrontend.Providers;

public class InputProvider : ITick, ILoad
{
    private readonly GameCommandsProvider gameCommandsProvider;

    public InputProvider(
        GameCommandsProvider gameCommandsProvider)
    {
        this.gameCommandsProvider = gameCommandsProvider;
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
        if (InputTracker.GetKeyDown(Key.Left) || XInputTracker.GetButtonDown(XButton.DPadLeft))
        {
            gameCommandsProvider.PreviousGame();
        }
        else if (InputTracker.GetKeyDown(Key.Right) || XInputTracker.GetButtonDown(XButton.DPadRight))
        {
            gameCommandsProvider.NextGame();
        }
        else if (InputTracker.GetKeyDown(Key.Enter) || XInputTracker.GetButtonDown(XButton.A))
        {
            gameCommandsProvider.LaunchGame();
        }
    }
}

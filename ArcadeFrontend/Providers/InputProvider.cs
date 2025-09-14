using ArcadeFrontend.Interfaces;
using Veldrid;

namespace ArcadeFrontend.Providers
{
    public class InputProvider : ITick
    {
        private readonly GameCommandsProvider gameCommandsProvider;

        public InputProvider(
            GameCommandsProvider gameCommandsProvider)
        {
            this.gameCommandsProvider = gameCommandsProvider;
        }

        public void Tick(float deltaSeconds)
        {
            //var gamepad = X.Gamepad_1;
            //gamepad.Update();

            if (InputTracker.GetKeyDown(Key.Left)) // || gamepad.Dpad_Left_up)
            {
                gameCommandsProvider.PreviousGame();
            }
            else if (InputTracker.GetKeyDown(Key.Right)) // || gamepad.Dpad_Right_up)
            {
                gameCommandsProvider.NextGame();
            }
            else if (InputTracker.GetKeyDown(Key.Enter)) // || gamepad.A_up)
            {
                gameCommandsProvider.LaunchGame();
            }
        }
    }
}

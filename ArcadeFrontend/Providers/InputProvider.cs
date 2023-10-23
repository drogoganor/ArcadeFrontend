using ArcadeFrontend.Interfaces;
using System.Diagnostics;
using Veldrid;

namespace ArcadeFrontend.Providers
{
    public class InputProvider : ITick
    {
        private readonly FrontendStateProvider frontendStateProvider;
        private readonly GamesFileProvider gamesFileProvider;

        private Process gameProcess;

        public InputProvider(
            FrontendStateProvider frontendStateProvider,
            GamesFileProvider gamesFileProvider)
        {
            this.frontendStateProvider = frontendStateProvider;
            this.gamesFileProvider = gamesFileProvider;
        }

        public void Tick(float deltaSeconds)
        {
            var state = frontendStateProvider.State;

            if (InputTracker.GetKeyDown(Key.Left))
            {
                var newIndex = state.GameIndex - 1;
                if (newIndex < 0)
                    newIndex += gamesFileProvider.Data.Games.Count;

                state.GameIndex = newIndex;
            }
            else if (InputTracker.GetKeyDown(Key.Right))
            {
                var newIndex = (state.GameIndex + 1) % gamesFileProvider.Data.Games.Count;
                state.GameIndex = newIndex;
            }
            else if (InputTracker.GetKeyDown(Key.Enter))
            {
                LaunchGame();
            }
        }

        private void LaunchGame()
        {
            var state = frontendStateProvider.State;
            var currentGame = gamesFileProvider.Data.Games[state.GameIndex];

            var startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(currentGame.Directory, currentGame.ProgramExe),
                WorkingDirectory = currentGame.Directory,
                Arguments = currentGame.Arguments,
            };

            gameProcess = Process.Start(startInfo);
            gameProcess.WaitForExit();
        }
    }
}

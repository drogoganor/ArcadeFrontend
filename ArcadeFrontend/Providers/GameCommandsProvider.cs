using ArcadeFrontend.Enums;
using ArcadeFrontend.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ArcadeFrontend.Providers;

public class GameCommandsProvider
{
    public class MameRomInfo
    {
        public string RomName { get; set; }
        public string Title { get; set; }
    }

    private readonly ILogger<GameCommandsProvider> logger;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly IDbContextFactory<MameDbContext> dbContextFactory;

    private Process gameProcess;

    public GameCommandsProvider(
        ILogger<GameCommandsProvider> logger,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        IDbContextFactory<MameDbContext> dbContextFactory)
    {
        this.logger = logger;
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.dbContextFactory = dbContextFactory;
    }

    public void SetGameIndex(int gameIndex)
    {
        var state = frontendStateProvider.State;

        state.CurrentGameIndex = gameIndex;

        gameScreenshotImagesProvider.UpdateGame();
    }

    public void PreviousGame()
    {
        var state = frontendStateProvider.State;

        var newIndex = state.CurrentGameIndex - 1;
        if (newIndex < 0)
            newIndex += gamesFileProvider.Data.Games.Count;

        SetGameIndex(newIndex);
    }

    public void NextGame()
    {
        var state = frontendStateProvider.State;

        var newIndex = (state.CurrentGameIndex + 1) % gamesFileProvider.Data.Games.Count;

        SetGameIndex(newIndex);
    }

    public void ToggleView()
    {
        var state = frontendStateProvider.State;

        if (state.CurrentView == ViewType.Big)
        {
            state.CurrentView = ViewType.List;
        }
        else if (state.CurrentView == ViewType.List)
        {
            state.CurrentView = ViewType.Big;
        }
    }

    public void LaunchGame()
    {
        var state = frontendStateProvider.State;
        var currentGame = gamesFileProvider.Data.Games[state.CurrentGameIndex];

        var currentSystem = gamesFileProvider.Data.Systems[currentGame.System];

        var arguments = string.Empty;
        if (!string.IsNullOrWhiteSpace(currentSystem.Arguments))
            arguments = $"{currentSystem.Arguments} ";

        arguments += currentGame.Arguments;

        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(currentSystem.Directory, currentSystem.Executable),
            WorkingDirectory = currentSystem.Directory,
            Arguments = arguments,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        gameProcess = new Process
        {
            StartInfo = startInfo
        };

        gameProcess.OutputDataReceived += GameProcess_OutputDataReceived;
        gameProcess.ErrorDataReceived += GameProcess_ErrorDataReceived;

        gameProcess.Start();

        gameProcess.BeginOutputReadLine();
        gameProcess.BeginErrorReadLine();

        gameProcess.WaitForExit();

        gameProcess.CancelOutputRead();
        gameProcess.CancelErrorRead();

        gameProcess.OutputDataReceived -= GameProcess_OutputDataReceived;
        gameProcess.ErrorDataReceived -= GameProcess_ErrorDataReceived;
    }

    private void GameProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
            return;

        logger.LogInformation(e.Data);
    }

    private void GameProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        if (e.Data == null)
            return;

        logger.LogError(e.Data);
    }
}

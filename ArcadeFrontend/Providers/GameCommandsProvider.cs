using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
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
    private readonly IFileSystem fileSystem;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly IDbContextFactory<MameDbContext> dbContextFactory;

    private Process gameProcess;

    public GameCommandsProvider(
        ILogger<GameCommandsProvider> logger,
        IFileSystem fileSystem,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        IDbContextFactory<MameDbContext> dbContextFactory)
    {
        this.logger = logger;
        this.fileSystem = fileSystem;
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.dbContextFactory = dbContextFactory;
    }

    public void SetSystem(string system)
    {
        var state = frontendStateProvider.State;

        state.CurrentSystem = system;
        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];

        var currentGame = currentSystem.Games.FirstOrDefault(x => state.CurrentGame == null || x.Name == state.CurrentGame)
            ?? currentSystem.Games.FirstOrDefault();

        if (currentGame == null)
            return;

        SetGame(currentGame.Name);
    }

    public void SetGame(string game)
    {
        var state = frontendStateProvider.State;

        state.CurrentGame = game;

        gameScreenshotImagesProvider.UpdateGame();
    }

    public void PreviousSystem()
    {
        var state = frontendStateProvider.State;

        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];
        var systemIndex = gamesFileProvider.Data.Values.ToList().IndexOf(currentSystem);

        var newIndex = systemIndex - 1;
        if (newIndex < 0)
            newIndex += gamesFileProvider.Data.Values.Count;

        var newSystem = gamesFileProvider.Data.Keys.ToList()[newIndex];

        SetSystem(newSystem);
    }

    public void NextSystem()
    {
        var state = frontendStateProvider.State;

        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];
        var systemIndex = gamesFileProvider.Data.Values.ToList().IndexOf(currentSystem);

        var newIndex = (systemIndex + 1) % gamesFileProvider.Data.Values.Count;

        var newSystem = gamesFileProvider.Data.Keys.ToList()[newIndex];

        SetSystem(newSystem);
    }

    public void PreviousGame()
    {
        var state = frontendStateProvider.State;
        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];

        var currentGame = currentSystem.Games.First(x => x.Name == state.CurrentGame);
        var gameIndex = currentSystem.Games.IndexOf(currentGame); // TODO: Write an indexer

        var newIndex = gameIndex - 1;
        if (newIndex < 0)
            newIndex += currentSystem.Games.Count;

        var newGame = currentSystem.Games[newIndex];

        SetGame(newGame.Name);
    }

    public void NextGame()
    {
        var state = frontendStateProvider.State;
        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];

        var currentGame = currentSystem.Games.First(x => state.CurrentGame == null || x.Name == state.CurrentGame);
        var gameIndex = currentSystem.Games.IndexOf(currentGame); // TODO: Write an indexer

        var newIndex = (gameIndex + 1) % currentSystem.Games.Count;

        var newGame = currentSystem.Games[newIndex];

        SetGame(newGame.Name);
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

    public void ShowGames()
    {
        var state = frontendStateProvider.State;

        state.CurrentView = ViewType.List;
    }

    public void ShowSystems()
    {
        var state = frontendStateProvider.State;

        state.CurrentView = ViewType.System;
    }

    public void LaunchGame()
    {
        var state = frontendStateProvider.State;
        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];

        var currentGame = currentSystem.Games.First(x => x.Name == state.CurrentGame);

        var arguments = string.Empty;
        if (!string.IsNullOrWhiteSpace(currentSystem.Arguments))
            arguments = $"{currentSystem.Arguments} ";

        arguments += currentGame.Arguments;

        var startInfo = new ProcessStartInfo
        {
            FileName = Path.Combine(fileSystem.DataDirectory, currentSystem.Directory, currentSystem.Executable),
            WorkingDirectory = Path.Combine(fileSystem.DataDirectory, currentSystem.Directory),
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

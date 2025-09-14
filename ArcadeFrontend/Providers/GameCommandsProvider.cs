using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace ArcadeFrontend.Providers;

public class GameCommandsProvider
{
    public class MameRomInfo
    {
        public string RomName { get; set; }
        public string Title { get; set; }
    }

    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly IDbContextFactory<MameDbContext> dbContextFactory;

    private Process gameProcess;

    public GameCommandsProvider(
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        IDbContextFactory<MameDbContext> dbContextFactory)
    {
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.dbContextFactory = dbContextFactory;
    }

    private void SetGameIndex(int gameIndex)
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
        };

        gameProcess = Process.Start(startInfo);
        gameProcess.WaitForExit();
    }

    public void ScanMameGames()
    {
        if (!gamesFileProvider.Data.Systems.TryGetValue(SystemType.Mame, out var mameData))
            return;

        var skipTheseRoms = new HashSet<string>
        {
            "qsound",
            "qsound_hle",
            "neogeo"
        };

        var romsDirectory = Path.Combine(mameData.Directory, "roms");
        var romFiles = Directory.GetFiles(romsDirectory, "*.zip");

        using var mameDbContext = dbContextFactory.CreateDbContext();

        var games = new List<GameData>();
        foreach (var romFile in romFiles)
        {
            var filename = Path.GetFileNameWithoutExtension(romFile);

            if (skipTheseRoms.Contains(filename))
                continue;

            var gameDef = new GameData
            {
                Name = filename,
                Arguments = filename,
                System = SystemType.Mame
            };

            var mameRom = mameDbContext.MameRom.FirstOrDefault(x => x.Name == filename);
            if (mameRom != null)
            {
                gameDef.Name = mameRom.Title;
            }

            games.Add(gameDef);
        }

        var gamesJson = JsonSerializer.Serialize(games);

        var fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";
        File.WriteAllText(fileName, gamesJson);

        using var fileOpener = new Process();

        fileOpener.StartInfo.FileName = "explorer";
        fileOpener.StartInfo.Arguments = "" + fileName + "";
        fileOpener.Start();
    }
}

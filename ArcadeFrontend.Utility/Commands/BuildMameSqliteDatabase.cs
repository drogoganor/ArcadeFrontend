using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Sqlite.Entities;
using ArcadeFrontend.Utility.Options;
using ArcadeFrontend.Utility.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Xml.Linq;

namespace ArcadeFrontend.Utility.Commands;

public class BuildMameSqliteDatabase
{
    private readonly ILogger<BuildMameSqliteDatabase> logger;
    private readonly MameDatabaseUpgrader dbUpgrader;
    private readonly IDbContextFactory<MameDbContext> dbContextFactory;

    public BuildMameSqliteDatabase(
        ILogger<BuildMameSqliteDatabase> logger,
        MameDatabaseUpgrader dbUpgrader,
        IDbContextFactory<MameDbContext> dbContextFactory)
    {
        this.logger = logger;
        this.dbUpgrader = dbUpgrader;
        this.dbContextFactory = dbContextFactory;
    }

    public async Task Build(BuildMameSqliteDatabaseOptions options)
    {
        try
        {
            var result = dbUpgrader.Upgrade();
            if (!result)
                return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error upgrading sqlite database.");
            return;
        }

        await BuildMameRomInfoFromXml(options);
    }

    private async Task BuildMameRomInfoFromXml(BuildMameSqliteDatabaseOptions options)
    {
        var mameRomXml = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "mame_718_0.279.xml");

        logger.LogInformation("Reading mame dat xml info from: {path}", mameRomXml);

        var xmlDoc = XDocument.Load(mameRomXml);
        var machineNodes = xmlDoc
            .Descendants("mame")
            .First()
            .Descendants("machine");

        logger.LogInformation("Creating mame sqlite db context...");

        var dbContext = await dbContextFactory.CreateDbContextAsync();

        var allRoms = dbContext.MameRom.ToList();
        dbContext.MameRom.RemoveRange(allRoms);

        logger.LogInformation("Building mame rom info...");

        foreach (var machineNode in machineNodes)
        {
            var romTitle = machineNode.Elements("description").First().Value;

            var indexOfBracket = romTitle.IndexOf('(');
            var substringLength = indexOfBracket >= 0 ? indexOfBracket - 1 : romTitle.Length;
            var clippedTitle = romTitle.Substring(0, substringLength);

            var rom = new MameRom
            {
                Name = machineNode.Attribute("name").Value,
                Title = clippedTitle
            };

            dbContext.MameRom.Add(rom);
        }

        logger.LogInformation("Saving database...");

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing mame roms to sqlite database.");
            throw;
        }

        await dbContext.DisposeAsync();

        // Now copy newest database to source folder
        var builtDbPath = Path.Combine(Environment.CurrentDirectory, "Content\\mame.db");
        var targetDbPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\ArcadeFrontend\\Content\\mame.db");

        logger.LogInformation("Copying mame db from '{source}' to '{dest}'", builtDbPath, targetDbPath);

        try
        {
            //File.Move(builtDbPath, targetDbPath, true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Couldn't move mame db.");
            throw;
        }


        var mameDirectory = "C:\\Users\\dwils\\AppData\\Local\\ArcadeFrontend\\data\\mame";

        var skipTheseRoms = new HashSet<string>
        {
            "qsound",
            "qsound_hle",
            "neogeo",
            "pgm",
            "namcoc69",
            "namcoc70",
            "namcoc75",
            "naomi",
            "jvs13551",
            "mie"
        };

        var romsDirectory = Path.Combine(mameDirectory, "roms");
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
                System = "Mame"
            };

            var mameRom = mameDbContext.MameRom.FirstOrDefault(x => x.Name == filename);
            if (mameRom != null)
            {
                gameDef.Name = mameRom.Title;
            }

            games.Add(gameDef);
        }

        var gamesJson = JsonSerializer.Serialize(games, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        var fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".txt";

        logger.LogInformation("Writing detected games.json info to temp file '{file}'", fileName);

        File.WriteAllText(fileName, gamesJson);

        var startInfo = new ProcessStartInfo
        {
            FileName = "explorer",
            Arguments = "" + fileName + ""
        };

        using var fileOpener = new Process();

        fileOpener.StartInfo = startInfo;
        fileOpener.Start();
        fileOpener.WaitForExit();
    }
}
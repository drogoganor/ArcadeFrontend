using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Sqlite.Entities;
using ArcadeFrontend.Utility.Options;
using ArcadeFrontend.Utility.Sqlite;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace ArcadeFrontend.Utility.Commands;

public class BuildMameSqliteDatabase
{
    private readonly ILogger<BuildMameSqliteDatabase> logger;
    private readonly MameDatabaseUpgrader dbUpgrader;
    private readonly MameDbContext mameDbContext;

    public BuildMameSqliteDatabase(
        ILogger<BuildMameSqliteDatabase> logger,
        MameDatabaseUpgrader dbUpgrader,
        MameDbContext mameDbContext)
    {
        this.logger = logger;
        this.dbUpgrader = dbUpgrader;
        this.mameDbContext = mameDbContext;
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

        var xmlDoc = XDocument.Load(mameRomXml);
        var machineNodes = xmlDoc
            .Descendants("mame")
            .First()
            .Descendants("machine");

        var allRoms = mameDbContext.MameRom.ToList();
        mameDbContext.MameRom.RemoveRange(allRoms);

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

            mameDbContext.MameRom.Add(rom);
        }

        try
        {
            await mameDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error writing mame roms to sqlite database.");
            throw;
        }

        // Now copy newest database to source folder
        var builtDbPath = Path.Combine(Environment.CurrentDirectory, "Content\\mame.db");
        var targetDbPath = Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\ArcadeFrontend\\Content\\mame.db");

        File.Copy(builtDbPath, targetDbPath, true);
        File.Delete(builtDbPath);
    }
}
using DbUp;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ArcadeFrontend.Utility.Sqlite;

public class MameDatabaseUpgrader
{
    private readonly ILogger<MameDatabaseUpgrader> logger;

    public MameDatabaseUpgrader(
        ILogger<MameDatabaseUpgrader> logger)
    {
        this.logger = logger;
    }

    public bool Upgrade()
    {
        var connectionString = "Data Source=Content\\mame.db;Pooling=false";

        var upgrader = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            //.LogToConsole()
            .LogTo(logger)
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            logger.LogError(result.Error, "Couldn't write to sqlite database.");
            return false;
        }

        logger.LogInformation("Database upgrade performed OK.");
        return true;
    }
}

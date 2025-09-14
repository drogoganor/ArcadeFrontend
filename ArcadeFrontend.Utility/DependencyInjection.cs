using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Utility.Commands;
using ArcadeFrontend.Utility.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ArcadeFrontend.Utility;

public static class DependencyInjection
{
    public static IHostBuilder BuildUtility(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<BuildMameSqliteDatabase>();
            services.AddSingleton<MameDatabaseUpgrader>();

            services.AddLogger();
            services.AddMameDb();
        });

        return builder;
    }
}

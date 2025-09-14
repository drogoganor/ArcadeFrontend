using ArcadeFrontend.Data;
using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Utility.Commands;
using ArcadeFrontend.Utility.Sqlite;
using Autofac;

namespace ArcadeFrontend.Utility;

public static class DependencyInjection
{
    public static ContainerBuilder BuildUtility(this ContainerBuilder builder)
    {
        builder
            .RegisterType<FileSystem>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        
        builder
            .RegisterType<BuildMameSqliteDatabase>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        
        builder
            .RegisterType<MameDatabaseUpgrader>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();
        
        builder
            .RegisterType<MameDbContext>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        return builder;
    }
}

using ArcadeFrontend;
using ArcadeFrontend.Utility;
using ArcadeFrontend.Utility.Commands;
using ArcadeFrontend.Utility.Options;
using Autofac;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var services = new ServiceCollection();
services.AddLogging();

var builder = new ContainerBuilder();
var container = builder
    .AddLogging()
    .BuildUtility()
    .Build();

var logger = container.Resolve<ILogger>();

if (args.Length == 0)
{
    logger.Error("Please specify a command");
}
else
{
    var command = args[0];
    if (command == "mame-db")
    {
        await Parser.Default
            .ParseArguments<BuildMameSqliteDatabaseOptions>(args.Skip(1).ToArray())
            .WithParsedAsync(async o =>
            {
                var command = container.Resolve<BuildMameSqliteDatabase>();
                await command.Build(o);
            });
    }
}

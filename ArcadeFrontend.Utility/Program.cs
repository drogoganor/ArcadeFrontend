using ArcadeFrontend.Utility;
using ArcadeFrontend.Utility.Commands;
using ArcadeFrontend.Utility.Options;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var services = DependencyInjection.BuildUtility(args);
var host = services.Build();

var logger = host.Services.GetRequiredService<ILogger>();

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
                var command = host.Services.GetRequiredService<BuildMameSqliteDatabase>();
                await command.Build(o);
            });
    }
}

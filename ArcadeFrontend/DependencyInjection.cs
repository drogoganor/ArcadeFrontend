using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Menus;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Render;
using ArcadeFrontend.Shaders;
using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Worlds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ArcadeFrontend;

public static class DependencyInjection
{
    public static IHostBuilder Build(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<IGraphicsDeviceProvider, GraphicsDeviceProvider>();
            services.AddSingleton<Sdl2WindowProvider>();
            services.AddSingleton<IApplicationWindow, VeldridStartupWindow>();
            services.AddSingleton<ImGuiProvider>();
            services.AddSingleton<ImGuiFontProvider>();
            services.AddSingleton<ImGuiColorsProvider>();
            services.AddSingleton<IMenuProvider, AppMenuProvider>();
            services.AddSingleton<IWorld, ArcadeWorld>();
            services.AddSingleton<ILoadProvider, LoadProvider>();
            services.AddSingleton<IAppClient, AppClient>();
            services.AddSingleton<ArcadeUI>();
            services.AddSingleton<ConfirmDialog>();
            services.AddSingleton<NextTickActionProvider>();
            services.AddSingleton<FrontendSettingsProvider>();
            services.AddSingleton<FileLoadProvider>();
            services.AddSingleton<ManifestProvider>();
            services.AddSingleton<Scene>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<Camera>();
            services.AddSingleton<ColorShader>();
            services.AddSingleton<TextureShader>();
            services.AddSingleton<WorldRenderer>();
            services.AddSingleton<OptionsDialog>();
            services.AddSingleton<GamesFileProvider>();
            services.AddSingleton<BigViewComponent>();
            services.AddSingleton<FrontendStateProvider>();
            services.AddSingleton<InputProvider>();
            services.AddSingleton<BackgroundImagesProvider>();
            services.AddSingleton<GameCommandsProvider>();
            services.AddSingleton<GameScreenshotImagesProvider>();
            services.AddSingleton<ControllerImagesProvider>();
            services.AddSingleton<ListViewComponent>();
            services.AddSingleton<GamePanelComponent>();
            services.AddSingleton<KeyboardImagesProvider>();
            services.AddSingleton<SystemViewComponent>();
            services.AddSingleton<HeaderComponent>();
            services.AddSingleton<FooterComponent>();

            services.AddLogger();
            services.AddMameDb();
        });

        return builder;
    }

    public static IServiceCollection AddMameDb(this IServiceCollection services)
    {
        services.AddDbContextFactory<MameDbContext>();
        return services;
    }

    public static IServiceCollection AddLogger(this IServiceCollection services, string logFileName = "log")
    {
        var localAppDataDirectory = Environment.GetEnvironmentVariable("LocalAppData");
        var logFilePath = Path.Combine(localAppDataDirectory, $@"ArcadeFrontend\logs\{logFileName}.txt");

        // Delete last log file
        if (File.Exists(logFilePath))
        {
            try
            {
                File.Delete(logFilePath);
            }
            catch (Exception ex)
            {
                // Suppress
                Console.WriteLine($"Couldn't delete existing log file '{logFilePath}': {ex.Message}");
            }
        }

        var log = new LoggerConfiguration()
            .MinimumLevel.Debug()
            //.WriteTo.Console()
            .WriteTo.File(logFilePath)
            .CreateLogger();

        Log.Logger = log;

        services.AddSingleton<ILogger>(log);

        services.AddLogging(loggingBuilder =>
            loggingBuilder.AddSerilog(log, dispose: true));

        return services;
    }
}

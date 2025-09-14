using ArcadeFrontend.Data;
using ArcadeFrontend.Menus;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Render;
using ArcadeFrontend.Shaders;
using ArcadeFrontend.Sqlite;
using ArcadeFrontend.Worlds;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ArcadeFrontend;

public static class DependencyInjection
{
    public static ContainerBuilder AddLogging(this ContainerBuilder builder, string logFileName = "log")
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
            .WriteTo.Console()
            .WriteTo.File(logFilePath)
            .CreateLogger();

        Log.Logger = log;

        builder
            .RegisterInstance(log)
            .AsSelf()
            .AsImplementedInterfaces();

        builder.Register(handler => LoggerFactory.Create(configure =>
        {
            configure.Services.AddLogging();
        }));

        builder.RegisterGeneric(typeof(Logger<>))
            .As(typeof(ILogger<>))
            .SingleInstance();

        return builder;
    }

    public static ContainerBuilder Build()
    {
        var builder = new ContainerBuilder();

        builder
            .RegisterType<GraphicsDeviceProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<Sdl2WindowProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<VeldridStartupWindow>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ImGuiProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ImGuiFontProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ImGuiColorsProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<AppMenuProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ArcadeWorld>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<LoadProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<AppClient>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ArcadeUI>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ConfirmDialog>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<NextTickActionProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<FrontendSettingsProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<FileLoadProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ManifestProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<Scene>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<FileSystem>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<Camera>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<ColorShader>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<TextureShader>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<WorldRenderer>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<OptionsDialog>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<GamesFileProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<GamePickerComponent>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<FrontendStateProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<InputProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<BackgroundImagesProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<GameCommandsProvider>()
            .AsSelf()
            .AsImplementedInterfaces()
            .SingleInstance();

        builder
            .RegisterType<GameScreenshotImagesProvider>()
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

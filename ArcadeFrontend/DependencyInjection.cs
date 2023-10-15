using ArcadeFrontend.Menus;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Worlds;
using Autofac;

namespace ArcadeFrontend
{
    public static class DependencyInjection
    {
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

            return builder;
        }
    }
}

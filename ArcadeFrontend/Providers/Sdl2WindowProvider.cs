using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace ArcadeFrontend.Providers
{
    public class Sdl2WindowProvider : ILoad
    {
        private readonly FrontendSettingsProvider settingsProvider;
        private readonly ManifestProvider manifestProvider;

        private Sdl2Window window;
        public Sdl2Window Window => window;

        public Sdl2WindowProvider(
            FrontendSettingsProvider settingsProvider,
            ManifestProvider manifestProvider)
        {
            this.manifestProvider = manifestProvider;
            this.settingsProvider = settingsProvider;
        }

        public void Load()
        {
            var modInfo = manifestProvider.ManifestFile;
            var settings = settingsProvider.Settings.Video;

            var windowSize = settings.ScreenType == ScreenType.Windowed ? settings.WindowedSize : settings.FullscreenSize;
            var initialState = settings.ScreenType switch
            {
                ScreenType.Windowed => WindowState.Normal,
                ScreenType.FullscreenWindowed => WindowState.BorderlessFullScreen,
                ScreenType.Fullscreen => WindowState.BorderlessFullScreen,
                _ => WindowState.Normal,
            };

            var windowCreateInfo = new WindowCreateInfo
            {
                X = 100,
                Y = 100,
                WindowInitialState = initialState,
                WindowWidth = (int)windowSize.X,
                WindowHeight = (int)windowSize.Y,
                WindowTitle = modInfo.Name,
            };

            window = VeldridStartup.CreateWindow(ref windowCreateInfo);
        }

        public void Unload()
        {
            if (window == null)
                return;

            window.Close();
            window = null;
        }
    }
}

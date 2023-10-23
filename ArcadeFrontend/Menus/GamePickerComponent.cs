using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus
{
    public class GamePickerComponent : IRenderable
    {
        private readonly IApplicationWindow window;
        private readonly GamesFileProvider gamesFileProvider;
        private readonly ImGuiFontProvider imGuiFontProvider;
        private readonly FrontendStateProvider frontendStateProvider;
        private readonly FrontendSettingsProvider frontendSettingsProvider;
        private readonly BackgroundImagesProvider backgroundImagesProvider;

        public GamePickerComponent(
            IApplicationWindow window,
            GamesFileProvider gamesFileProvider,
            ImGuiFontProvider imGuiFontProvider,
            FrontendStateProvider frontendStateProvider,
            FrontendSettingsProvider frontendSettingsProvider,
            BackgroundImagesProvider backgroundImagesProvider)
        {
            this.window = window;
            this.gamesFileProvider = gamesFileProvider;
            this.imGuiFontProvider = imGuiFontProvider;
            this.frontendStateProvider = frontendStateProvider;
            this.frontendSettingsProvider = frontendSettingsProvider;
            this.backgroundImagesProvider = backgroundImagesProvider;
        }

        public void Draw(float deltaSeconds)
        {
            var windowSize = new Vector2(window.Width, window.Height);
            var settings = frontendSettingsProvider.Settings;
            var imageIndex = settings.BackgroundIndex;
            var imageInfo = backgroundImagesProvider.ImGuiImages[(uint)imageIndex];

            var state = frontendStateProvider.State;
            var currentGame = gamesFileProvider.Data.Games[state.GameIndex];

            imGuiFontProvider.PushFont(FontSize.Medium);

            var padding = 5;
            var dialogSize = new Vector2(800, 600);
            var dialogPosition = (windowSize - dialogSize) / 2;

            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(windowSize);
            if (ImGui.Begin("Background",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoMouseInputs |
                ImGuiWindowFlags.NoFocusOnAppearing))
            {
                ImGui.Image(imageInfo.IntPtr, windowSize);

                ImGui.End();
            }

            ImGui.SetNextWindowPos(dialogPosition);
            ImGui.SetNextWindowSize(dialogSize);
            if (ImGui.Begin("Game Display",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoNavFocus |
                //ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoMouseInputs |
                ImGuiWindowFlags.NoFocusOnAppearing
                ))
            {
                imGuiFontProvider.PushFont(FontSize.Large);
                HorizontallyCenteredText(currentGame.Name, dialogSize.X);
                imGuiFontProvider.PopFont();
                HorizontallyCenteredText(currentGame.Platform, dialogSize.X);

                ImGui.End();
            }


            imGuiFontProvider.PopFont();
        }

        private void DrawBackground()
        {

        }

        private static void HorizontallyCenteredText(string text, float width)
        {
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((width - textWidth) * 0.5f);
            ImGui.Text(text);
        }
    }
}

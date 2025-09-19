using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;
using Veldrid;
using Vortice.Mathematics;

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
        private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
        private readonly ControllerImagesProvider controllerImagesProvider;

        public GamePickerComponent(
            IApplicationWindow window,
            GamesFileProvider gamesFileProvider,
            ImGuiFontProvider imGuiFontProvider,
            FrontendStateProvider frontendStateProvider,
            FrontendSettingsProvider frontendSettingsProvider,
            BackgroundImagesProvider backgroundImagesProvider,
            GameScreenshotImagesProvider gameScreenshotImagesProvider,
            ControllerImagesProvider controllerImagesProvider)
        {
            this.window = window;
            this.gamesFileProvider = gamesFileProvider;
            this.imGuiFontProvider = imGuiFontProvider;
            this.frontendStateProvider = frontendStateProvider;
            this.frontendSettingsProvider = frontendSettingsProvider;
            this.backgroundImagesProvider = backgroundImagesProvider;
            this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
            this.controllerImagesProvider = controllerImagesProvider;
        }

        public void Draw(float deltaSeconds)
        {
            var windowSize = new Vector2(window.Width, window.Height);
            var settings = frontendSettingsProvider.Settings;
            var backgroundImageAvailable = backgroundImagesProvider.ImGuiImages.TryGetValue(settings.BackgroundImage, out var backgroundImage);

            var state = frontendStateProvider.State;

            var currentGame = gamesFileProvider.Data.Games[state.CurrentGameIndex];

            var currentSystem = gamesFileProvider.Data.Systems[currentGame.System];

            imGuiFontProvider.PushFont(FontSize.Medium);

            var padding = 5;
            var dialogSize = new Vector2(800, 600);
            var screenshotSize = new Vector2(640, 480);
            var dialogPosition = (windowSize - dialogSize) / 2;

            var controllerImageSize = new Vector2(96);

            var borderSpaceWidth = 128;

            if (backgroundImageAvailable)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
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
                    ImGui.SetCursorPos(Vector2.Zero);

                    if (backgroundImageAvailable)
                        ImGui.Image(backgroundImage.IntPtr, windowSize);

                    var leftButtonPosition = new Vector2(borderSpaceWidth - controllerImageSize.X, (windowSize.Y - controllerImageSize.Y) / 2f);
                    var rightButtonPosition = new Vector2(windowSize.X - borderSpaceWidth, (windowSize.Y - controllerImageSize.Y) / 2f);

                    if (controllerImagesProvider.ImGuiImages.TryGetValue("dpleft", out var leftImage))
                    {
                        ImGui.SetCursorPos(leftButtonPosition);
                        ImGui.Image(leftImage.IntPtr, controllerImageSize);
                    }

                    if (controllerImagesProvider.ImGuiImages.TryGetValue("dpright", out var rightImage))
                    {
                        ImGui.SetCursorPos(rightButtonPosition);
                        ImGui.Image(rightImage.IntPtr, controllerImageSize);
                    }

                    ImGui.End();
                }

                ImGui.PopStyleVar();
            }
            else
            {
                state.BackgroundImageAvailable = false;
            }


            ImGui.SetNextWindowPos(dialogPosition);
            ImGui.SetNextWindowSize(dialogSize);
            if (ImGui.Begin("Game Display",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse |
                //ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoNavFocus |
                //ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoMouseInputs
                //ImGuiWindowFlags.NoFocusOnAppearing
                ))
            {
                imGuiFontProvider.PushFont(FontSize.Large);
                HorizontallyCenteredColoredText(currentGame.Name, dialogSize.X, RgbaFloat.Green.ToVector4());
                imGuiFontProvider.PopFont();
                HorizontallyCenteredText(currentSystem.Name, dialogSize.X);

                var firstScreenshot = gameScreenshotImagesProvider.ImGuiImages.FirstOrDefault();
                if (firstScreenshot != null)
                {
                    ImGui.SetCursorPosX((dialogSize.X - screenshotSize.X) / 2f);
                    ImGui.Image(firstScreenshot.IntPtr, screenshotSize);
                }

                ImGui.End();
            }

            imGuiFontProvider.PopFont();
        }

        private static void HorizontallyCenteredText(string text, float width)
        {
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((width - textWidth) * 0.5f);
            ImGui.Text(text);
        }

        private static void HorizontallyCenteredColoredText(string text, float width, Vector4 color)
        {
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((width - textWidth) * 0.5f);
            ImGui.TextColored(color, text);
        }
    }
}

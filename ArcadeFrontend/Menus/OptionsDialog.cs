using ImGuiNET;
using System.Numerics;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Enums;

namespace ArcadeFrontend.Menus
{
    public class OptionsDialog : IRenderable
    {
        public bool IsVisible => isVisible;

        private readonly IApplicationWindow window;
        private readonly ImGuiProvider imGuiProvider;
        private readonly ImGuiFontProvider imGuiFontProvider;
        private readonly IFileSystem fileSystem;
        private readonly FrontendSettingsProvider frontendSettingsProvider;

        private bool isVisible;

        private SettingsFile Options => frontendSettingsProvider.Settings;

        private Vector4 backgroundColor;

        public OptionsDialog(
            IApplicationWindow window,
            ImGuiProvider imGuiProvider,
            ImGuiFontProvider imGuiFontProvider,
            IFileSystem fileSystem,
            FrontendSettingsProvider frontendSettingsProvider)
        {
            this.window = window;
            this.imGuiProvider = imGuiProvider;
            this.imGuiFontProvider = imGuiFontProvider;
            this.fileSystem = fileSystem;
            this.frontendSettingsProvider = frontendSettingsProvider;
        }

        public void Show()
        {
            backgroundColor = Options.BackgroundColor;

            isVisible = true;
        }

        private void HandleResult(bool result)
        {
            if (result)
            {
                Options.BackgroundColor = backgroundColor;

                frontendSettingsProvider.SaveSettings();
            }

            isVisible = false;
        }

        public void Draw(float deltaSeconds)
        {
            if (!isVisible) return;

            var fullScreenSize = new Vector2(window.Width, window.Height);

            var dialogSize = new Vector2(510, 326);
            var dialogPosition = (fullScreenSize - dialogSize) / 2;

            imGuiFontProvider.PushFont(FontSize.Medium);

            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(fullScreenSize);
            if (ImGui.Begin("",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize))
            {
                ImGui.SetNextWindowPos(dialogPosition);
                ImGui.SetNextWindowSize(dialogSize);

                if (ImGui.Begin("Arcade Frontend Options",
                    ImGuiWindowFlags.NoCollapse |
                    ImGuiWindowFlags.NoMove |
                    ImGuiWindowFlags.NoResize))
                {
                    imGuiFontProvider.PushFont(FontSize.Small);

                    ImGui.ColorEdit4("Background Color", ref backgroundColor, ImGuiColorEditFlags.AlphaBar);

                    imGuiFontProvider.PopFont();
                    imGuiFontProvider.PushFont(FontSize.Medium);

                    var controlButtonSize = new Vector2(100, 25);

                    ImGui.BeginTable("SelectionBar", 3);
                    ImGui.TableSetupColumn("Selection", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("OK", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Cancel", ImGuiTableColumnFlags.WidthFixed);

                    ImGui.TableNextColumn();
                    ImGui.TableNextColumn();

                    var isValid = true;

                    if (!isValid) ImGui.BeginDisabled();

                    if (ImGui.Button("OK", controlButtonSize))
                    {
                        HandleResult(true);
                    }

                    if (!isValid) ImGui.EndDisabled();

                    ImGui.TableNextColumn();

                    if (ImGui.Button("Cancel", controlButtonSize))
                    {
                        HandleResult(false);
                    }

                    ImGui.EndTable();

                    ImGui.End();

                    imGuiFontProvider.PopFont();
                }

                ImGui.End();
            }
        }
    }
}

using ImGuiNET;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;

namespace ArcadeFrontend.Providers
{
    /// <summary>
    /// Convenience class for switching font sizes in ImGui
    /// Initially created to exclude font size commands in Vulkan which are throwing an exception.
    /// </summary>
    public class ImGuiFontProvider
    {
        private readonly IFileSystem fileSystem;
        private readonly ModManifestProvider modManifestProvider;
        private readonly ImGuiProvider imGuiProvider;
        private readonly GameSettingsProvider gameSettingsProvider;

        // HACK: If we started as Vulkan we can't switch to D3D11 with fonts unless the whole app is restarted.
        private bool wasEverVulkan = false;

        private BackendType backendType => gameSettingsProvider.Settings.Video.BackendType;

        public ImGuiFontProvider(
            IFileSystem fileSystem,
            ImGuiProvider imGuiProvider,
            ModManifestProvider modManifestProvider,
            GameSettingsProvider gameSettingsProvider)
        {
            this.fileSystem = fileSystem;
            this.imGuiProvider = imGuiProvider;
            this.modManifestProvider = modManifestProvider;
            this.gameSettingsProvider = gameSettingsProvider;

            if (backendType == BackendType.Vulkan)
            {
                wasEverVulkan = true;
            }
        }

        public void PushFont(FontSize fontSize)
        {
            if (backendType == BackendType.Vulkan)
            {
                wasEverVulkan = true;
            }
            else if (!wasEverVulkan)
            {
                ImGui.PushFont(imGuiProvider.Fonts[fontSize]);
            }
        }

        public void PopFont()
        {
            if (backendType == BackendType.Vulkan)
            {
                wasEverVulkan = true;
            }
            else if (!wasEverVulkan)
            {
                ImGui.PopFont();
            }
        }
    }
}

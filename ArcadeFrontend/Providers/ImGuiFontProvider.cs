using ImGuiNET;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;

namespace ArcadeFrontend.Providers;

/// <summary>
/// Convenience class for switching font sizes in ImGui
/// Initially created to exclude font size commands in Vulkan which are throwing an exception.
/// </summary>
public class ImGuiFontProvider
{
    private readonly IFileSystem fileSystem;
    private readonly ManifestProvider manifestProvider;
    private readonly ImGuiProvider imGuiProvider;
    private readonly FrontendSettingsProvider frontendSettingsProvider;

    // HACK: If we started as Vulkan we can't switch to D3D11 with fonts unless the whole app is restarted.
    private bool wasEverVulkan = false;

    private BackendType backendType => frontendSettingsProvider.Settings.Video.BackendType;

    public ImGuiFontProvider(
        IFileSystem fileSystem,
        ImGuiProvider imGuiProvider,
        ManifestProvider manifestProvider,
        FrontendSettingsProvider frontendSettingsProvider)
    {
        this.fileSystem = fileSystem;
        this.imGuiProvider = imGuiProvider;
        this.manifestProvider = manifestProvider;
        this.frontendSettingsProvider = frontendSettingsProvider;

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

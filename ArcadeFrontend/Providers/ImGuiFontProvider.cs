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
    private readonly IApplicationWindow window;
    private readonly IFileSystem fileSystem;
    private readonly ManifestProvider manifestProvider;
    private readonly FrontendSettingsProvider frontendSettingsProvider;

    private BackendType backendType => frontendSettingsProvider.Settings.Video.BackendType;

    public ImGuiFontProvider(
        IApplicationWindow window,
        IFileSystem fileSystem,
        ManifestProvider manifestProvider,
        FrontendSettingsProvider frontendSettingsProvider)
    {
        this.window = window;
        this.fileSystem = fileSystem;
        this.manifestProvider = manifestProvider;
        this.frontendSettingsProvider = frontendSettingsProvider;
    }

    public void PushFont(FontSize fontSize)
    {
        ImGui.PushFont(window.ImGuiRenderer.Fonts[fontSize]);
    }

    public void PopFont()
    {
        // TODO: We're calling exit game from the render thread and it throws an exception here
        // Highest-level exit game call needs to be called from a next tick action instead
        ImGui.PopFont();
    }
}

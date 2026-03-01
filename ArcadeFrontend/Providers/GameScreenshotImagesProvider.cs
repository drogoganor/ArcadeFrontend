using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using static SDL3.SDL;
using static ArcadeFrontend.Shaders.ShaderCommon;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering images to render using ImGui
/// </summary>
public class GameScreenshotImagesProvider
{
    private readonly IApplicationWindow window;
    private readonly IFileSystem fileSystem;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;

    public List<ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public GameScreenshotImagesProvider(
        IApplicationWindow window,
        IFileSystem fileSystem,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider)
    {
        this.window = window;
        this.fileSystem = fileSystem;
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
    }

    public void UpdateGame()
    {
        Unload();
        Load();
    }

    public void Load()
    {
        var state = frontendStateProvider.State;
        var currentSystem = gamesFileProvider.Data[state.CurrentSystem];

        var currentGame = currentSystem.Games.First(x => state.CurrentGame == null || x.Name == state.CurrentGame);

        // Only mame for now
        if (currentGame.System != "Mame")
            return;

        var snapDirectory = Path.Combine(currentSystem.Directory, "snap", currentGame.Arguments);

        if (!Directory.Exists(snapDirectory))
            return;

        var imageFiles = Directory.GetFiles(snapDirectory, "*.png");

        for (uint i = 0; i < imageFiles.Length; i++)
        {
            var backgroundFile = imageFiles[i];

            var texture = LoadImage(window, backgroundFile, 4, out var imageWidth, out var imageHeight);
            var imageSize = new Vector2(imageWidth, imageHeight);

            ImGuiImages.Add(new ImGuiImageInfo
            {
                PixelSize = imageSize,
                IntPtr = texture
            });
        }
    }

    public void Unload()
    {
        foreach (var img in ImGuiImages)
        {
            SDL_DestroyTexture(img.IntPtr);
        }

        ImGuiImages.Clear();
    }
}

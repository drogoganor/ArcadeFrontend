using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using Veldrid.ImageSharp;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering images to render using ImGui
/// </summary>
public class GameScreenshotImagesProvider
{
    private readonly IFileSystem fileSystem;
    private readonly IGraphicsDeviceProvider graphicsDeviceProvider;
    private readonly ImGuiProvider imGuiProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;

    public List<ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public GameScreenshotImagesProvider(
        IFileSystem fileSystem,
        IGraphicsDeviceProvider graphicsDeviceProvider,
        ImGuiProvider imGuiProvider,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider)
    {
        this.graphicsDeviceProvider = graphicsDeviceProvider;
        this.fileSystem = fileSystem;
        this.imGuiProvider = imGuiProvider;
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
        var gd = graphicsDeviceProvider.GraphicsDevice;

        var state = frontendStateProvider.State;
        var currentGame = gamesFileProvider.Data.Games[state.CurrentGameIndex];

        var currentSystem = gamesFileProvider.Data.Systems[currentGame.System];

        // Only mame for now
        if (currentGame.System != SystemType.Mame)
            return;

        var snapDirectory = Path.Combine(currentSystem.Directory, "snap", currentGame.Arguments);

        if (!Directory.Exists(snapDirectory))
            return;

        var imageFiles = Directory.GetFiles(snapDirectory, "*.png");

        for (uint i = 0; i < imageFiles.Length; i++)
        {
            var backgroundFile = imageFiles[i];
            var image = new ImageSharpTexture(backgroundFile);
            var deviceImage = image.CreateDeviceTexture(gd, gd.ResourceFactory);

            var imageSize = new Vector2(image.Width, image.Height);
            var view = gd.ResourceFactory.CreateTextureView(deviceImage);
            var ptr = imGuiProvider.ImGuiRenderer.GetOrCreateImGuiBinding(gd.ResourceFactory, view);

            ImGuiImages.Add(new ImGuiImageInfo
            {
                PixelSize = imageSize,
                IntPtr = ptr,
                Texture = deviceImage,
                TextureView = view,
            });
        }
    }

    public void Unload()
    {
        foreach (var img in ImGuiImages)
        {
            imGuiProvider.ImGuiRenderer.RemoveImGuiBinding(img.TextureView);
            img.TextureView.Dispose();
            img.Texture.Dispose();
        }

        ImGuiImages.Clear();
    }
}

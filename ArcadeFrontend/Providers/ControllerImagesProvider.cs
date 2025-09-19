using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using Veldrid.ImageSharp;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering images to render using ImGui
/// </summary>
public class ControllerImagesProvider
{
    private readonly IFileSystem fileSystem;
    private readonly IGraphicsDeviceProvider graphicsDeviceProvider;
    private readonly ImGuiProvider imGuiProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;

    public Dictionary<string, ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public ControllerImagesProvider(
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

        var imagesDirectory = Path.Combine(Environment.CurrentDirectory, "Content", "images");

        if (!Directory.Exists(imagesDirectory))
            return;

        var imageFiles = Directory.GetFiles(imagesDirectory, "*.png");

        for (uint i = 0; i < imageFiles.Length; i++)
        {
            var backgroundFile = imageFiles[i];
            var image = new ImageSharpTexture(backgroundFile);
            var deviceImage = image.CreateDeviceTexture(gd, gd.ResourceFactory);

            var imageSize = new Vector2(image.Width, image.Height);
            var view = gd.ResourceFactory.CreateTextureView(deviceImage);
            var ptr = imGuiProvider.ImGuiRenderer.GetOrCreateImGuiBinding(gd.ResourceFactory, view);

            var imageKey = Path.GetFileNameWithoutExtension(backgroundFile);

            ImGuiImages.Add(imageKey, new ImGuiImageInfo
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
        foreach (var kvp in ImGuiImages)
        {
            var image = kvp.Value;
            imGuiProvider.ImGuiRenderer.RemoveImGuiBinding(image.TextureView);
            image.TextureView.Dispose();
            image.Texture.Dispose();
        }

        ImGuiImages.Clear();
    }
}

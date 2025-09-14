using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using Veldrid.ImageSharp;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering background images to render using ImGui
/// </summary>
public class BackgroundImagesProvider
{
    private readonly IFileSystem fileSystem;
    private readonly GraphicsDeviceProvider graphicsDeviceProvider;
    private readonly ImGuiProvider imGuiProvider;

    public Dictionary<string, ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public BackgroundImagesProvider(
        IFileSystem fileSystem,
        GraphicsDeviceProvider graphicsDeviceProvider,
        ImGuiProvider imGuiProvider)
    {
        this.graphicsDeviceProvider = graphicsDeviceProvider;
        this.fileSystem = fileSystem;
        this.imGuiProvider = imGuiProvider;
    }

    public void Load()
    {
        var gd = graphicsDeviceProvider.GraphicsDevice;

        var backgroundFiles = Directory.GetFiles(fileSystem.StagingBackgroundsDirectory, "*.png");

        for (uint i = 0; i < backgroundFiles.Length; i++)
        {
            var backgroundFile = backgroundFiles[i];
            var image = new ImageSharpTexture(backgroundFile);
            var deviceImage = image.CreateDeviceTexture(gd, gd.ResourceFactory);

            var imageSize = new Vector2(image.Width, image.Height);
            var view = gd.ResourceFactory.CreateTextureView(deviceImage);
            var ptr = imGuiProvider.ImGuiRenderer.GetOrCreateImGuiBinding(gd.ResourceFactory, view);

            ImGuiImages.Add(Path.GetFileName(backgroundFile), new ImGuiImageInfo
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
            imGuiProvider.ImGuiRenderer.RemoveImGuiBinding(img.Value.TextureView);
            img.Value.TextureView.Dispose();
            img.Value.Texture.Dispose();
        }

        ImGuiImages.Clear();
    }
}

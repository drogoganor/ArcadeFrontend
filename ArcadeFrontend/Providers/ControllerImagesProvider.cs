using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using static SDL3.SDL;
using static ArcadeFrontend.Shaders.ShaderCommon;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering images to render using ImGui
/// </summary>
public class ControllerImagesProvider
{
    private readonly IApplicationWindow window;

    public Dictionary<string, ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public ControllerImagesProvider(
        IApplicationWindow window)
    {
        this.window = window;
    }

    public void UpdateGame()
    {
        Unload();
        Load();
    }

    public void Load()
    {
        var imagesDirectory = Path.Combine(Environment.CurrentDirectory, "Content", "images", "controller");

        if (!Directory.Exists(imagesDirectory))
            return;

        var imageFiles = Directory.GetFiles(imagesDirectory, "*.png");

        for (uint i = 0; i < imageFiles.Length; i++)
        {
            var backgroundFile = imageFiles[i];

            var texture = LoadImage(window, backgroundFile, 4, out var imageWidth, out var imageHeight);
            var imageSize = new Vector2(imageWidth, imageHeight);

            var imageKey = Path.GetFileNameWithoutExtension(backgroundFile);
            ImGuiImages.Add(imageKey, new ImGuiImageInfo
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
            SDL_DestroyTexture(img.Value.IntPtr);
        }

        ImGuiImages.Clear();
    }
}

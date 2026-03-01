using ArcadeFrontend.Data;
using ArcadeFrontend.Interfaces;
using System.Numerics;
using static SDL3.SDL;
using static ArcadeFrontend.Shaders.ShaderCommon;

namespace ArcadeFrontend.Providers;

/// <summary>
/// For registering background images to render using ImGui
/// </summary>
public class BackgroundImagesProvider
{
    private readonly IFileSystem fileSystem;
    private readonly IApplicationWindow window;

    public Dictionary<string, ImGuiImageInfo> ImGuiImages { get; private set; } = new();

    public BackgroundImagesProvider(
        IApplicationWindow window,
        IFileSystem fileSystem)
    {
        this.window = window;
        this.fileSystem = fileSystem;
    }

    public void Load()
    {
        var backgroundFiles = Directory.GetFiles(fileSystem.BackgroundsDirectory, "*.png");

        for (uint i = 0; i < backgroundFiles.Length; i++)
        {
            var backgroundFile = backgroundFiles[i];

            var texture = LoadImage(window, backgroundFile, 4, out var imageWidth, out var imageHeight);
            var imageSize = new Vector2(imageWidth, imageHeight);

            ImGuiImages.Add(Path.GetFileName(backgroundFile), new ImGuiImageInfo
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

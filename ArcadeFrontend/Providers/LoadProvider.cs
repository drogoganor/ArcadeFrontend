using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Shaders;

namespace ArcadeFrontend.Providers;

public class LoadProvider : ILoadProvider
{
    private readonly IApplicationWindow window;
    private readonly Sdl2WindowProvider sdl2WindowProvider;
    private readonly GraphicsDeviceProvider graphicsDeviceProvider;
    private readonly ImGuiProvider imGuiProvider;
    private readonly FileLoadProvider fileLoadProvider;
    private readonly ColorShader colorShader;
    private readonly TextureShader textureShader;
    private readonly BackgroundImagesProvider backgroundImagesProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;

    public LoadProvider(
        IApplicationWindow window,
        Sdl2WindowProvider sdl2WindowProvider,
        GraphicsDeviceProvider graphicsDeviceProvider,
        ImGuiProvider imGuiProvider,
        FileLoadProvider fileLoadProvider,
        ColorShader colorShader,
        TextureShader textureShader,
        BackgroundImagesProvider backgroundImagesProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider)
    {
        this.window = window;
        this.sdl2WindowProvider = sdl2WindowProvider;
        this.graphicsDeviceProvider = graphicsDeviceProvider;
        this.imGuiProvider = imGuiProvider;
        this.fileLoadProvider = fileLoadProvider;
        this.colorShader = colorShader;
        this.textureShader = textureShader;
        this.backgroundImagesProvider = backgroundImagesProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
    }

    public void Load()
    {
        sdl2WindowProvider.Load();
        graphicsDeviceProvider.Load();
        window.Load();
        imGuiProvider.Load();
        fileLoadProvider.Load();

        colorShader.Load();
        //textureShader.Load();

        backgroundImagesProvider.Load();
        gameScreenshotImagesProvider.Load();
    }

    public void Unload()
    {
        gameScreenshotImagesProvider.Unload();
        backgroundImagesProvider.Unload();

        //textureShader.Unload();
        colorShader.Unload();

        fileLoadProvider.Unload();
        imGuiProvider.Unload();
        window.Unload();
        graphicsDeviceProvider.Unload();
        sdl2WindowProvider.Unload();
    }
}

using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Shaders;

namespace ArcadeFrontend.Providers;

public class LoadProvider : ILoadProvider
{
    private readonly IApplicationWindow window;
    private readonly FileLoadProvider fileLoadProvider;
    private readonly ColorShader colorShader;
    private readonly TextureShader textureShader;
    private readonly BackgroundImagesProvider backgroundImagesProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly InputProvider inputProvider;
    private readonly ControllerImagesProvider controllerImagesProvider;
    private readonly KeyboardImagesProvider keyboardImagesProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;

    public LoadProvider(
        IApplicationWindow window,
        FileLoadProvider fileLoadProvider,
        ColorShader colorShader,
        TextureShader textureShader,
        BackgroundImagesProvider backgroundImagesProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        InputProvider inputProvider,
        ControllerImagesProvider controllerImagesProvider,
        KeyboardImagesProvider keyboardImagesProvider,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider)
    {
        this.window = window;
        this.fileLoadProvider = fileLoadProvider;
        this.colorShader = colorShader;
        this.textureShader = textureShader;
        this.backgroundImagesProvider = backgroundImagesProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.inputProvider = inputProvider;
        this.controllerImagesProvider = controllerImagesProvider;
        this.keyboardImagesProvider = keyboardImagesProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
    }

    public void Load()
    {
        window.Load();
        inputProvider.Load();
        fileLoadProvider.Load();

        var currentSystem = gamesFileProvider.Data.Keys.FirstOrDefault() ?? "Mame";

        var games = gamesFileProvider.Data[currentSystem].Games;

        // HACK: HACK HACK HACK
        // Set the first system and game
        frontendStateProvider.State.CurrentSystem = currentSystem;
        frontendStateProvider.State.CurrentGame = games.FirstOrDefault(x => x.System == frontendStateProvider.State.CurrentSystem)?.Name;

        colorShader.Load();
        //textureShader.Load();

        backgroundImagesProvider.Load();
        gameScreenshotImagesProvider.Load();
        controllerImagesProvider.Load();
        keyboardImagesProvider.Load();
    }

    public void Unload()
    {
        keyboardImagesProvider.Unload();
        controllerImagesProvider.Unload();
        gameScreenshotImagesProvider.Unload();
        backgroundImagesProvider.Unload();

        //textureShader.Unload();
        colorShader.Unload();

        fileLoadProvider.Unload();
        inputProvider.Unload();
        window.Unload();
    }
}

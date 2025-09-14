using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Menus;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Render;
using Veldrid;

namespace ArcadeFrontend;

public class AppClient : IAppClient
{
    public Action OnEndSession;

    private readonly IApplicationWindow window;
    private readonly IMenuProvider menuProvider;
    private readonly ArcadeUI ui;
    private readonly Scene scene;
    private readonly IWorld world;
    private readonly ILoadProvider loadProvider;

    private bool isExiting = false;

    public AppClient(
        IApplicationWindow window,
        ArcadeUI ui,
        IMenuProvider menuProvider,
        Scene scene,
        IWorld world,
        ILoadProvider loadProvider)
    {
        this.window = window;
        this.menuProvider = menuProvider;
        this.ui = ui;
        this.scene = scene;
        this.world = world;
        this.loadProvider = loadProvider;

        window.Tick += Window_Tick;
        window.Rendering += Window_Rendering;
        window.KeyPressed += Window_KeyPressed;
        ui.OnExit += ExitGame;
    }

    private void Window_Tick(float deltaSeconds)
    {
        world.Tick(deltaSeconds);
    }

    private void Window_KeyPressed(KeyEvent obj)
    {
        if (obj.Key == Key.Escape)
        {
            //ToggleInGameMenu();
        }
    }

    private void Window_Rendering(float deltaSeconds)
    {
        if (!isExiting)
        {
            scene.Draw(deltaSeconds);
        }
    }

    public void Run()
    {
        menuProvider.Show(MenuType.Main);
        window.Run();
    }

    public void ExitGame()
    {
        isExiting = true;
        window.Tick -= Window_Tick;
        window.Rendering -= Window_Rendering;
        window.KeyPressed -= Window_KeyPressed;

        loadProvider.Unload();

        window.Close();
    }

    /// <summary>
    /// Hide all menus and show game UI
    /// Do special handling of camera controller and also unpause local sim
    /// </summary>
    //public void ShowGameUI()
    //{
    //    menuProvider.Show(MenuType.GameUI);

    //    window.Window.CursorVisible = false;
    //    inGameCameraController.ReturnFromMenu();

    //    pauseProvider.Unpause();
    //}

    public void ShowMainMenu()
    {
        menuProvider.Show(MenuType.Main);
    }

    //public void ToggleInGameMenu()
    //{
    //    var isGame = menuProvider.MenuType == MenuType.GameUI;
    //    if (isGame)
    //    {
    //        lastGameMenuType = menuProvider.MenuType;

    //        pauseProvider.Pause();
    //        window.Window.CursorVisible = true;
    //        menuProvider.Show(MenuType.InGameMenu);
    //    }
    //    else if (menuProvider.MenuType == MenuType.InGameMenu)
    //    {
    //        menuProvider.Show(lastGameMenuType);
    //        window.Window.CursorVisible = false;
    //        inGameCameraController.ReturnFromMenu();
    //        pauseProvider.Unpause();
    //    }
    //}
}

using ImGuiNET;
using System.Numerics;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Enums;
using Microsoft.Extensions.Logging;

namespace ArcadeFrontend.Menus;

public class ArcadeUI : Menu
{
    public event Action OnExit;

    private readonly ILogger<ArcadeUI> logger;
    private readonly IFileSystem fileSystem;
    private readonly ConfirmDialog confirmDialog;
    private readonly OptionsDialog optionsDialog;
    private readonly NextTickActionProvider nextTickActionProvider;
    private readonly BigViewComponent bigViewComponent;
    private readonly ListViewComponent listViewComponent;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameCommandsProvider gameCommandsProvider;
    private readonly FrontendSettingsProvider frontendSettingsProvider;
    private readonly BackgroundImagesProvider backgroundImagesProvider;
    private readonly SystemViewComponent systemViewComponent;
    private readonly HeaderComponent headerComponent;
    private readonly FooterComponent footerComponent;

    public ArcadeUI(
        ILogger<ArcadeUI> logger,
        IFileSystem fileSystem,
        IApplicationWindow window,
        ImGuiProvider imGuiProvider,
        ImGuiFontProvider imGuiFontProvider,
        IGraphicsDeviceProvider graphicsDeviceProvider,
        ConfirmDialog confirmDialog,
        OptionsDialog optionsDialog,
        NextTickActionProvider nextTickActionProvider,
        BigViewComponent bigViewComponent,
        ListViewComponent listViewComponent,
        GameCommandsProvider gameCommandsProvider,
        FrontendStateProvider frontendStateProvider,
        FrontendSettingsProvider frontendSettingsProvider,
        BackgroundImagesProvider backgroundImagesProvider,
        SystemViewComponent systemViewComponent,
        HeaderComponent headerComponent,
        FooterComponent footerComponent)
        : base(window, imGuiProvider, imGuiFontProvider, graphicsDeviceProvider)
    {
        this.logger = logger;
        this.fileSystem = fileSystem;
        this.confirmDialog = confirmDialog;
        this.optionsDialog = optionsDialog;
        this.nextTickActionProvider = nextTickActionProvider;
        this.bigViewComponent = bigViewComponent;
        this.listViewComponent = listViewComponent;
        this.gameCommandsProvider = gameCommandsProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.frontendSettingsProvider = frontendSettingsProvider;
        this.backgroundImagesProvider = backgroundImagesProvider;
        this.systemViewComponent = systemViewComponent;
        this.headerComponent = headerComponent;
        this.footerComponent = footerComponent;
    }

    private void HandleConfirmExit(bool result)
    {
        if (result)
        {
            nextTickActionProvider.Enqueue(OnExit);
        }
    }

    public override void Draw(float deltaSeconds)
    {
        if (!IsVisible)
            return;

        DrawMenu();

        headerComponent.Draw(deltaSeconds);

        var state = frontendStateProvider.State;
        if (state.CurrentView == ViewType.Big)
        {
            bigViewComponent.Draw(deltaSeconds);
        }
        else if (state.CurrentView == ViewType.List)
        {
            listViewComponent.Draw(deltaSeconds);
        }
        else if (state.CurrentView == ViewType.System)
        {
            systemViewComponent.Draw(deltaSeconds);
        }

        footerComponent.Draw(deltaSeconds);

        optionsDialog.Draw(deltaSeconds);
        confirmDialog.Draw(deltaSeconds);

        base.Draw(deltaSeconds);
    }


    private void DrawMenu()
    {
        var windowSize = new Vector2(window.Width, window.Height);
        var menuSize = new Vector2(window.Width, 45);

        imGuiFontProvider.PushFont(FontSize.Medium);
        ImGui.BeginMainMenuBar();
        if (ImGui.BeginMenu("File"))
        {
            if (ImGui.MenuItem("Exit"))
            {
                confirmDialog.Show(HandleConfirmExit, "Exit Arcade Frontend", "Are you sure you want to exit?");
            }

            ImGui.EndMenu();
        }

        if (ImGui.BeginMenu("Edit"))
        {
            if (ImGui.MenuItem("Options..."))
            {
                optionsDialog.Show();
            }

            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        imGuiFontProvider.PopFont();


        var settings = frontendSettingsProvider.Settings;
        var backgroundImageAvailable = backgroundImagesProvider.ImGuiImages.TryGetValue(settings.BackgroundImage, out var backgroundImage);

        var state = frontendStateProvider.State;

        if (backgroundImageAvailable)
        {
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.SetNextWindowPos(Vector2.Zero);
            ImGui.SetNextWindowSize(windowSize);
            if (ImGui.Begin("Background",
                ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoDecoration |
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoBringToFrontOnFocus |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoNavFocus |
                ImGuiWindowFlags.NoBackground |
                ImGuiWindowFlags.NoMouseInputs |
                ImGuiWindowFlags.NoFocusOnAppearing))
            {
                ImGui.SetCursorPos(Vector2.Zero);

                if (settings.UseBackgroundImage && backgroundImageAvailable)
                    ImGui.Image(backgroundImage.IntPtr, windowSize);

                ImGui.End();
            }

            ImGui.PopStyleVar();
        }
        else
        {
            state.BackgroundImageAvailable = false;
        }
    }
}

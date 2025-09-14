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
    private readonly GamePickerComponent gamePickerComponent;
    private readonly GameCommandsProvider gameCommandsProvider;

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
        GamePickerComponent gamePickerComponent,
        GameCommandsProvider gameCommandsProvider)
        : base(window, imGuiProvider, imGuiFontProvider, graphicsDeviceProvider)
    {
        this.logger = logger;
        this.fileSystem = fileSystem;
        this.confirmDialog = confirmDialog;
        this.optionsDialog = optionsDialog;
        this.nextTickActionProvider = nextTickActionProvider;
        this.gamePickerComponent = gamePickerComponent;
        this.gameCommandsProvider = gameCommandsProvider;
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

        gamePickerComponent.Draw(deltaSeconds);

        DrawMenu();

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

            if (ImGui.MenuItem("Scan MAME games..."))
            {
                gameCommandsProvider.ScanMameGames();
            }

            ImGui.EndMenu();
        }

        ImGui.EndMainMenuBar();

        imGuiFontProvider.PopFont();
    }
}

using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus;

public class HeaderComponent : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameCommandsProvider gameCommandsProvider;
    private readonly GamePanelComponent gamePanelComponent;
    private readonly ControllerImagesProvider controllerImagesProvider;
    private readonly KeyboardImagesProvider keyboardImagesProvider;
    private readonly ImGuiColorsProvider imGuiColorsProvider;

    public HeaderComponent(
        IApplicationWindow window,
        GamesFileProvider gamesFileProvider,
        ImGuiFontProvider imGuiFontProvider,
        FrontendStateProvider frontendStateProvider,
        GameCommandsProvider gameCommandsProvider,
        GamePanelComponent gamePanelComponent,
        ControllerImagesProvider controllerImagesProvider,
        KeyboardImagesProvider keyboardImagesProvider,
        ImGuiColorsProvider imGuiColorsProvider)
    {
        this.window = window;
        this.gamesFileProvider = gamesFileProvider;
        this.imGuiFontProvider = imGuiFontProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.gameCommandsProvider = gameCommandsProvider;
        this.gamePanelComponent = gamePanelComponent;
        this.controllerImagesProvider = controllerImagesProvider;
        this.keyboardImagesProvider = keyboardImagesProvider;
        this.imGuiColorsProvider = imGuiColorsProvider;
    }

    public void Draw(float deltaSeconds)
    {
        var windowSize = new Vector2(window.Width, window.Height);

        var position = new Vector2(0, UIConstants.MenuHeight + UIConstants.Margin); // UIConstants.Margin
        var size = new Vector2(windowSize.X, UIConstants.BannerHeight); //  - (2 * UIConstants.Margin)

        imGuiFontProvider.PushFont(FontSize.Large);

        ImGui.SetNextWindowPos(position);
        ImGui.SetNextWindowSize(size);


        imGuiColorsProvider.PushColor(ImGuiCol.WindowBg, ImGuiColor.BlackPanelLighter);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);


        if (ImGui.Begin("Header",
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoCollapse |
            //ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoNavFocus |
            //ImGuiWindowFlags.NoBackground |
            ImGuiWindowFlags.NoMouseInputs
            //ImGuiWindowFlags.NoFocusOnAppearing
            ))
        {
            DrawTextTable();
            ImGui.End();
        }

        ImGui.PopStyleVar();
        imGuiColorsProvider.PopColor();
        imGuiFontProvider.PopFont();
    }

    private void DrawTextTable()
    {
        var state = frontendStateProvider.State;

        var currentGame = gamesFileProvider.Data.Games.First(x => x.Name == state.CurrentGame);
        var currentSystem = gamesFileProvider.Data.Systems[currentGame.System];

        var view = state.CurrentView;

        if (view == ViewType.System)
        {
            DrawItems("Choose a game system", showChangeSystem: false, showChangeView: false);
        }
        else
        {
            DrawItems(currentSystem.Name);
        }


        ///////
       
        void DrawItems(string label, bool showChangeSystem = true, bool showChangeView = true)
        {
            var columnCount = 0;
            if (string.IsNullOrWhiteSpace(label))
            {
                return;
            }

            columnCount = 1;
            if (showChangeSystem)
            {
                columnCount += 6;
            }

            if (showChangeView)
            {
                columnCount += 6;
            }

            ImGui.BeginTable("Top text", columnCount);
            ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthStretch);

            if (showChangeSystem)
            {
                ImGui.TableSetupColumn("Padding", ImGuiTableColumnFlags.WidthFixed, 170);
                ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Button", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Or", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Key", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch);
            }

            if (showChangeView)
            {
                ImGui.TableSetupColumn("Padding2", ImGuiTableColumnFlags.WidthFixed, 60);
                ImGui.TableSetupColumn("Label2", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Button2", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Or2", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Key2", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Text2", ImGuiTableColumnFlags.WidthStretch);
            }

            ImGui.TableNextColumn();
            ImGui.Text(label);

            if (showChangeSystem)
            {
                DrawChangeSystemText();
            }

            if (showChangeView)
            {
                DrawChangeViewText();
            }

            ImGui.EndTable();
        }
    }

    private void DrawChangeSystemText()
    {
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.Text("Press");
        ImGui.TableNextColumn();

        if (controllerImagesProvider.ImGuiImages.TryGetValue("w", out var buttonImage))
        {
            ImGui.Image(buttonImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("or");

        ImGui.TableNextColumn();

        if (keyboardImagesProvider.ImGuiImages.TryGetValue("z", out var keyImage))
        {
            ImGui.Image(keyImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("to change system");
    }

    private void DrawChangeViewText()
    {
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.Text("Press");
        ImGui.TableNextColumn();

        if (controllerImagesProvider.ImGuiImages.TryGetValue("n", out var buttonImage))
        {
            ImGui.Image(buttonImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("or");

        ImGui.TableNextColumn();

        if (keyboardImagesProvider.ImGuiImages.TryGetValue("x", out var keyImage))
        {
            ImGui.Image(keyImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("to change view");
    }
}

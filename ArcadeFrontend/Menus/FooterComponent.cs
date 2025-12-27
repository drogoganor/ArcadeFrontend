using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus;

public class FooterComponent : IRenderable
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

    public FooterComponent(
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

        var position = new Vector2(0, windowSize.Y - (UIConstants.BannerHeight + UIConstants.Margin));
        var size = new Vector2(windowSize.X, UIConstants.BannerHeight);

        imGuiFontProvider.PushFont(FontSize.Large);

        ImGui.SetNextWindowPos(position);
        ImGui.SetNextWindowSize(size);

        imGuiColorsProvider.PushColor(ImGuiCol.WindowBg, ImGuiColor.BlackPanelLighter);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

        if (ImGui.Begin("Footer",
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
            DrawItems(showGameLaunch: false);
        }
        else
        {
            DrawItems(showGameLaunch: true);
        }


        ///////
       
        void DrawItems(bool showGameLaunch)
        {
            var columnCount = 5;

            var offsetX = (window.Width - 336) / 2f;

            ImGui.SetCursorPosX(offsetX);

            ImGui.BeginTable("Bottom text", columnCount);
            //ImGui.TableSetupColumn("Padding", ImGuiTableColumnFlags.WidthFixed, 420);
            ImGui.TableSetupColumn("Label", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Button", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Or", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Key", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch);

            //ImGui.TableNextColumn();
            ImGui.TableNextColumn();

            ImGui.Text("Press");

            if (showGameLaunch)
            {
                DrawLaunchGameText();
            }
            else
            {
                DrawSelectSystemText();
            }

            ImGui.EndTable();
        }
    }

    private void DrawSelectSystemText()
    {
        ImGui.TableNextColumn();

        if (controllerImagesProvider.ImGuiImages.TryGetValue("s", out var buttonImage))
        {
            ImGui.Image(buttonImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("or");

        ImGui.TableNextColumn();

        if (keyboardImagesProvider.ImGuiImages.TryGetValue("enter", out var keyImage))
        {
            ImGui.Image(keyImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("to select system");
    }

    private void DrawLaunchGameText()
    {
        ImGui.TableNextColumn();

        if (controllerImagesProvider.ImGuiImages.TryGetValue("s", out var buttonImage))
        {
            ImGui.Image(buttonImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("or");

        ImGui.TableNextColumn();

        if (keyboardImagesProvider.ImGuiImages.TryGetValue("enter", out var keyImage))
        {
            ImGui.Image(keyImage.IntPtr, new Vector2(UIConstants.ButtonIconSize));
        }

        ImGui.TableNextColumn();

        ImGui.Text("to launch game");
    }
}

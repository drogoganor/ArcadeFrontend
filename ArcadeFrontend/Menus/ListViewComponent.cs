using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus;

public class ListViewComponent : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameCommandsProvider gameCommandsProvider;
    private readonly GamePanelComponent gamePanelComponent;

    public ListViewComponent(
        IApplicationWindow window,
        GamesFileProvider gamesFileProvider,
        ImGuiFontProvider imGuiFontProvider,
        FrontendStateProvider frontendStateProvider,
        GameCommandsProvider gameCommandsProvider,
        GamePanelComponent gamePanelComponent)
    {
        this.window = window;
        this.gamesFileProvider = gamesFileProvider;
        this.imGuiFontProvider = imGuiFontProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.gameCommandsProvider = gameCommandsProvider;
        this.gamePanelComponent = gamePanelComponent;
    }

    public void Draw(float deltaSeconds)
    {
        var windowSize = new Vector2(window.Width, window.Height);

        var state = frontendStateProvider.State;
        var currentGame = gamesFileProvider.Data.Games.First(x => x.Name == state.CurrentGame);
        var games = gamesFileProvider.Data.Games.Where(x => x.System == state.CurrentSystem);

        var headerHeight = UIConstants.BannerHeight + (2 * UIConstants.Margin);

        var listPosition = new Vector2(UIConstants.Margin, UIConstants.MenuHeight + headerHeight);
        var listSize = new Vector2(UIConstants.ListWidth, windowSize.Y - (UIConstants.MenuHeight + (2 * headerHeight)));

        var panelWidth = windowSize.X - (UIConstants.ListWidth + (3 * UIConstants.Margin));
        var size = new Vector2(panelWidth, listSize.Y);
        var position = new Vector2(UIConstants.ListWidth + (2 * UIConstants.Margin), listPosition.Y);

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
            imGuiFontProvider.PushFont(FontSize.Large);

            ImGui.SetCursorPos(listPosition);
            if (ImGui.BeginListBox("", listSize))
            {
                foreach (var listItem in games)
                {
                    if (ImGui.Selectable(listItem.Name, listItem.Name == state.CurrentGame))
                    {
                        gameCommandsProvider.SetGame(listItem.Name);
                    }
                }

                ImGui.EndListBox();
            }

            imGuiFontProvider.PopFont();

            // Screenshot
            //var dialogSize = new Vector2(800, listSize.Y);
            var screenshotSize = new Vector2(640, 480);
            //var listRight = UIConstants.ListWidth + (2 * UIConstants.Margin);
            //var dialogPosition = new Vector2(
            //    listRight + ((windowSize.X - listRight - dialogSize.X) / 2f),
            //    UIConstants.MenuHeight + UIConstants.Margin);

            gamePanelComponent.Draw(position, size);
            ImGui.End();
        }

        ImGui.PopStyleVar();
    }
}

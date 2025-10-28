using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus;

public class SystemViewComponent : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameCommandsProvider gameCommandsProvider;
    private readonly GamePanelComponent gamePanelComponent;

    public SystemViewComponent(
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

        var systems = gamesFileProvider.Data.Systems;

        imGuiFontProvider.PushFont(FontSize.Large);

        var menuHeight = 14;
        var borderSpaceWidthX = 12;
        var borderSpaceWidthY = 24;
        var listWidth = 340;

        var listSize = new Vector2(listWidth, windowSize.Y - ((2 * borderSpaceWidthY) + menuHeight));
        var listPosition = new Vector2(borderSpaceWidthX, borderSpaceWidthY + menuHeight);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(windowSize);
        if (ImGui.Begin("System Background",
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
            ImGui.SetCursorPos(listPosition);
            if (ImGui.BeginListBox("", listSize))
            {
                foreach (var kvp in systems)
                {
                    var listItem = kvp.Value;
                    var key = kvp.Key;
                    if (ImGui.Selectable(listItem.Name, key == state.CurrentSystem))
                    {
                        gameCommandsProvider.SetSystem(key);
                    }
                }

                ImGui.EndListBox();
            }

            // Screenshot
            //var dialogSize = new Vector2(800, 620);
            //var screenshotSize = new Vector2(640, 480);
            //var listRight = listWidth + (2 * borderSpaceWidthX);
            //var dialogPosition = new Vector2(listRight + ((windowSize.X - listRight - dialogSize.X) / 2f), (windowSize.Y - dialogSize.Y) / 2f);

            //gamePanelComponent.Draw(dialogPosition, dialogSize);
            ImGui.End();
        }

        ImGui.PopStyleVar();
        imGuiFontProvider.PopFont();
    }
}

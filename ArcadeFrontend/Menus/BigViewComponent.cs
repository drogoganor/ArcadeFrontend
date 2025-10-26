using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;

namespace ArcadeFrontend.Menus;

public class BigViewComponent : IRenderable
{
    private readonly IApplicationWindow window;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly ControllerImagesProvider controllerImagesProvider;
    private readonly GamePanelComponent gamePanelComponent;

    public BigViewComponent(
        IApplicationWindow window,
        GamesFileProvider gamesFileProvider,
        ImGuiFontProvider imGuiFontProvider,
        FrontendStateProvider frontendStateProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        ControllerImagesProvider controllerImagesProvider,
        GamePanelComponent gamePanelComponent)
    {
        this.window = window;
        this.gamesFileProvider = gamesFileProvider;
        this.imGuiFontProvider = imGuiFontProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.controllerImagesProvider = controllerImagesProvider;
        this.gamePanelComponent = gamePanelComponent;
    }

    public void Draw(float deltaSeconds)
    {
        var windowSize = new Vector2(window.Width, window.Height);

        imGuiFontProvider.PushFont(FontSize.Medium);

        var dialogSize = new Vector2(800, 620);
        var dialogPosition = (windowSize - dialogSize) / 2;
        var controllerImageSize = new Vector2(96);
        var borderSpaceWidth = 128;

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

            var leftButtonPosition = new Vector2(borderSpaceWidth - controllerImageSize.X, (windowSize.Y - controllerImageSize.Y) / 2f);
            var rightButtonPosition = new Vector2(windowSize.X - borderSpaceWidth, (windowSize.Y - controllerImageSize.Y) / 2f);

            if (controllerImagesProvider.ImGuiImages.TryGetValue("dpleft", out var leftImage))
            {
                ImGui.SetCursorPos(leftButtonPosition);
                ImGui.Image(leftImage.IntPtr, controllerImageSize);
            }

            if (controllerImagesProvider.ImGuiImages.TryGetValue("dpright", out var rightImage))
            {
                ImGui.SetCursorPos(rightButtonPosition);
                ImGui.Image(rightImage.IntPtr, controllerImageSize);
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();

        imGuiFontProvider.PopFont();

        gamePanelComponent.Draw(dialogPosition, dialogSize);
    }
}

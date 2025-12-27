using ArcadeFrontend.Data;
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

        var headerHeight = UIConstants.BannerHeight + (2 * UIConstants.Margin);

        var position = new Vector2(UIConstants.LargeBorderWidth, UIConstants.MenuHeight + headerHeight);
        var size = new Vector2(windowSize.X - (2 * UIConstants.LargeBorderWidth), windowSize.Y - (UIConstants.MenuHeight + (2 * headerHeight)));

        var controllerImageSize = new Vector2(96);

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

            var leftButtonPosition = new Vector2((UIConstants.LargeBorderWidth - controllerImageSize.X) / 2f, position.Y + ((size.Y - controllerImageSize.Y) / 2f));
            var rightButtonPosition = new Vector2(windowSize.X - ((UIConstants.LargeBorderWidth + controllerImageSize.X) / 2f), position.Y + ((size.Y - controllerImageSize.Y) / 2f));

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

        gamePanelComponent.Draw(position, size);
    }
}

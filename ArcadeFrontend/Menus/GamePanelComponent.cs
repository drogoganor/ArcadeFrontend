using ArcadeFrontend.Data;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Numerics;
using Veldrid;

namespace ArcadeFrontend.Menus;

public class GamePanelComponent
{
    private readonly IApplicationWindow window;
    private readonly GamesFileProvider gamesFileProvider;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GameScreenshotImagesProvider gameScreenshotImagesProvider;
    private readonly ControllerImagesProvider controllerImagesProvider;
    private readonly ImGuiColorsProvider imGuiColorsProvider;
    private readonly KeyboardImagesProvider keyboardImagesProvider;

    public GamePanelComponent(
        IApplicationWindow window,
        GamesFileProvider gamesFileProvider,
        ImGuiFontProvider imGuiFontProvider,
        FrontendStateProvider frontendStateProvider,
        GameScreenshotImagesProvider gameScreenshotImagesProvider,
        ControllerImagesProvider controllerImagesProvider,
        ImGuiColorsProvider imGuiColorsProvider,
        KeyboardImagesProvider keyboardImagesProvider)
    {
        this.window = window;
        this.gamesFileProvider = gamesFileProvider;
        this.imGuiFontProvider = imGuiFontProvider;
        this.frontendStateProvider = frontendStateProvider;
        this.gameScreenshotImagesProvider = gameScreenshotImagesProvider;
        this.controllerImagesProvider = controllerImagesProvider;
        this.imGuiColorsProvider = imGuiColorsProvider;
        this.keyboardImagesProvider = keyboardImagesProvider;

    }

    public void Draw(Vector2 position, Vector2 size)
    {
        var state = frontendStateProvider.State;
        var currentGame = gamesFileProvider.Data.Games.First(x => x.Name == state.CurrentGame);
        var currentSystem = gamesFileProvider.Data.Systems[currentGame.System];

        imGuiFontProvider.PushFont(FontSize.Medium);

        imGuiFontProvider.PushFont(FontSize.ExtraLarge);
        imGuiFontProvider.PopFont();


        var maxScreenshotWidth = size.X - (2 * UIConstants.Margin);

        ImGui.SetNextWindowPos(position);
        ImGui.SetNextWindowSize(size);


        imGuiColorsProvider.PushColor(ImGuiCol.WindowBg, ImGuiColor.BlackPanelLighter);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);

        //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);

        if (ImGui.Begin("Game Display",
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
            imGuiFontProvider.PushFont(FontSize.ExtraLarge);
            HorizontallyCenteredColoredText(currentGame.Name, size.X, RgbaFloat.Green.ToVector4());

            var titleHeight = ImGui.CalcTextSize(currentGame.Name);
            var maxScreenshotHeight = size.Y - ((3 * UIConstants.Margin) + titleHeight.Y);

            imGuiFontProvider.PopFont();

            //HorizontallyCenteredText(currentSystem.Name, size.X);

            var maxScreenshotSize = new Vector2(maxScreenshotWidth, maxScreenshotHeight);

            var firstScreenshot = gameScreenshotImagesProvider.ImGuiImages.FirstOrDefault();
            if (firstScreenshot != null)
            {
                var screenshotSize = Utils.ScaleSizeProportionally(firstScreenshot.PixelSize, maxScreenshotSize);

                ImGui.SetCursorPosX((size.X - screenshotSize.X) / 2f);
                ImGui.Image(firstScreenshot.IntPtr, screenshotSize);
            }

            ImGui.End();
        }

        //ImGui.PopStyleVar();
        ImGui.PopStyleVar();
        imGuiColorsProvider.PopColor();

        //ImGui.PopStyleVar();

        imGuiFontProvider.PopFont();
    }

    private static void HorizontallyCenteredText(string text, float width)
    {
        var textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((width - textWidth) * 0.5f);
        ImGui.Text(text);
    }

    private static void HorizontallyCenteredColoredText(string text, float width, Vector4 color)
    {
        var textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((width - textWidth) * 0.5f);
        ImGui.TextColored(color, text);
    }
}

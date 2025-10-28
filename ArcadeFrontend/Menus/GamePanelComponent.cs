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

        var buttonSize = 32;

        var screenshotSize = new Vector2(640, 480);

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
            imGuiFontProvider.PushFont(FontSize.Large);
            HorizontallyCenteredColoredText(currentGame.Name, size.X, RgbaFloat.Green.ToVector4());
            imGuiFontProvider.PopFont();
            HorizontallyCenteredText(currentSystem.Name, size.X);

            var firstScreenshot = gameScreenshotImagesProvider.ImGuiImages.FirstOrDefault();
            if (firstScreenshot != null)
            {
                ImGui.SetCursorPosX((size.X - screenshotSize.X) / 2f);
                ImGui.Image(firstScreenshot.IntPtr, screenshotSize);
            }


            var actionsWidth = 200;
            var actionsX = (size.X - actionsWidth) / 2f;

            ImGui.SetCursorPosX(actionsX);

            imGuiFontProvider.PushFont(FontSize.Large);
            ImGui.BeginTable("ActionsBar", 4);
            ImGui.TableSetupColumn("Button", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Or", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Key", ImGuiTableColumnFlags.WidthFixed);
            ImGui.TableSetupColumn("Text", ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableNextColumn();

            if (controllerImagesProvider.ImGuiImages.TryGetValue("s", out var buttonImage))
            {
                ImGui.Image(buttonImage.IntPtr, new Vector2(buttonSize));
            }

            ImGui.TableNextColumn();

            ImGui.Text("or");

            ImGui.TableNextColumn();

            if (keyboardImagesProvider.ImGuiImages.TryGetValue("enter", out var keyImage))
            {
                ImGui.Image(keyImage.IntPtr, new Vector2(buttonSize));
            }

            ImGui.TableNextColumn();

            ImGui.Text("to start game");

            ImGui.TableNextColumn();

            if (controllerImagesProvider.ImGuiImages.TryGetValue("w", out buttonImage))
            {
                ImGui.Image(buttonImage.IntPtr, new Vector2(buttonSize));
            }

            ImGui.TableNextColumn();

            ImGui.Text("or");

            ImGui.TableNextColumn();

            if (keyboardImagesProvider.ImGuiImages.TryGetValue("z", out keyImage))
            {
                ImGui.Image(keyImage.IntPtr, new Vector2(buttonSize));
            }

            ImGui.TableNextColumn();

            ImGui.Text("to change view");

            ImGui.EndTable();
            imGuiFontProvider.PopFont();

            ImGui.End();
        }

        ImGui.PopStyleVar();
        //ImGui.PopStyleVar();
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

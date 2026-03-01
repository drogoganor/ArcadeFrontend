using ImGuiNET;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;

namespace ArcadeFrontend.Menus;

public abstract class Menu : IRenderable
{
    protected readonly IApplicationWindow window;
    protected readonly ImGuiFontProvider imGuiFontProvider;

    protected bool IsVisible = true;

    public virtual void Show()
    {
        IsVisible = true;
    }

    public virtual void Hide()
    {
        IsVisible = false;
    }

    public Menu(
        IApplicationWindow window,
        ImGuiFontProvider imGuiFontProvider)
    {
        this.window = window;
        this.imGuiFontProvider = imGuiFontProvider;

        window.Resized += HandleWindowResize;
    }

    public virtual void Draw(float deltaSeconds)
    {
        if (!IsVisible) return;
    }

    protected virtual void HandleWindowResize()
    {
        //imGuiProvider.ImGuiRenderer.WindowResized((int)window.Width, (int)window.Height);
    }

    protected static void HorizontallyCenteredText(string text, float width)
    {
        var textWidth = ImGui.CalcTextSize(text).X;

        ImGui.SetCursorPosX((width - textWidth) * 0.5f);
        ImGui.Text(text);
    }
}

using ImGuiNET;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;

namespace ArcadeFrontend.Menus
{
    /// <summary>
    /// TODO: Handle device destroy
    /// </summary>
    public abstract class Menu : IRenderable
    {
        protected readonly IApplicationWindow window;
        protected readonly GraphicsDeviceProvider graphicsDeviceProvider;
        protected readonly ImGuiProvider imGuiProvider;
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
            ImGuiProvider imGuiProvider,
            ImGuiFontProvider imGuiFontProvider,
            GraphicsDeviceProvider graphicsDeviceProvider)
        {
            this.window = window;
            this.imGuiProvider = imGuiProvider;
            this.imGuiFontProvider = imGuiFontProvider;
            this.graphicsDeviceProvider = graphicsDeviceProvider;

            window.Resized += HandleWindowResize;
        }

        public virtual void Draw(float deltaSeconds)
        {
            if (!IsVisible) return;

            var gd = graphicsDeviceProvider.GraphicsDevice;
            var cl = imGuiProvider.CommandList;
            var imGuiRenderer = imGuiProvider.ImGuiRenderer;

            if (cl == null)
                return;

            cl.Begin();
            cl.SetFramebuffer(graphicsDeviceProvider.Framebuffer);
            imGuiRenderer.Render(gd, cl);
            cl.End();
            gd.SubmitCommands(cl);
        }

        protected virtual void HandleWindowResize()
        {
            imGuiProvider.ImGuiRenderer.WindowResized((int)window.Width, (int)window.Height);
        }

        protected static void HorizontallyCenteredText(string text, float width)
        {
            var textWidth = ImGui.CalcTextSize(text).X;

            ImGui.SetCursorPosX((width - textWidth) * 0.5f);
            ImGui.Text(text);
        }
    }
}

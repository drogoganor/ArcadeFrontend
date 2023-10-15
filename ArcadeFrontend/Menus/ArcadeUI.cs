using ImGuiNET;
using System.Numerics;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using Serilog.Core;
using ArcadeFrontend.Enums;

namespace ArcadeFrontend.Menus
{
    public class ArcadeUI : Menu
    {
        public event Action OnExit;

        private readonly Logger logger;
        private readonly IFileSystem fileSystem;
        private readonly ConfirmDialog confirmDialog;
        private readonly NextTickActionProvider nextTickActionProvider;

        public ArcadeUI(
            Logger logger,
            IFileSystem fileSystem,
            IApplicationWindow window,
            ImGuiProvider imGuiProvider,
            ImGuiFontProvider imGuiFontProvider,
            GraphicsDeviceProvider graphicsDeviceProvider,
            ConfirmDialog confirmDialog,
            NextTickActionProvider nextTickActionProvider)
            : base(window, imGuiProvider, imGuiFontProvider, graphicsDeviceProvider)
        {
            this.logger = logger;
            this.fileSystem = fileSystem;
            this.confirmDialog = confirmDialog;
            this.nextTickActionProvider = nextTickActionProvider;
        }

        private void HandleConfirmExit(bool result)
        {
            if (result)
            {
                nextTickActionProvider.Enqueue(OnExit);
            }
        }

        public override void Draw(float deltaSeconds)
        {
            if (!IsVisible)
                return;

            DrawMenu();

            base.Draw(deltaSeconds);
        }

        private void DrawMenu()
        {
            var mapsDirectory = new DirectoryInfo(fileSystem.MapDirectory);
            var windowSize = new Vector2(window.Width, window.Height);
            var menuSize = new Vector2(window.Width, 45);

            imGuiFontProvider.PushFont(FontSize.Medium);
            ImGui.BeginMainMenuBar();
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Exit"))
                {
                    confirmDialog.Show(HandleConfirmExit, "Exit Editor", "Are you sure you want to exit?");
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Frontend Options..."))
                {
                    //mapOptionsDialog.Show();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Tools"))
            {
                if (ImGui.MenuItem("Options..."))
                {
                    //editorOptionsDialog.Show();
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();

            imGuiFontProvider.PopFont();
        }
    }
}

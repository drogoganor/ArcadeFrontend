using ImGuiNET;
using System.Numerics;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ArcadeFrontend.Enums;

namespace ArcadeFrontend.Menus;

public class ConfirmDialog : IRenderable
{
    public bool IsVisible => isVisible;

    private readonly IApplicationWindow window;
    private readonly ImGuiFontProvider imGuiFontProvider;

    private string title = "";
    private string content = "";
    private Action<bool> onConfirm;
    private bool isVisible;

    public ConfirmDialog(
        IApplicationWindow window,
        ImGuiFontProvider imGuiFontProvider)
    {
        this.window = window;
        this.imGuiFontProvider = imGuiFontProvider;
    }

    public void Show(Action<bool> onConfirm, string title, string content)
    {
        this.title = title;
        this.content = content;
        this.onConfirm = onConfirm;
        isVisible = true;
    }

    private void HandleResult(bool result)
    {
        onConfirm.Invoke(result);
        isVisible = false;
    }

    public void Draw(float deltaSeconds)
    {
        if (!isVisible) return;

        var fullScreenSize = new Vector2(window.Width, window.Height);

        imGuiFontProvider.PushFont(FontSize.Medium);

        var padding = 5;
        var dialogSize = new Vector2(400, 156);
        var dialogPosition = (fullScreenSize - dialogSize) / 2;

        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(fullScreenSize);
        if (ImGui.Begin("",
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoNavFocus |
            ImGuiWindowFlags.Modal))
        {
            ImGui.SetNextWindowPos(dialogPosition);
            ImGui.SetNextWindowSize(dialogSize);

            var controlButtonSize = new Vector2(100, 25);

            if (ImGui.Begin(title,
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize))
            {
                var innerPanelSize = new Vector2(dialogSize.X - padding * 2, 80);
                if (ImGui.BeginChild("File Table", innerPanelSize))
                {
                    ImGui.Text(content);
                    ImGui.EndChild();
                }

                ImGui.BeginTable("SelectionBar", 3);
                ImGui.TableSetupColumn("Selection", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("OK", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Cancel", ImGuiTableColumnFlags.WidthFixed);

                ImGui.TableNextColumn();
                ImGui.TableNextColumn();

                if (ImGui.Button("OK", controlButtonSize))
                {
                    HandleResult(true);
                }

                ImGui.TableNextColumn();

                if (ImGui.Button("Cancel", controlButtonSize))
                {
                    HandleResult(false);
                }

                ImGui.EndTable();

                ImGui.End();
            }

            ImGui.End();
        }

        imGuiFontProvider.PopFont();
    }
}

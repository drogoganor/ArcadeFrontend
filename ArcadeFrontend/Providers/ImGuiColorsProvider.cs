using ArcadeFrontend.Enums;
using ImGuiNET;
using System.Numerics;
using Veldrid;

namespace ArcadeFrontend.Providers;

public class ImGuiColorsProvider
{
    public Dictionary<ImGuiColor, ImGuiCol> Colors { get; set; } = new();

    private Dictionary<ImGuiColor, Vector4> presetColors = new()
    {
        { ImGuiColor.Black, RgbaFloat.Black.ToVector4() },
        { ImGuiColor.InventoryEmpty, new Vector4(0.063f, 0.063f, 0.063f, 1f) },
        { ImGuiColor.PanelBlue, RgbaFloat.CornflowerBlue.ToVector4() }
    };

    public ImGuiColorsProvider()
    {
        foreach (var color in presetColors)
        {
            Colors.Add(color.Key, ImGuiCol.ChildBg);
        }
    }

    public void PushColor(ImGuiCol element, ImGuiColor color)
    {
        ImGui.PushStyleColor(element, presetColors[color]);
    }

    public void PopColor()
    {
        ImGui.PopStyleColor();
    }
}

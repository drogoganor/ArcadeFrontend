using ArcadeFrontend.Enums;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data;

public class FrontendState
{
    public string CurrentSystem { get; set; } = "Mame";
    public string CurrentGame { get; set; } = "19XX: The War Against Destiny";

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ViewType CurrentView { get; set; }

    public bool BackgroundImageAvailable { get; set; } = true;
}

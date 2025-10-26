using ArcadeFrontend.Enums;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data;

public class FrontendState
{
    public int CurrentGameIndex { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ViewType CurrentView { get; set; }

    public bool BackgroundImageAvailable { get; set; } = true;
}

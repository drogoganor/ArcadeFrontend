using ArcadeFrontend.Data.Json;
using ArcadeFrontend.Enums;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data.Files
{
    public class GamesFile
    {
        [JsonConverter(typeof(JsonConverterEnumDictionary<SystemType, SystemData>))]
        public Dictionary<SystemType, SystemData> Systems { get; set; } = new();
        public List<GameData> Games { get; set; } = new();
    }

    public class GameData
    {
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SystemType System { get; set; }
        public string Arguments { get; set; }
    }

    public class SystemData
    {
        public string Name { get; set; }
        public string Directory { get; set; }
        public string Executable { get; set; }
        public string Arguments { get; set; }
    }
}

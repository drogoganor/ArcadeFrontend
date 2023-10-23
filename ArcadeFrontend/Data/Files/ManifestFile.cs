using ArcadeFrontend.Enums;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data.Files
{
    public class ManifestFile
    {
        public string Name { get; set; }
        public string GamesFile { get; set; }
        public string LanguagesFile { get; set; }
        public ModMenuFont[] Fonts { get; set; }
    }

    public class ModMenuFont
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FontSize SizeEnum { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
    }
}

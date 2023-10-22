using ArcadeFrontend.Enums;
using System.Text.Json.Serialization;

namespace ArcadeFrontend.Data.Files
{
    public class ManifestFile
    {
        public string Name { get; set; }
        public int TileSize { get; set; }
        public string[] Campaigns { get; set; }
        public string TextureFile { get; set; }
        public string BlocksFile { get; set; }
        public string ActorFile { get; set; }
        public string SkyboxFile { get; set; }
        public string SoundFile { get; set; }
        public string NotificationsFile { get; set; }
        public string InputFile { get; set; }
        public string DecalsFile { get; set; }
        public string LanguagesFile { get; set; }
        public string WeaponsFile { get; set; }
        public string ProjectilesFile { get; set; }
        public string EffectsFile { get; set; }
        public string PlayerFile { get; set; }
        public ModSpriteData Sprites { get; set; }
        public ModMenuFont[] Fonts { get; set; }
    }

    public class ModSpriteData
    {
        public float SpriteScale { get; set; } = 0.1f;
        public float OrthoScale { get; set; } = 3f;
        public int SpriteImageSize { get; set; } = 3200;
        public string SpriteFile { get; set; }
    }

    public class ModMenuFont
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FontSize SizeEnum { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
    }
}

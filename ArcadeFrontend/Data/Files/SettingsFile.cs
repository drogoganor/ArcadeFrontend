using ArcadeFrontend.Data.Json;
using ArcadeFrontend.Enums;
using System.Numerics;
using System.Text.Json.Serialization;
using Veldrid;

namespace ArcadeFrontend.Data.Files;

public class SettingsFile
{
    public string Language { get; set; } = "en";
    public VideoSettings Video { get; set; } = new();
    public AudioSettings Audio { get; set; } = new();
    public InputSettings Input { get; set; } = new();

    [JsonConverter(typeof(JsonConverterVector4))]
    public Vector4 BackgroundColor { get; set; } = new Vector4(0.13141087f, 0.2670157f, 0.21376769f, 1);

    public bool UseBackgroundImage { get; set; } = false;

    public string BackgroundImage { get; set; } = "default.png";

    public SettingsFile Clone()
    {
        return new SettingsFile
        {
            Language = Language,
            Video = new VideoSettings
            {
                ScreenType = Video.ScreenType,
                BackendType = Video.BackendType,
                FullscreenSize = Video.FullscreenSize,
                WindowedSize = Video.WindowedSize,
                VSync = Video.VSync,
                TextureSamplerType = Video.TextureSamplerType,
                SpriteSamplerType = Video.SpriteSamplerType,
            },
            Audio = new AudioSettings
            {
                SoundVolume = Audio.SoundVolume,
            },
            Input = new InputSettings
            {
                Bindings = Input.Bindings?.ToDictionary(x => x.Key, x => x.Value) ?? new(),
            }
        };
    }
}

public class InputSettings
{
    [JsonConverter(typeof(JsonConverterEnumDictionary<InputBindingType, BoundInput>))]
    public Dictionary<InputBindingType, BoundInput> Bindings { get; set; } = new();
}

public class BoundInput
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public InputType InputType { get; set; }
    public string Input { get; set; }

    // Key and MouseButton are set on load in GameAppLoadProvider
    [JsonIgnore]
    public Key Key { get; set; }

    [JsonIgnore]
    public MouseButton MouseButton { get; set; }
}

public class AudioSettings
{
    public float SoundVolume { get; set; } = 1f;
}

public class VideoSettings
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ScreenType ScreenType { get; set; } = ScreenType.Windowed;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public BackendType BackendType { get; set; } = BackendType.Direct3D11;

    [JsonConverter(typeof(JsonConverterVector2))]
    public Vector2 FullscreenSize { get; set; } = new Vector2(1920, 1080);

    [JsonConverter(typeof(JsonConverterVector2))]
    public Vector2 WindowedSize { get; set; } = new Vector2(1200, 720);

    [JsonIgnore]
    public Vector2 CurrentSize => ScreenType == ScreenType.Windowed ? WindowedSize : FullscreenSize;

    public bool VSync { get; set; } = true;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SamplerType TextureSamplerType { get; set; } = SamplerType.Point;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SamplerType SpriteSamplerType { get; set; } = SamplerType.Point;
}

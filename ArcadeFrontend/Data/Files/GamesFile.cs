namespace ArcadeFrontend.Data.Files;

public class GamesFile
{
    public Dictionary<string, SystemData> Systems { get; set; } = [];
    public List<GameData> Games { get; set; } = [];
}

public class GameData
{
    public string Name { get; set; }

    public string System { get; set; }
    public string Arguments { get; set; }
}

public class SystemData
{
    public string Name { get; set; }
    public string Directory { get; set; }
    public string Executable { get; set; }
    public string Arguments { get; set; }
}

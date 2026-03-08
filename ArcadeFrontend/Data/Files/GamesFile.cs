namespace ArcadeFrontend.Data.Files;

public class GamesFile
{
    public string Name { get; set; }
    public string Directory { get; set; }
    public string RomDirectory { get; set; }
    public string Executable { get; set; }
    public string Arguments { get; set; }
    public List<GameData> Games { get; set; } = [];
}

public class GameData
{
    public string Name { get; set; }
    public string System { get; set; }
    public string Arguments { get; set; }
}

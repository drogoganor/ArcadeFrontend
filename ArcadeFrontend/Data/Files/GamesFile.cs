namespace ArcadeFrontend.Data.Files
{
    public class GamesFile
    {
        public List<GameData> Games { get; set; } = new();
    }

    public class GameData
    {
        public string Name { get; set; }
        public string Platform { get; set; }
        public string ProgramExe { get; set; }
        public string Directory { get; set; }
        public string Arguments { get; set; }
    }
}

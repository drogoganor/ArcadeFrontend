using ArcadeFrontend.Data.Files;
using ArcadeFrontend.Enums;
using ArcadeFrontend.Interfaces;
using ArcadeFrontend.Providers;
using ImGuiNET;
using System.Diagnostics;
using System.Numerics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ArcadeFrontend.Menus;

public class GenerateRomsJsonDialog : IRenderable
{
    public bool IsVisible => isVisible;

    private readonly IApplicationWindow window;
    private readonly ImGuiFontProvider imGuiFontProvider;
    private readonly IFileSystem fileSystem;
    private readonly FrontendStateProvider frontendStateProvider;
    private readonly GamesFileProvider gamesFileProvider;

    private bool isVisible;


    private string extension = ".zip";

    public GenerateRomsJsonDialog(
        IApplicationWindow window,
        ImGuiFontProvider imGuiFontProvider,
        IFileSystem fileSystem,
        FrontendStateProvider frontendStateProvider,
        GamesFileProvider gamesFileProvider)
    {
        this.window = window;
        this.imGuiFontProvider = imGuiFontProvider;
        this.fileSystem = fileSystem;
        this.frontendStateProvider = frontendStateProvider;
        this.gamesFileProvider = gamesFileProvider;
    }

    public void Show()
    {
        isVisible = true;
    }

    private void HandleResult(bool result)
    {
        if (result)
        {
            var currentSystem = gamesFileProvider.Data[frontendStateProvider.State.CurrentSystem];

            var romDirectory = Path.Combine(fileSystem.DataDirectory, currentSystem.Directory, currentSystem.RomDirectory);

            if (Directory.Exists(romDirectory))
            {
                var dummyGameFile = new GamesFile
                {
                    Directory = currentSystem.Directory,
                    RomDirectory = currentSystem.RomDirectory,
                    Arguments = currentSystem.Arguments,
                    Executable = currentSystem.Executable,
                    Name = currentSystem.Name,
                };

                var files = Directory.GetFiles(romDirectory, $"*{extension}");
                foreach (var file in files)
                {
                    var game = new GameData
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Arguments = $"\"{currentSystem.RomDirectory}\\{Path.GetFileName(file)}\"",
                        System = currentSystem.Name
                    };

                    dummyGameFile.Games.Add(game);
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true // Optional: for pretty-printing the output
                };

                var gamesJson = JsonSerializer.Serialize(dummyGameFile, options);

                var appDataFile = $"OutputGameData-{currentSystem.Name}.txt";
                var appDataFilePath = Path.Combine(fileSystem.DataDirectory, appDataFile);

                File.WriteAllText(appDataFilePath, gamesJson);
            }

            //frontendSettingsProvider.SaveSettings();
        }

        isVisible = false;
    }

    public void Draw(float deltaSeconds)
    {
        if (!isVisible) return;

        var fullScreenSize = new Vector2(window.Width, window.Height);

        var dialogSize = new Vector2(510, 326);
        var dialogPosition = (fullScreenSize - dialogSize) / 2;

        imGuiFontProvider.PushFont(FontSize.Medium);

        ImGui.SetNextWindowPos(Vector2.Zero);
        ImGui.SetNextWindowSize(fullScreenSize);
        if (ImGui.Begin("##",
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoBringToFrontOnFocus |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoResize))
        {
            ImGui.SetNextWindowPos(dialogPosition);
            ImGui.SetNextWindowSize(dialogSize);

            if (ImGui.Begin("Generate Roms Json",
                ImGuiWindowFlags.NoCollapse |
                ImGuiWindowFlags.NoMove |
                ImGuiWindowFlags.NoResize))
            {
                imGuiFontProvider.PushFont(FontSize.Small);

                if (ImGui.InputText("Extension", ref extension, 4))
                {

                }

                if (ImGui.Button("Open data directory in Explorer..."))
                {
                    OpenInExplorer(fileSystem.AppDataDirectory);
                }

                imGuiFontProvider.PopFont();
                imGuiFontProvider.PushFont(FontSize.Medium);

                var controlButtonSize = new Vector2(100, 25);

                ImGui.BeginTable("SelectionBar", 3);
                ImGui.TableSetupColumn("Selection", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("OK", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableSetupColumn("Cancel", ImGuiTableColumnFlags.WidthFixed);

                ImGui.TableNextColumn();
                ImGui.TableNextColumn();

                var isValid = true;

                if (!isValid) ImGui.BeginDisabled();

                if (ImGui.Button("OK", controlButtonSize))
                {
                    HandleResult(true);
                }

                if (!isValid) ImGui.EndDisabled();

                ImGui.TableNextColumn();

                if (ImGui.Button("Cancel", controlButtonSize))
                {
                    HandleResult(false);
                }

                ImGui.EndTable();

                imGuiFontProvider.PopFont();

                ImGui.End();
            }

            ImGui.End();
        }

        imGuiFontProvider.PopFont();
    }

    static void OpenInExplorer(string path)
    {
        var cmd = "explorer.exe";
        Process.Start(cmd, path);
    }
}

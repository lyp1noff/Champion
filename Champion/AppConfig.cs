using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Champion;

[Serializable]
public class AppConfig
{
    public int MaxCompetitorsPerGroup { get; set; }
    public int MaxCompetitorsPerRoundGroup { get; set; }
    public int AutoSaveTimeInterval { get; set; }
    public int MaxAutoSaves { get; set; }
    [JsonIgnore]
    public string AppFolder { get; private set; }
    [JsonIgnore]
    public string TemplatesFolder { get; private set; }
    [JsonIgnore]
    public string ExportFolder { get; private set; }
    [JsonIgnore]
    public string CategoriesFile { get; private set; }
    [JsonIgnore]
    public string ConfigFilePath { get; set; }
    [JsonIgnore]
    public string LastSaveFilePath { get; set; }
    [JsonIgnore]
    public string SaveFolderPath{ get; set; }

    public void Initialize()
    {
        string appFolderRaw;
#if WINDOWS
            appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
#elif MACOS
        appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents");
#else
        appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
#endif
        MaxCompetitorsPerGroup = 6;
        MaxCompetitorsPerRoundGroup = 4;
        AutoSaveTimeInterval = 3;
        MaxAutoSaves = 50;
        AppFolder = Path.Combine(appFolderRaw, "Champion");
        TemplatesFolder = Path.Combine(AppFolder, "Templates");
        ExportFolder = Path.Combine(AppFolder, "Export");
        SaveFolderPath = Path.Combine(AppFolder, "Saves");
        CategoriesFile = Path.Combine(AppFolder, "categories.txt");
        ConfigFilePath = Path.Combine(AppFolder, "config.json");
    }

    public AppConfig Load()
    {
        try
        {
            string json = File.ReadAllText(ConfigFilePath);
            AppConfig config = JsonSerializer.Deserialize<AppConfig>(json);
            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config file: {ex.Message}");
            return null;
        }
    }

    public void Save()
    {
        try
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving config to JSON: {ex.Message}");
        }
    }
}
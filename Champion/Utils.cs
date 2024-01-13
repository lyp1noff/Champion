using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Xceed.Words.NET;

namespace Champion;

public class Utils
{
    public static bool IsMicrosoftWordInstalled()
    {
        string[] wordInstallationPaths =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        };

        foreach (var path in wordInstallationPaths)
        {
            var wordExecutablePath = Path.Combine(path, "Microsoft Office\\root\\Office16\\WINWORD.EXE");
            if (File.Exists(wordExecutablePath)) return true;
        }

        return false;
    }

    public static string ComputeMD5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
    }

    [Obsolete("Obsolete")]
    public static string ComputeMD5(object obj)
    {
        using var md5 = MD5.Create();
        var formatter = new BinaryFormatter();
        using (var stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
        }
    }

    public static string GetFileName(string input)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (var c in invalidChars) input = input.Replace(c.ToString(), "");
        var fileName = input;
        return fileName;
    }

    public static string ReadFile(string filePath)
    {
        using var reader = new StreamReader(filePath);
        return reader.ReadToEnd();
    }

    public static void CreateDocxBracket(string categoryName, List<Competitor> competitors, string folder, string fileName)
    {
        using var document = DocX.Load(Path.Combine(folder, $"{competitors.Count}.docx"));
        document.ReplaceText("{{ name_0 }}", categoryName);

        var idx = 1;
        foreach (var competitor in competitors)
        {
            document.ReplaceText($"{{{{ name_{idx} }}}}", competitor.GetFullName());
            idx++;
        }

        document.SaveAs($"{fileName}");
    }

    public static void CreateDocxRoundBracket(string categoryName, List<Competitor> competitors, string folder, string fileName)
    {
        using var document = DocX.Load(Path.Combine(folder, "round.docx"));
        document.ReplaceText("{{ name_0 }}", categoryName);

        var idx = 1;
        foreach (var competitor in competitors)
        {
            document.ReplaceText($"{{{{ name_{idx} }}}}", competitor.GetFullName());
            idx++;
        }

        for (; idx <= 5; idx++)
        {
            document.ReplaceText($"{{{{ name_{idx} }}}}", "");
        }


        document.SaveAs($"{fileName}");
    }

    public static List<int> SplitCompetitors(int numCompetitors, int maxCompetitorsPerGroup)
    {
        var numGroups = (int)Math.Ceiling((double)numCompetitors / maxCompetitorsPerGroup);
        var numPerGroup = numCompetitors / numGroups;
        var numWithExtra = numCompetitors % numGroups;

        var result = new List<int>();

        for (var i = 0; i < numGroups; i++)
        {
            result.Add(numPerGroup);
            if (numWithExtra > 0)
            {
                result[i]++;
                numWithExtra--;
            }
        }

        return result;
    }

    //public static async Task RunDocTo(string arguments, ProgressBar progressbar)
    //{
    //    try
    //    {
    //        var processInfo = new ProcessStartInfo
    //        {
    //            FileName = "docto.exe",
    //            Arguments = arguments,
    //            CreateNoWindow = true,
    //            UseShellExecute = false,
    //            RedirectStandardOutput = true,
    //            RedirectStandardError = true
    //        };

    //        using (var process = new Process { StartInfo = processInfo })
    //        {
    //            process.Start();

    //            while (!process.StandardOutput.EndOfStream)
    //            {
    //                var line = await process.StandardOutput.ReadLineAsync();
    //                if (line != null)
    //                {
    //                    Debug.WriteLine(line);
    //                    if (line.Contains("Converted"))
    //                        Application.Current.Dispatcher.Invoke(() => { progressbar.Value += 1; });
    //                }
    //            }

    //            await process.WaitForExitAsync();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine(ex.ToString());
    //    }
    //}

    public static Boolean ExportValidationCheck(CompetitorManager competitorManager, string templatesFolder, string exportFolder, int maxCompetitorsPerGroup)
    {
        if (string.IsNullOrEmpty(templatesFolder) || string.IsNullOrEmpty(exportFolder))
        {
            //MessageBox.Show("Не указаны папки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return true;
        }

        if (competitorManager.Competitors.GroupBy(c => c.Category).Any(group => group.Count() < 2))
        {
            //MessageBox.Show("Категория содержит менее 2 участников", "Ошибка", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
            return true;
        }

        var competitorSizes = competitorManager.GetCategories().Select(category =>
        {
            var numberOfCompetitors = competitorManager.GetBracket(category).Count();
            return Utils.SplitCompetitors(numberOfCompetitors, maxCompetitorsPerGroup);
        }).SelectMany(list => list).ToList();

        foreach (var size in competitorSizes)
            if (!File.Exists(Path.Combine(templatesFolder, $"{size}.docx")))
            {
                //MessageBox.Show($"Отсутствует шаблон для категории размером {size}", "Ошибка", MessageBoxButton.OK,
                //    MessageBoxImage.Error);
                return true;
            }

        return false;
    }

    public static void DownloadDefaultBrackets(string appFolder, string templatesFolder)
    {
        string bracketsZipUrl = "https://noboobs.help/projects/ChampionBracketMaker/assets/templates.zip";
        string bracketsZipName = Path.GetFileName(bracketsZipUrl);
        string bracketsZip = Path.Combine(appFolder, bracketsZipName);

        string categoriesUrl = "https://noboobs.help/projects/ChampionBracketMaker/assets/categories.txt";
        string categoriesName = Path.GetFileName(categoriesUrl);
        string categories = Path.Combine(appFolder, categoriesName);

        try
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(categoriesUrl, categories);
                client.DownloadFile(bracketsZipUrl, bracketsZip);
            }
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"Ошибка подключения к серверу!", "Ошибка", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
            return;
        }

        if (!Directory.EnumerateFileSystemEntries(templatesFolder).Any())
        {
            ZipFile.ExtractToDirectory(bracketsZip, templatesFolder);
        }
        else
        {
            //MessageBox.Show($"Удалите существующие сетки", "Ошибка", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
        }

        File.Delete(bracketsZip);
    }

    //public static void InitializeDefaultConfig(string configFilePath)
    //{
    //    if (!File.Exists(configFilePath))
    //    {
    //        AppConfig defaultConfig = new AppConfig
    //        {
    //            MaxCompetitorsPerGroup = 6,
    //            MaxCompetitorsPerRoundGroup = 4
    //        };

    //        SaveConfigToJson(defaultConfig, configFilePath);
    //    }
    //}

    //public static AppConfig LoadConfig(string configFilePath)
    //{
    //    try
    //    {
    //        string json = File.ReadAllText(configFilePath);
    //        AppConfig config = JsonSerializer.Deserialize<AppConfig>(json);
    //        return config;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error loading config file: {ex.Message}");
    //        return null;
    //    }
    //}

    //public static void SaveConfigToJson(AppConfig config, string filePath)
    //{
    //    try
    //    {
    //        string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
    //        File.WriteAllText(filePath, json);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error saving config to JSON: {ex.Message}");
    //    }
    //}
}
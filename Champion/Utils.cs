using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xceed.Words.NET;

namespace Champion;

public abstract class Utils
{
    // public static bool IsMicrosoftWordInstalled()
    // {
    //     string[] wordInstallationPaths =
    //     {
    //         Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
    //         Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
    //     };
    //
    //     foreach (var path in wordInstallationPaths)
    //     {
    //         var wordExecutablePath = Path.Combine(path, "Microsoft Office\\root\\Office16\\WINWORD.EXE");
    //         if (File.Exists(wordExecutablePath)) return true;
    //     }
    //
    //     return false;
    // }

    public static string ComputeMd5(string filePath)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
    }

    public static string ComputeMd5(object obj)
    {
        using var md5 = MD5.Create();
        using var stream = new MemoryStream();
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        serializer.WriteObject(stream, obj);
        stream.Position = 0;
        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
    }

    public static string GetFileName(string input)
    {
        var invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        foreach (var c in invalidChars) input = input.Replace(c.ToString(), "");
        var fileName = input;
        return fileName;
    }

    public static void CreateDocxBracket(string categoryName, List<Competitor> competitors, string folder,
        string fileName)
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

    public static void CreateDocxRoundBracket(string categoryName, List<Competitor> competitors, string folder,
        string fileName)
    {
        using var document = DocX.Load(Path.Combine(folder, "round.docx"));
        document.ReplaceText("{{ name_0 }}", categoryName);

        var idx = 1;
        foreach (var competitor in competitors)
        {
            document.ReplaceText($"{{{{ name_{idx} }}}}", competitor.GetFullName());
            idx++;
        }

        for (; idx < 6; idx++) document.ReplaceText($"{{{{ name_{idx} }}}}", "");

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

    public static bool ExportValidationCheck(CompetitorManager competitorManager, string templatesFolder,
        string exportFolder, int maxCompetitorsPerGroup)
    {
        if (string.IsNullOrEmpty(templatesFolder) || string.IsNullOrEmpty(exportFolder))
            //MessageBox.Show("Не указаны папки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return true;

        if (competitorManager.Competitors.GroupBy(c => c.Category).Any(group => group.Count() < 2))
            //MessageBox.Show("Категория содержит менее 2 участников", "Ошибка", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
            return true;

        var competitorSizes = competitorManager.GetCategories().Select(category =>
        {
            var numberOfCompetitors = competitorManager.GetBracket(category).Count();
            return SplitCompetitors(numberOfCompetitors, maxCompetitorsPerGroup);
        }).SelectMany(list => list).ToList();

        foreach (var size in competitorSizes)
            if (!File.Exists(Path.Combine(templatesFolder, $"{size}.docx")))
                //MessageBox.Show($"Отсутствует шаблон для категории размером {size}", "Ошибка", MessageBoxButton.OK,
                //    MessageBoxImage.Error);
                return true;

        return false;
    }

    private static async Task DownloadFileAsync(HttpClient httpClient, string url, string filePath)
    {
        using (var response = await httpClient.GetAsync(url))
        {
            response.EnsureSuccessStatusCode();

            await using (var contentStream = await response.Content.ReadAsStreamAsync())
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await contentStream.CopyToAsync(fileStream);
            }
        }
    }

    public static async Task DownloadDefaultBrackets(string appFolder, string templatesFolder)
    {
        var bracketsZipUrl = "https://noboobs.help/projects/ChampionBracketMaker/assets/templates.zip";
        var bracketsZipName = Path.GetFileName(bracketsZipUrl);
        var bracketsZip = Path.Combine(appFolder, bracketsZipName);

        var categoriesUrl = "https://noboobs.help/projects/ChampionBracketMaker/assets/categories.txt";
        var categoriesName = Path.GetFileName(categoriesUrl);
        var categories = Path.Combine(appFolder, categoriesName);

        try
        {
            using (var httpClient = new HttpClient())
            {
                await DownloadFileAsync(httpClient, categoriesUrl, categories);
                await DownloadFileAsync(httpClient, bracketsZipUrl, bracketsZip);
            }

            if (!Directory.EnumerateFileSystemEntries(templatesFolder).Any())
                ZipFile.ExtractToDirectory(bracketsZip, templatesFolder);
            // MessageBox.Show($"Удалите существующие сетки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            File.Delete(bracketsZip);
        }
        catch (Exception e)
        {
            //MessageBox.Show($"Ошибка подключения к серверу!", "Ошибка", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
            if (!File.Exists(categories))
            {
                File.Create(categories).Close();
            }
        }
    }

    public static async Task ConvertToPdf(string folderPath, string fileName)
    {
        var url = "http://noboobs.help:3300/forms/libreoffice/convert";

        using var formData = new MultipartFormDataContent();
        formData.Add(new StringContent("true"), "merge");

        string[] files = Directory.GetFiles(folderPath);
        for (var i = 0; i < files.Length; i++)
        {
            var filePath = files[i];
            await using var fileStream = File.OpenRead(filePath);
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            formData.Add(fileContent, "files", $"{i}.docx");
        }

        try
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(300);
            HttpResponseMessage response = await httpClient.PostAsync(url, formData);
            if (response.IsSuccessStatusCode)
            {
                await using var fileStream = File.Create(fileName);
                await response.Content.CopyToAsync(fileStream);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void RunProcess(string arg)
    {
        new Process
        {
            StartInfo = new ProcessStartInfo(arg)
            {
                UseShellExecute = true
            }
        }.Start();
    }
}
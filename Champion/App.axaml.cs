using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Champion.ViewModels;
using Champion.Views;

namespace Champion;

public class App : Application
{
    public static MainWindow MainWindow;
    public static AppConfig AppConfig = new();
    public static CompetitorManager CompetitorManager = new();
    public static ObservableCollection<string> AllCategories = new();
    private DispatcherTimer autosaveTimer;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppConfig.Initialize();

        Directory.CreateDirectory(AppConfig.AppFolder);
        Directory.CreateDirectory(AppConfig.TemplatesFolder);
        Directory.CreateDirectory(AppConfig.ExportFolder);
        Directory.CreateDirectory(AppConfig.SaveFolderPath);

        autosaveTimer = new DispatcherTimer();
        autosaveTimer.Tick += AutoSaveTimer_Tick;
        autosaveTimer.Interval = TimeSpan.FromMinutes(AppConfig.AutoSaveTimeInterval);
        autosaveTimer.Start();

        if (Path.Exists(AppConfig.ConfigFilePath))
        {
            var loadedConfig = AppConfig.Load();
            typeof(AppConfig)
                .GetProperties()
                .Where(prop => prop.GetValue(loadedConfig) != null)
                .ToList()
                .ForEach(prop => prop.SetValue(AppConfig, prop.GetValue(loadedConfig)));
        }
        else
        {
            AppConfig.Save();
        }

        
        if (!Directory.EnumerateFileSystemEntries(AppConfig.TemplatesFolder).Any())
            Task.Run(Utils.DownloadDefaultBrackets).Wait();

        if (!Path.Exists(AppConfig.CategoriesFile))
            Task.Run(Utils.DownloadDefaultCategoriesList).Wait();

        AllCategories = new ObservableCollection<string>(File.ReadAllLines(AppConfig.CategoriesFile));
    }

    private void AutoSaveTimer_Tick(object? sender, EventArgs e)
    {
        var dateTime = DateTime.Now;
        var backupSaveFilePath = Path.Combine(AppConfig.SaveFolderPath,
            dateTime.ToString("dd-MM-yyyy-HH-mm-ss") + "-backup.cbr");
        CompetitorManager.Serialize(backupSaveFilePath);

        DirectoryInfo directoryInfo = new DirectoryInfo(AppConfig.SaveFolderPath);
        FileInfo[] files = directoryInfo.GetFiles().OrderBy(f => f.LastWriteTime).ToArray();
        int filesToRemoveCount = Math.Max(0, files.Length - AppConfig.MaxAutoSaves);
        for (int i = 0; i < filesToRemoveCount; i++)
        {
            FileInfo fileToRemove = files[i];
            Console.WriteLine($"Removing file: {fileToRemove.FullName}");
            fileToRemove.Delete();
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var filePath = args[1];
                CompetitorManager.Deserialize(filePath);
                AppConfig.LastSaveFilePath = filePath;
            }
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
            MainWindow = desktop.MainWindow as MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
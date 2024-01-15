using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Champion.ViewModels;
using Champion.Views;
using System.IO;
using System.Linq;

namespace Champion;

public class App : Application
{
    public static CompetitorManager СompetitorManager = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppConfig.Initialize();

        Directory.CreateDirectory(AppConfig.AppFolder);
        Directory.CreateDirectory(AppConfig.TemplatesFolder);
        Directory.CreateDirectory(AppConfig.ExportFolder);

        if (!Directory.EnumerateFileSystemEntries(AppConfig.TemplatesFolder).Any())
            Utils.DownloadDefaultBrackets(AppConfig.AppFolder, AppConfig.TemplatesFolder);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };

        base.OnFrameworkInitializationCompleted();
    }
}
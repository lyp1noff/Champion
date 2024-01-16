using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Champion.ViewModels;
using Champion.Views;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Champion;

public class App : Application
{
    public static AppConfig AppConfig = new();
    public static CompetitorManager CompetitorManager = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppConfig.Initialize();
        AppConfig.MaxCompetitorsPerGroup = 6;
        AppConfig.MaxCompetitorsPerRoundGroup = 4;

        CompetitorManager.AddCompetitor(new Competitor("Andrew", "Paziuka", "Zalan", "CADET OPEN"));
        CompetitorManager.AddCompetitor(new Competitor("Ivan", "Paziuka", "Zalan", "CADET OPEN"));
        CompetitorManager.AddCompetitor(new Competitor("Ярик", "Кипелов", "Боев", "KUMITE 12 YEARS OPEN"));
        CompetitorManager.AddCompetitor(new Competitor("Вася", "Морозов", "Боев", "KUMITE 12 YEARS OPEN"));

        Directory.CreateDirectory(AppConfig.AppFolder);
        Directory.CreateDirectory(AppConfig.TemplatesFolder);
        Directory.CreateDirectory(AppConfig.ExportFolder);

        if (!Directory.EnumerateFileSystemEntries(AppConfig.TemplatesFolder).Any())
            Task.Run(() => Utils.DownloadDefaultBrackets(AppConfig.AppFolder, AppConfig.TemplatesFolder)).Wait();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)

            desktop.MainWindow = OperatingSystem.IsMacOS() ?
                new MainWindow { DataContext = new MainWindowViewModel() } :
                new MainWindowWin { DataContext = new MainWindowViewModel() };

        base.OnFrameworkInitializationCompleted();
    }
}
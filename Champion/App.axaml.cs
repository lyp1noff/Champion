using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Champion.ViewModels;
using Champion.Views;

namespace Champion;

public class App : Application
{
    public static AppConfig AppConfig = new();
    public static CompetitorManager CompetitorManager = new();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        AppConfig.Initialize();

        Directory.CreateDirectory(AppConfig.AppFolder);
        Directory.CreateDirectory(AppConfig.TemplatesFolder);
        Directory.CreateDirectory(AppConfig.ExportFolder);

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
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                Utils.DeserializeCompetitors(args[1]);
            }
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
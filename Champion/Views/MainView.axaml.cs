using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Champion.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        AppConfig.Initialize();

        Directory.CreateDirectory(AppConfig.AppFolder);
        Directory.CreateDirectory(AppConfig.TemplatesFolder);
        Directory.CreateDirectory(AppConfig.ExportFolder);

        if (!Directory.EnumerateFileSystemEntries(AppConfig.TemplatesFolder).Any())
            Utils.DownloadDefaultBrackets(AppConfig.AppFolder, AppConfig.TemplatesFolder);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Champion.ViewModels;

namespace Champion.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }
    public void RefreshDataContext()
    {
        DataContext = new MainWindowViewModel();
    }

    public static FilePickerFileType CBracket { get; } = new("CBracket")
    {
        Patterns = new[] { "*.cbracket" },
        //AppleUniformTypeIdentifiers = new[] { "public.image" },
    };

    private async void OpenFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open CBracket File",
            FileTypeFilter = new[] { CBracket },
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var filePath = files[0].Path.AbsolutePath;
            Utils.DeserializeCompetitors(filePath);
        }
        RefreshDataContext();
    }

    private async void SaveFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save CBracket File",
            FileTypeChoices = new[] { CBracket }
        });

        if (file is not null)
        {
            var filePath = file.Path.AbsolutePath;
            Utils.SerializeCompetitors(filePath);
        }
    }
}
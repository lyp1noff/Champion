using System;
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
        AppleUniformTypeIdentifiers = new[] { "cbracket" },
    };

    private void OpenFileWinClick(object sender, RoutedEventArgs args)
    {
        OpenFile();
    }

    private void SaveFileWinClick(object sender, RoutedEventArgs args)
    {
        if (String.IsNullOrEmpty(App.AppConfig.SaveFilePath))
        {
            SaveFile();
        }
        else
        {
            Utils.SerializeCompetitors(App.AppConfig.SaveFilePath);
        }
    }

    private void SaveAsFileWinClick(object sender, RoutedEventArgs args)
    {
        SaveFile();
    }

    private void OpenFileClick(object sender, EventArgs e)
    {
        OpenFile();
    }

    private void SaveFileClick(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(App.AppConfig.SaveFilePath))
        {
            SaveFile();
        }
        else
        {
            Utils.SerializeCompetitors(App.AppConfig.SaveFilePath);
        }
    }

    private void SaveAsFileClick(object sender, EventArgs e)
    {
        SaveFile();
    }

    private async void OpenFile()
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

    private async void SaveFile()
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
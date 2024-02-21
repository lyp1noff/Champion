using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace Champion.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += MainWindow_Closing;
    }
    
    private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (e.IsProgrammatic) return;
        if (HasUnsavedChanges())
        {
            e.Cancel = true;
            var dialog = new SaveFileDialog();
            var result = await dialog.ShowDialog<int>(App.MainWindow);
            switch (result)
            {
                case 1 when String.IsNullOrEmpty(App.AppConfig.LastSaveFilePath):
                    await SaveFile();
                    Close();
                    break;
                case 1:
                    App.CompetitorManager.Serialize(App.AppConfig.LastSaveFilePath);
                    Close();
                    break;
                case 2:
                    Close();
                    break;
                case 0:
                    e.Cancel = false;
                    break;
            }
        }
    }
    
    private bool HasUnsavedChanges()
    {
        if (App.CompetitorManager.GetSize() == 0) return false;
        if (String.IsNullOrEmpty(App.AppConfig.LastSaveFilePath)) return true;
        if (Utils.ComputeMd5(App.CompetitorManager) == Utils.ComputeMd5(App.AppConfig.LastSaveFilePath))
            return false;
        return true;
    }

    
    private static FilePickerFileType ChampionBracket { get; } = new("ChampionBracket")
    {
        Patterns = new[] { "*.cbr" },
        AppleUniformTypeIdentifiers = new[] { "cbr" },
    };

    private void OpenFileWinClick(object sender, RoutedEventArgs args)
    {
        OpenFile();
    }

    private async void SaveFileWinClick(object sender, RoutedEventArgs args)
    {
        if (String.IsNullOrEmpty(App.AppConfig.LastSaveFilePath))
        {
            await SaveFile();
        }
        else
        {
            App.CompetitorManager.Serialize(App.AppConfig.LastSaveFilePath);
        }
    }

    private async void SaveAsFileWinClick(object sender, RoutedEventArgs args)
    {
        await SaveFile();
    }

    private void OpenFileClick(object sender, EventArgs e)
    {
        OpenFile();
    }

    private async void SaveFileClick(object sender, EventArgs e)
    {
        if (String.IsNullOrEmpty(App.AppConfig.LastSaveFilePath))
        {
            await SaveFile();
        }
        else
        {
            App.CompetitorManager.Serialize(App.AppConfig.LastSaveFilePath);
        }
    }

    private async void SaveAsFileClick(object sender, EventArgs e)
    {
        await SaveFile();
    }

    private async void OpenFile()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Champion Bracket File",
            FileTypeFilter = new[] { ChampionBracket },
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            var filePath = files[0].Path.AbsolutePath;
            App.CompetitorManager.Deserialize(filePath);
        }
    }

    private async Task SaveFile()
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Champion Bracket File",
            FileTypeChoices = new[] { ChampionBracket }
        });

        if (file is not null)
        {
            var filePath = file.Path.AbsolutePath;
            App.CompetitorManager.Serialize(filePath);
        }
    }
}
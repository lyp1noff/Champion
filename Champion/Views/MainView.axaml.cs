using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ChampionBracketMaker;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Champion.Views;

public partial class MainView : UserControl
{
    private readonly string _appFolder = null!;
    private readonly string _templatesFolder = null!;
    private readonly string _exportFolder = null!;


    public MainView()
    {
        InitializeComponent();
        NameTextBox = this.FindControl<TextBox>("NameTextBox");

        string currentDirectory = Environment.CurrentDirectory;
        Console.WriteLine("Current Directory: " + currentDirectory);

        string appFolderRaw;
        #if WINDOWS
            appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        #elif MACOS
            appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Documents");
        #else
            appFolderRaw = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
        #endif
        var appFolder = Path.Combine(appFolderRaw, "Champion");
        _appFolder = appFolder;
        Directory.CreateDirectory(appFolder);

        _templatesFolder = Path.Combine(appFolder, "Templates");
        Directory.CreateDirectory(_templatesFolder);

        _exportFolder = Path.Combine(appFolder, "Export");
        Directory.CreateDirectory(_exportFolder);


        if (!Directory.EnumerateFileSystemEntries(_templatesFolder).Any())
            Utils.DownloadDefaultBrackets(_appFolder, _templatesFolder);

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnButtonClick(object sender, RoutedEventArgs e)
    {
        List<Competitor> competitors = new List<Competitor>();
        competitors.Add(new Competitor("Andrew", "Paziuka", "Zalan", "CADET OPEN"));
        competitors.Add(new Competitor("Ivan", "Paziuka", "Zalan", "CADET OPEN"));

        var exportFolder = Path.Combine(_exportFolder, DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
        Directory.CreateDirectory(exportFolder);

        var filename = Utils.GetFileName(competitors[0].Category);
        var filepath = Path.Combine(exportFolder, filename);


        Utils.CreateDocxBracket(competitors[0].Category, competitors, _templatesFolder, filepath);
        Debug.WriteLine(NameTextBox.Text);
    }
}

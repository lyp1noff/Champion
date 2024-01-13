using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Champion.Views;

public partial class MainView : UserControl
{

    public MainView()
    {
        InitializeComponent();
        NameTextBox = this.FindControl<TextBox>("NameTextBox");

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

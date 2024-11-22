using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Champion.ViewModels;

namespace Champion.Views;

public partial class ExportSelectionDialog : Window
{
    public event Action<List<string>, List<string>>? OnExport;
    
    public ExportSelectionDialog()
    {
        InitializeComponent();
        DataContext = new ExportSelectionViewModel();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ExportSelectionViewModel viewModel)
        {
            var regularCategories = viewModel.GetSelectedCategories();
            var roundCategories = viewModel.GetIndeterminateCategories();
            OnExport?.Invoke(regularCategories, roundCategories);
        }
        Close();
    }
}
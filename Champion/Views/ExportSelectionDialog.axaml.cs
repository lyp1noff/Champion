using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Champion.ViewModels;

namespace Champion.Views;

public partial class ExportSelectionDialog : Window
{
    public List<string> SelectedCategories { get; private set; } = new();

    public ExportSelectionDialog()
    {
        InitializeComponent();
        DataContext = new ExportSelectionViewModel();
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(0);
    }

    private void ExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ExportSelectionViewModel viewModel)
        {
            SelectedCategories = viewModel.GetSelectedCategories();
        }
        Close(1);
    }
    
    private void RoundExportButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ExportSelectionViewModel viewModel)
        {
            SelectedCategories = viewModel.GetSelectedCategories();
        }
        Close(2);
    }
}
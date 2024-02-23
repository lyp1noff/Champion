using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Champion.Views;

public partial class SaveFileDialog : Window
{
    public SaveFileDialog()
    {
        InitializeComponent();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(0);
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(1);
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close(2);
    }
}
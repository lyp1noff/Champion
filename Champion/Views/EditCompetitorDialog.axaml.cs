using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Champion.Views;

public partial class EditCompetitorDialog : Window
{
    private Competitor _competitor;
    
    public EditCompetitorDialog(Competitor competitor)
    {
        InitializeComponent();
        _competitor = competitor;
        FillControls();
    }
    
    private void FillControls()
    {
        NameTextBox.Text = _competitor.Name;
        SurnameTextBox.Text = _competitor.Surname;
        CategoryComboBox.ItemsSource = new ObservableCollection<string>(File.ReadAllLines(App.AppConfig.CategoriesFile));
        CategoryComboBox.Text = _competitor.Category;
        CoachTextBox.Text = _competitor.Coach;
    }

    
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close(_competitor);
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _competitor.Name = NameTextBox.Text;
        _competitor.Surname = SurnameTextBox.Text;
        _competitor.Category = CategoryComboBox.Text;
        _competitor.Coach = CoachTextBox.Text;
        Close(_competitor);
    }
}
using System.Windows.Input;
using ReactiveUI;

namespace Champion.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _surnameTextBox = null!;
    private string _nameTextBox = null!;
    private string _coachTextBox = null!;
    private string _categoryComboBoxSelection = null!;
    
    public MainViewModel()
    {
        AddCompetitor = ReactiveCommand.Create(() =>
        {
            if (_nameTextBox == null || _surnameTextBox == null || _coachTextBox == null || _categoryComboBoxSelection == null) { return; }
            App.СompetitorManager.AddCompetitor(new Competitor(_nameTextBox, _surnameTextBox, _coachTextBox, _categoryComboBoxSelection));
        });
    }

    public ICommand AddCompetitor { get; }
    public string SurnameTextBox
    {
        get => _surnameTextBox;
        set => this.RaiseAndSetIfChanged(ref _surnameTextBox, value);
    }
    public string NameTextBox
    {
        get => _nameTextBox;
        set => this.RaiseAndSetIfChanged(ref _nameTextBox, value);
    }
    public string CoachTextBox
    {
        get => _coachTextBox;
        set => this.RaiseAndSetIfChanged(ref _coachTextBox, value);
    }
    public string CategoryComboBoxSelection
    {
        get => _categoryComboBoxSelection;
        set => this.RaiseAndSetIfChanged(ref _categoryComboBoxSelection, value);
    }
}
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Champion.Views;
using ReactiveUI;

namespace Champion.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<string> _options;
    private Competitor _selectedItem;

    private string _surnameTextBox = null!;
    private string _nameTextBox = null!;
    private string _coachTextBox = null!;
    private string _categoryComboBoxSelection = null!;
    
    public MainViewModel()
    {
        if (!Path.Exists(App.AppConfig.CategoriesFile))
            Task.Run(() => Utils.DownloadDefaultBrackets(App.AppConfig.AppFolder, App.AppConfig.TemplatesFolder)).Wait();
        Options = new ObservableCollection<string>(File.ReadAllLines(App.AppConfig.CategoriesFile));

        AddCompetitor = ReactiveCommand.Create(() =>
        {
            if (_nameTextBox == null || _surnameTextBox == null || _coachTextBox == null ||
                _categoryComboBoxSelection == null)
                return;

            App.CompetitorManager.AddCompetitor(new Competitor(_nameTextBox, _surnameTextBox, _coachTextBox,
                _categoryComboBoxSelection));
        });

        EditCompetitor = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new EditCompetitorDialog(_selectedItem);
            var result = await dialog.ShowDialog<Competitor>(App.MainWindow);
        });
        RemoveCompetitor = ReactiveCommand.Create(() => { App.CompetitorManager.RemoveCompetitor(_selectedItem); });
    }
    
    public ReactiveCommand<Unit, Unit> AddCompetitor { get; }
    public ReactiveCommand<Unit, Unit> EditCompetitor { get; }
    public ReactiveCommand<Unit, Unit> RemoveCompetitor { get; }
    public ObservableCollection<Competitor> Competitors
    {
        get => App.CompetitorManager.Competitors;
        set => this.RaiseAndSetIfChanged(ref App.CompetitorManager.Competitors, value);
    }

    public Competitor SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    public ObservableCollection<string> Options
    {
        get => _options;
        set => this.RaiseAndSetIfChanged(ref _options, value);
    }

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
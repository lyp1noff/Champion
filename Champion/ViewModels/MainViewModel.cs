using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using Champion.Views;
using ReactiveUI;

namespace Champion.ViewModels;

public class MainViewModel : ViewModelBase
{
    private ObservableCollection<Competitor> _competitors;
    private Competitor _selectedItem = null!;
    private string? _surnameTextBox;
    private string? _nameTextBox;
    private string? _coachTextBox;
    private string? _categoryComboBoxSelection;
    
    public MainViewModel()
    {
        _competitors = App.CompetitorManager.Competitors;
        App.CompetitorManager.CollectionChanged += CompetitorManagerCategories_CollectionChanged;
        
        AddCompetitor = ReactiveCommand.Create(() =>
        {
            if (string.IsNullOrEmpty(_coachTextBox) || string.IsNullOrEmpty(_categoryComboBoxSelection))
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
    
    private void CompetitorManagerCategories_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Competitors = App.CompetitorManager.Competitors;
    }
    
    public ReactiveCommand<Unit, Unit> AddCompetitor { get; }
    public ReactiveCommand<Unit, Unit> EditCompetitor { get; }
    public ReactiveCommand<Unit, Unit> RemoveCompetitor { get; }
    
    public ObservableCollection<Competitor> Competitors
    {
        get => _competitors;
        set => this.RaiseAndSetIfChanged(ref _competitors, value);
    }

    public Competitor SelectedItem
    {
        get => _selectedItem;
        set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
    }

    private ObservableCollection<string> Options => App.AllCategories;

    public string SurnameTextBox
    {
        get => _surnameTextBox;
        set => this.RaiseAndSetIfChanged(ref _surnameTextBox, value);
    }

    public string? NameTextBox
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
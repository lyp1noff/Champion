using System.Collections.ObjectModel;

namespace Champion.ViewModels;

public class BracketEditorViewModel : ViewModelBase
{
    private CompetitorManager _competitorManager;

    public BracketEditorViewModel()
    {
        _competitorManager = App.CompetitorManager;
    }

    public ObservableCollection<string> Categories => new(_competitorManager.GetCategories());
}

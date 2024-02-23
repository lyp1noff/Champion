using System.Collections.Generic;
using System.Linq;

namespace Champion.ViewModels;

public class BracketEditorViewModel : ViewModelBase
{
    public List<string> Categories => App.CompetitorManager.GetCategories().OrderBy(s => s).ToList();
}

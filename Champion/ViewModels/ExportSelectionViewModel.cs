using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Champion.ViewModels;

public class ExportSelectionViewModel : ViewModelBase
{
    public ObservableCollection<CategoryItem> Categories { get; set; }

    public ExportSelectionViewModel()
    {
        Categories = new ObservableCollection<CategoryItem>(
            App.CompetitorManager.GetCategories()
                .OrderBy(s => s)
                .Select(name => new CategoryItem { Name = name, IsChecked = false })
        );
    }
    
    public List<string> GetSelectedCategories()
    {
        return Categories
            .Where(c => c.IsChecked)
            .Select(c => c.Name)
            .ToList();
    }
}

public class CategoryItem
{
    public string Name { get; set; }
    public bool IsChecked { get; set; }
}
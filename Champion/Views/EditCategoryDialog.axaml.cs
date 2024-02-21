using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Linq;

namespace Champion.Views
{
    public partial class EditCategoryDialog : Window
    {
        private readonly List<Competitor> _category;

        public EditCategoryDialog(List<Competitor> category)
        {
            InitializeComponent();
            _category = category;

            bool areCategoriesSame = _category.All(c => c.Category == _category[0].Category);
            if (areCategoriesSame)
            {
                CategoryComboBox.ItemsSource = App.AllCategories;
                CategoryComboBox.Text = _category[0].Category;
            }
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }

        private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
        {
            foreach (var competitor in _category)
            {
                competitor.Category = CategoryComboBox.Text;
            }

            Close(true);
        }
    }
}

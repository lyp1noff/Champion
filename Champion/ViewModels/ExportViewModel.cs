using ReactiveUI;
using System;
using System.IO;
using System.Windows.Input;

namespace Champion.ViewModels;

public class ExportViewModel : ViewModelBase
{
    public ExportViewModel()
    {
        ExportToDocx = ReactiveCommand.Create(() =>
        {
            var categories = App.CompetitorManager.GetCategories();

            if (Utils.ExportValidationCheck(App.CompetitorManager, App.AppConfig.TemplatesFolder, App.AppConfig.ExportFolder, App.AppConfig.MaxCompetitorsPerGroup)) { return; }

            var dateTime = DateTime.Now;
            var exportFolder = Path.Combine(App.AppConfig.ExportFolder, dateTime.ToString("dd-MM-yyyy-HH-mm-ss"));
            Directory.CreateDirectory(exportFolder);

            foreach (var categoryName in categories)
            {
                var numberOfCompetitors = App.CompetitorManager.GetBracket(categoryName).Count;
                var categorySizes = Utils.SplitCompetitors(numberOfCompetitors, App.AppConfig.MaxCompetitorsPerGroup);

                var idx = 1;
                var pointer = 0;
                foreach (var categorySize in categorySizes)
                {
                    var category = App.CompetitorManager.GetBracket(categoryName);
                    var filename = Utils.GetFileName(categoryName);
                    var filepath = Path.Combine(exportFolder, filename);
                    var bracket = category.GetRange(pointer, categorySize);
                    pointer += categorySize;
                    if (categorySizes.Count > 1)
                        filepath += $"_{idx}";
                    Utils.CreateDocxBracket(categoryName, bracket, App.AppConfig.TemplatesFolder, $"{filepath}");
                    idx++;
                }
            }
        });
    }
    public ICommand ExportToDocx { get; }
}
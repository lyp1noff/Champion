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
            App.СompetitorManager.AddCompetitor(new Competitor("Andrew", "Paziuka", "Zalan", "CADET OPEN"));
            App.СompetitorManager.AddCompetitor(new Competitor("Ivan", "Paziuka", "Zalan", "CADET OPEN"));

            var currentExportFolder =
                Path.Combine(AppConfig.ExportFolder, DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
            Directory.CreateDirectory(currentExportFolder);
            foreach (var category in App.СompetitorManager.GetCategories())
            {
                var filename = Utils.GetFileName(category);
                var filepath = Path.Combine(currentExportFolder, filename);

                Utils.CreateDocxBracket(category, App.СompetitorManager.GetBracket("CADET OPEN"), AppConfig.TemplatesFolder,
                    filepath);
            }
        });
    }
    public ICommand ExportToDocx { get; }
}
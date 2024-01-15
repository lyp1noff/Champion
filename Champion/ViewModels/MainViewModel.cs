using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using ReactiveUI;

namespace Champion.ViewModels;

public class MainViewModel : ViewModelBase
{
    public CompetitorManager competitorManager = new();

    public MainViewModel()
    {
        AddCompetitor = ReactiveCommand.Create(() =>
        {
            competitorManager.AddCompetitor(new Competitor("Andrew", "Paziuka", "Zalan", "CADET OPEN"));
            competitorManager.AddCompetitor(new Competitor("Ivan", "Paziuka", "Zalan", "CADET OPEN"));

            competitors.Add(new Competitor("Andrew", "Paziuka", "Zalan", "CADET OPEN"));
            competitors.Add(new Competitor("Ivan", "Paziuka", "Zalan", "CADET OPEN"));

            var currentExportFolder =
                Path.Combine(AppConfig.ExportFolder, DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss"));
            Directory.CreateDirectory(currentExportFolder);
            foreach (var category in competitorManager.GetCategories())
            {
                var filename = Utils.GetFileName(category);
                var filepath = Path.Combine(currentExportFolder, filename);

                Utils.CreateDocxBracket(category, competitorManager.GetBracket("CADET OPEN"), AppConfig.TemplatesFolder,
                    filepath);
            }
        });
    }

    public ObservableCollection<Competitor> competitors { get; } = new();

    public ICommand AddCompetitor { get; }
}
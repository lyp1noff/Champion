using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using ReactiveUI;

namespace Champion.ViewModels;

public class ExportViewModel : ViewModelBase
{
    private bool _checkBoxStatus;
    private bool _progressBarVisibility;
    private int _progressBarMax;
    private int _progressBarValue;
    private string _countTotalText;
    private readonly string _versionText;

    public ExportViewModel()
    {
        _checkBoxStatus = true;
        _progressBarVisibility = false;
        _progressBarMax = 0;
        _progressBarValue = 0;
        _countTotalText = "Пусто";
        _versionText = $"Версия: {Assembly.GetExecutingAssembly().GetName().Version?.ToString()}";

        App.CompetitorManager.CollectionChanged += CompetitorManagerCategories_CollectionChanged;

        ExportToDocx = ReactiveCommand.CreateFromTask(async () =>
        {
            var categories = App.CompetitorManager.GetCategories();

            if (Utils.ExportValidationCheck(App.CompetitorManager, App.AppConfig.TemplatesFolder,
                    App.AppConfig.ExportFolder, App.AppConfig.MaxCompetitorsPerGroup)) return;

            var dateTime = DateTime.Now;
            var exportFolder = Path.Combine(App.AppConfig.ExportFolder, dateTime.ToString("dd-MM-yyyy-HH-mm-ss"));
            Directory.CreateDirectory(exportFolder);

            ProgressBarVisibility = true;
            ProgressBarMax = categories.Count;
            ProgressBarValue = 0;

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
                    await Task.Run(() =>
                        Utils.CreateDocxBracket(categoryName, bracket, App.AppConfig.TemplatesFolder, $"{filepath}"));
                    ProgressBarValue += 1;
                    idx++;
                }
            }

            if (CheckBoxStatus)
            {
                await Utils.ConvertToPdf(exportFolder, Path.Combine(exportFolder, "ALL.pdf"));
            }

            ProgressBarVisibility = false;
        });

        ExportToDocxRound = ReactiveCommand.CreateFromTask(async () =>
        {
            var categories = App.CompetitorManager.GetCategories();

            if (Utils.ExportValidationCheck(App.CompetitorManager, App.AppConfig.TemplatesFolder,
                    App.AppConfig.ExportFolder, App.AppConfig.MaxCompetitorsPerRoundGroup)) return;

            var dateTime = DateTime.Now;
            var exportFolder = Path.Combine(App.AppConfig.ExportFolder, dateTime.ToString("dd-MM-yyyy-HH-mm-ss"));
            Directory.CreateDirectory(exportFolder);

            ProgressBarVisibility = true;
            ProgressBarMax = categories.Count;
            ProgressBarValue = 0;

            foreach (var categoryName in categories)
            {
                var numberOfCompetitors = App.CompetitorManager.GetBracket(categoryName).Count;
                var categorySizes =
                    Utils.SplitCompetitors(numberOfCompetitors, App.AppConfig.MaxCompetitorsPerRoundGroup);

                var idx = 1;
                var pointer = 0;
                foreach (var categorySize in categorySizes)
                {
                    var category = App.CompetitorManager.GetBracket(categoryName);
                    var filename = Utils.GetFileName(categoryName);
                    var filepath = Path.Combine(exportFolder, filename);
                    var bracket = category.GetRange(pointer, categorySize);
                    pointer += categorySize;
                    var bracketSize = bracket.Count;
                    if (categorySizes.Count > 1) filepath += $"_{idx}";
                    if (bracketSize == 2)
                    {
                        await Task.Run(() =>
                            Utils.CreateDocxBracket(categoryName, bracket, App.AppConfig.TemplatesFolder,
                                $"{filepath}"));
                    }
                    else
                    {
                        await Task.Run(() =>
                            Utils.CreateDocxRoundBracket(categoryName, bracket, App.AppConfig.TemplatesFolder,
                                $"{filepath}"));
                    }

                    ProgressBarValue += 1;
                    idx++;
                }
            }

            if (CheckBoxStatus)
            {
                await Utils.ConvertToPdf(exportFolder, Path.Combine(exportFolder, "ALL.pdf"));
            }

            ProgressBarVisibility = false;
        });

        ExportByCategory = ReactiveCommand.Create(() =>
        {
            var result = "";
            foreach (var category in App.CompetitorManager.GetCategories())
            {
                var competitorsByCategory = App.CompetitorManager.GetBracket(category);
                result += $"{category}\n";
                foreach (var cmp in competitorsByCategory) result += cmp.GetFullName() + "\n";
                result += "\n";
            }
            
            var filepath = Path.Combine(App.AppConfig.ExportFolder, "category.txt");
            File.WriteAllText(filepath, result);
            
            new Process
            {
                StartInfo = new ProcessStartInfo(filepath)
                {
                    UseShellExecute = true
                }
            }.Start();
        });
        
        ExportByCoach = ReactiveCommand.Create(() =>
        {
            if (App.CompetitorManager.Competitors.Count == 0) return;
            var coachList = App.CompetitorManager.GetCoachList();
            var result = "";
            foreach (var coach in coachList)
            {
                var competitorsByCoach = App.CompetitorManager.GetCompetitorsByCoach(coach);
                result += $"{coach}\n";
                foreach (var cmp in competitorsByCoach) result += cmp.GetString() + "\n";
                result += "\n";
            }
	
            var filepath = Path.Combine(App.AppConfig.ExportFolder, "coach.txt");
            File.WriteAllText(filepath, result);
            
            new Process
            {
                StartInfo = new ProcessStartInfo(filepath)
                {
                    UseShellExecute = true
                }
            }.Start();
        });

        OpenExportFolder = ReactiveCommand.Create(() =>
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(App.AppConfig.ExportFolder)
                {
                    UseShellExecute = true
                }
            }.Start();
        });
        
        OpenAppFolder = ReactiveCommand.Create(() =>
        {
            new Process
            {
                StartInfo = new ProcessStartInfo(App.AppConfig.AppFolder)
                {
                    UseShellExecute = true
                }
            }.Start();
        });
    }
    
    private void CompetitorManagerCategories_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetCounterLabels();
    }

    public ReactiveCommand<Unit, Unit> ExportToDocx { get; }
    public ReactiveCommand<Unit, Unit> ExportToDocxRound { get; }
    public ReactiveCommand<Unit, Unit> ExportByCategory { get; }
    public ReactiveCommand<Unit, Unit> ExportByCoach { get; }
    public ReactiveCommand<Unit, Unit> OpenExportFolder { get; }
    public ReactiveCommand<Unit, Unit> OpenAppFolder { get; }

    public bool ProgressBarVisibility
    {
        get => _progressBarVisibility;
        set => this.RaiseAndSetIfChanged(ref _progressBarVisibility, value);
    }

    public int ProgressBarMax
    {
        get => _progressBarMax;
        set => this.RaiseAndSetIfChanged(ref _progressBarMax, value);
    }

    public int ProgressBarValue
    {
        get => _progressBarValue;
        set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
    }

    public bool CheckBoxStatus
    {
        get => _checkBoxStatus;
        set => this.RaiseAndSetIfChanged(ref _checkBoxStatus, value);
    }

    public string CountTotalText
    {
        get => _countTotalText;
        set => this.RaiseAndSetIfChanged(ref _countTotalText, value);
    }

    public string VersionText
    {
        get => _versionText;
        set => this.RaiseAndSetIfChanged(ref _countTotalText, value);
    }
    
    private void SetCounterLabels()
    {
        var applicationsQuantity = App.CompetitorManager.GetSize();
        var competitorsInMultipleCategory = App.CompetitorManager.CountCompetitorsInMultipleCategory();
        var output = "";
	
        var i = 0;
        for (; i < competitorsInMultipleCategory.Count - 1; i++)
        {
            if (i == 0)
            {
                output += $"Уч. с {i + 1} категорией: {competitorsInMultipleCategory[i]}\n";
                continue;
            }
	
            output += $"Уч. с {i + 1} категориями: {competitorsInMultipleCategory[i]}\n";
        }
	
        output += $"Участников: {competitorsInMultipleCategory[i]}\n";
        output += $"Заявок: {applicationsQuantity}";
	
        // CounterLabel.Content = output.Replace("\n", "\t");
        CountTotalText = output;
    }

}
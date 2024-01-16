using System.Diagnostics;
using ReactiveUI;
using System.Reactive;

namespace Champion.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        MainView = new MainViewModel();
        ExportView = new ExportViewModel();

        OpenFile = ReactiveCommand.Create(() =>
        {
            Debug.WriteLine("open");
        });

        SaveFile = ReactiveCommand.Create(() =>
        {
            Debug.WriteLine("save");
        });

        SaveAsFile = ReactiveCommand.Create(() =>
        {
            Debug.WriteLine("saveas");
        });
    }

    public MainViewModel MainView { get; }
    public ExportViewModel ExportView { get; }

    public ReactiveCommand<Unit, Unit> OpenFile { get; }
    public ReactiveCommand<Unit, Unit> SaveFile { get; }
    public ReactiveCommand<Unit, Unit> SaveAsFile { get; }
}
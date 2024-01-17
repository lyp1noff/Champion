namespace Champion.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        MainView = new MainViewModel();
        ExportView = new ExportViewModel();
    }

    public MainViewModel MainView { get; }
    public ExportViewModel ExportView { get; }
}
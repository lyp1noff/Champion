namespace Champion.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        MainView = new MainViewModel();
        BracketEditor = new BracketEditorViewModel();
        ExportView = new ExportViewModel();
    }

    public MainViewModel MainView { get; }
    public BracketEditorViewModel BracketEditor { get; }
    public ExportViewModel ExportView { get; }
}
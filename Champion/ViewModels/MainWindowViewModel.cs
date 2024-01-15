using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace Champion.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, string> OpenCommand { get; }

    public MainWindowViewModel(Func<Task<String>> openCommand) {
        OpenCommand = ReactiveCommand
            .CreateFromTask(openCommand, outputScheduler: RxApp.MainThreadScheduler);
    }
}
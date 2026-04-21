using ReactiveUI;

namespace AvaloniaUniv.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;

    public MainWindowViewModel()
    {
        _currentViewModel = new LauncherViewModel();
    }

    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
    }
}

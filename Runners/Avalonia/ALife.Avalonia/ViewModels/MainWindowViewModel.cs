using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    /// <summary>
    /// View model for the main window. This primarily controls changing between views.
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ViewModels.ViewModelBase"/>
    public class MainWindowViewModel : ViewModelBase
    {
        // The default is the first page
        private ViewModelBase _currentViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // We will default going to the launcher page and have logic from there to go where-ever we need to go
            _currentViewModel = new LauncherViewModel();
        }

        /// <summary>
        /// Gets the current page. The property is read-only
        /// </summary>
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
    }
}

using ReactiveUI;

namespace ALife.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // The default is the first page
        private ViewModelBase _currentPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            // Set current page to first on start up
            _currentPage = new LauncherViewModel();
        }

        /// <summary>
        /// Gets the current page. The property is read-only
        /// </summary>
        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }
    }
}

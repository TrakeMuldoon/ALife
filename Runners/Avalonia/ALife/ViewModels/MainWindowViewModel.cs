using ReactiveUI;

namespace ALife.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            // Set current page to first on start up
            _currentPage = new LauncherViewModel();
        }

        // The default is the first page
        private ViewModelBase _currentPage;

        /// <summary>
        /// Gets the current page. The property is read-only
        /// </summary>
        public ViewModelBase CurrentPage
        {
            get { return _currentPage; }
            set { this.RaiseAndSetIfChanged(ref _currentPage, value); }
        }
    }
}

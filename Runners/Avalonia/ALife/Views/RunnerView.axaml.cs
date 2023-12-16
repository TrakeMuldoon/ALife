using ALife.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Views
{
    public partial class RunnerView : UserControl
    {
        private RunnerViewModel _vm => (RunnerViewModel)DataContext;

        public RunnerView()
        {
            InitializeComponent();
        }

        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            _vm.StopRunner();

            var windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentPage = new LauncherViewModel();
        }

        public void Start_Click(object sender, RoutedEventArgs args)
        {
            _vm.StartRunner();
        }

        public void Stop_Click(object sender, RoutedEventArgs args)
        {
            _vm.StopRunner();
        }

        public void Restart_Click(object sender, RoutedEventArgs args)
        {
            _vm.StartRunner();
        }
    }
}

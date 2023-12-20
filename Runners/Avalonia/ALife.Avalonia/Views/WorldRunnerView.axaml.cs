using ALife.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Avalonia.Views
{
    public partial class WorldRunnerView : UserControl
    {
        public WorldRunnerView()
        {
            InitializeComponent();
        }

        private WorldRunnerViewModel _vm => (WorldRunnerViewModel)DataContext;

        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentViewModel = new LauncherViewModel();
        }
    }
}

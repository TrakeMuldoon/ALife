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

        public void _Click(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// If the simulation is paused, executes a single turn
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_OneTurnButton_Click(object sender, RoutedEventArgs args)
        {
        }

        /// <summary>
        /// Pauses the execution of the simulation.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_PauseButton_Click(object sender, RoutedEventArgs args)
        {
            _vm.IsEnabled = false;
            // TODO: Investigate why this isn't working...
        }

        /// <summary>
        /// Plays the simulation
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Execution_PlayButton_Click(object sender, RoutedEventArgs args)
        {
            _vm.IsEnabled = true;
            // TODO: Investigate why this isn't working...
        }

        public void FF_FFButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentViewModel = new LauncherViewModel();
        }

        public void Seed_NewSeedButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Seed_ResetWorldButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Speed_FastButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Speed_NormalButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Speed_SlowButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Speed_VeryFastButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Speed_VerySlowButton_Click(object sender, RoutedEventArgs args)
        {
        }

        public void Zoom_InButton_Click(object sender, RoutedEventArgs args)
        {
            Zoom_Slider.Value += 100;
        }

        public void Zoom_OutButton_Click(object sender, RoutedEventArgs args)
        {
            Zoom_Slider.Value -= 100;
        }
    }
}

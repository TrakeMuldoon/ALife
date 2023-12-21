using ALife.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Avalonia.Views
{
    /// <summary>
    /// A ScenarioRunner for the Avalonia GUI implementation of ALife
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl"/>
    public partial class ScenarioRunnerView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRunnerView"/> class.
        /// </summary>
        public ScenarioRunnerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the vm.
        /// </summary>
        /// <value>The vm.</value>
        private ScenarioRunnerViewModel _vm => (ScenarioRunnerViewModel)DataContext;

        /// <summary>
        /// Handles the Click event of the Restart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public void Restart_Click(object sender, RoutedEventArgs args)
        {
            _vm.StartRunner();
        }

        /// <summary>
        /// Handles the Click event of the ReturntoLauncher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            _vm.StopRunner();

            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentViewModel = new LauncherViewModel();
        }

        /// <summary>
        /// Handles the Click event of the Start control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public void Start_Click(object sender, RoutedEventArgs args)
        {
            _vm.StartRunner();
        }

        /// <summary>
        /// Handles the Click event of the Stop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        public void Stop_Click(object sender, RoutedEventArgs args)
        {
            _vm.StopRunner();
        }
    }
}

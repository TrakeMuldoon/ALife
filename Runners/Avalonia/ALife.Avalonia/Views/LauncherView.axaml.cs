using ALife.Avalonia.ViewModels;
using ALife.Core.Scenarios;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Avalonia.Views
{
    /// <summary>
    /// The LauncherView is the first view that is shown to the user. It allows the user to select a scenario and seed
    /// to run.
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl"/>
    public partial class LauncherView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherView"/> class.
        /// </summary>
        public LauncherView()
        {
            InitializeComponent();

            (string, int?, AutoStartMode)? startingScenario = ScenarioRegister.GetAutoStartScenario();
            if(startingScenario != null)
            {
                _vm.SelectedScenario = startingScenario.Value.Item1;
                _vm.CurrentSeedText = startingScenario.Value.Item2?.ToString() ?? string.Empty;

                StartSimulation(startingScenario.Value.Item3);
            }
            else
            {
                // otherwise setup the DataContext
                DataContext = new LauncherViewModel();
            }
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The vm.</value>
        private LauncherViewModel _vm => (LauncherViewModel)DataContext;

        /// <summary>
        /// Handles the Click event of the LaunchGui control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void LaunchGui_Click(object sender, RoutedEventArgs args)
        {
            if(!string.IsNullOrWhiteSpace(_vm.SelectedScenario))
            {
                StartSimulation(AutoStartMode.AutoStartVisual);
            }
        }

        /// <summary>
        /// Handles the Click event of the LaunchRunner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void LaunchRunner_Click(object sender, RoutedEventArgs args)
        {
            if(!string.IsNullOrWhiteSpace(_vm.SelectedScenario))
            {
                StartSimulation(AutoStartMode.AutoStartConsole);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ScenariosList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        public void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectScenario(selectedItem);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the SeedSuggestions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectSeed(selectedItem);
        }

        /// <summary>
        /// Starts the simulation.
        /// </summary>
        /// <param name="startMode">The start mode.</param>
        private void StartSimulation(AutoStartMode startMode)
        {
            // grab the seed from the text box
            int? seed = int.TryParse(_vm.CurrentSeedText, out int x) ? x : null;

            // Instantiate a new ViewModel based on the start mode
            ViewModelBase vm = startMode switch
            {
                AutoStartMode.AutoStartConsole => new BatchRunnerViewModel(_vm.SelectedScenario, seed, _vm.AutoStartScenarioRunner),
                AutoStartMode.AutoStartVisual => new SingularRunnerViewModel(_vm.SelectedScenario, _vm.AutoStartScenarioRunner, seed),
                _ => throw new System.Exception($"Invalid start mode: {startMode}"),
            };

            // Change the page to the selected ViewModel
            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;
            windowMvm.CurrentViewModel = vm;
        }
    }
}

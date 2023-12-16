using ALife.Core.Scenarios;
using ALife.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Views
{
    public partial class LauncherView : UserControl
    {
        private LauncherViewModel _vm => (LauncherViewModel)DataContext;

        public LauncherView()
        {
            InitializeComponent();

            var startingScenario = ScenarioRegister.GetAutoStartScenario();
            if(startingScenario != null)
            {
                var scenarioName = startingScenario.Value.Item1;
                var scenarioSeed = startingScenario.Value.Item2;
                var mode = startingScenario.Value.Item3;
                // TODO: Start the scenario in the requested mode (Visual or Console)
                if (mode == AutoStartMode.AutoStartConsole)
                {
                    // Start in Console mode
                }
                else
                {
                    // Start the Visual mode
                }
            }
            else
            {
                // otherwise setup the DataContext
                this.DataContext = new LauncherViewModel();

            }
        }

        public void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectScenario(selectedItem);
        }

        public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectSeed(selectedItem);
        }

        public void LaunchRunner_Click(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(_vm.SelectedScenario))
            {
                var windowMvm = (MainWindowViewModel)Parent.DataContext;

                int? seed = int.TryParse(_vm.CurrentSeedText, out var x) ? x : null;
                var runner = new RunnerViewModel(_vm.SelectedScenario, seed, _vm.AutoStartScenarioRunner);

                windowMvm.CurrentPage = runner;
            }
        }

        public void LaunchGui_Click(object sender, RoutedEventArgs args)
        {
            // TODO
        }
    }
}

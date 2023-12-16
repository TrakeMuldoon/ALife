using ALife.Core.Scenarios;
using ALife.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Views
{
    public partial class LauncherView : UserControl
    {
        private LauncherViewModel _mvm => (LauncherViewModel)DataContext;

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
            _mvm.SelectScenario(selectedItem);
        }

        public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (ListBox)sender;
            var selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _mvm.SelectSeed(selectedItem);
        }

        public void LaunchRunner_Click(object sender, RoutedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(_mvm.SelectedScenario))
            {
                var windowMvm = (MainWindowViewModel)Parent.DataContext;
                var runner = new RunnerViewModel();


                windowMvm.CurrentPage = runner;
            }
        }

        public void LaunchGui_Click(object sender, RoutedEventArgs args)
        {
            // TODO
        }
    }
}

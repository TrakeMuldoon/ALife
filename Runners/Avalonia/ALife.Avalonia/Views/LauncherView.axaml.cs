using ALife.Avalonia.ViewModels;
using ALife.Core.Scenarios;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Avalonia.Views
{
    public partial class LauncherView : UserControl
    {
        public LauncherView()
        {
            InitializeComponent();

            (string, int?, AutoStartMode)? startingScenario = ScenarioRegister.GetAutoStartScenario();
            if(startingScenario != null)
            {
                _ = startingScenario.Value.Item1;
                _ = startingScenario.Value.Item2;
                AutoStartMode mode = startingScenario.Value.Item3;
                // TODO: Start the scenario in the requested mode (Visual or Console)
                if(mode == AutoStartMode.AutoStartConsole)
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
                DataContext = new LauncherViewModel();
            }
        }

        private LauncherViewModel _vm => (LauncherViewModel)DataContext;

        public void LaunchGui_Click(object sender, RoutedEventArgs args)
        {
            // TODO
        }

        public void LaunchRunner_Click(object sender, RoutedEventArgs args)
        {
            if(!string.IsNullOrWhiteSpace(_vm.SelectedScenario))
            {
                MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.DataContext;

                int? seed = int.TryParse(_vm.CurrentSeedText, out int x) ? x : null;
                ScenarioRunnerViewModel runner = new(_vm.SelectedScenario, seed, _vm.AutoStartScenarioRunner);

                windowMvm.CurrentPage = runner;
            }
        }

        public void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectScenario(selectedItem);
        }

        public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            string selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
            _vm.SelectSeed(selectedItem);
        }
    }
}

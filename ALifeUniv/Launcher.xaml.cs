using System.Linq;
using System.Text;
using ALifeUni.ALife.Scenarios;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ALifeUni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Launcher : Page
    {
        public Launcher()
        {
            InitializeComponent();

            // See ALife.Scenarios.ScenarioFactory.cs for a list of scenarios

            var scenarioNameOverride = string.Empty; // set to something to skip straight to the scenario and not display the launcher
            int? scenarioSeedOverride = null; // null = random seed

            if (!string.IsNullOrWhiteSpace(scenarioNameOverride))
            {
                MainPage.ScenarioName = scenarioNameOverride;
                MainPage.ScenarioSeed = scenarioSeedOverride;
                _ = Frame.Navigate(typeof(MainPage));
            }
            else
            {
                SuggestionsText.Text = string.Empty;
                ScenariosList.Items.Clear();
                foreach (var scenario in ScenarioFactory.Scenarios)
                {
                    ScenariosList.Items.Add(scenario);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the Button control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ScenariosList.SelectedItem is string scenarioName)
            {
                MainPage.ScenarioName = scenarioName;
                MainPage.ScenarioSeed = int.TryParse(SeedText.Text, out var seed) ? seed : (int?)null;
                _ = Frame.Navigate(typeof(MainPage));
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ScenariosList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestionsText.Text = string.Empty;
            if (ScenariosList.SelectedItem is string scenarioName)
            {
                var sb = new StringBuilder();
                var suggestions = ScenarioFactory.GetSuggestions(scenarioName);
                if (suggestions.Count > 0)
                {
                    var maxSeedLength = suggestions.Select(x => x.Key).Max().ToString().Length;

                    foreach (var suggestion in suggestions)
                    {
                        _ = sb.AppendLine($"{suggestion.Key.ToString($"D{maxSeedLength}")} : {suggestion.Value}");
                    }
                    SuggestionsText.Text = sb.ToString();
                }
            }
        }
    }
}

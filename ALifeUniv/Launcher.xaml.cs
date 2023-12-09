using ALifeUni.ALife.Scenarios;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private string StartingSeedText = string.Empty;
        private Dictionary<string, (int, string)> currentSeedSuggestions;

        public Launcher()
        {
            InitializeComponent();

            // See ALife.Scenarios.ScenarioFactory.cs for a list of scenarios
            // To skip straight to a scenario and skip the launcher, set the AutoStartScenario in the ScenarioRegistration attribute for a scenario to true.
            Nullable<(string, Nullable<int>)> startingScenario = ScenarioRegister.GetAutoStartScenario();
            if(startingScenario != null)
            {
                MainPage.ScenarioName = startingScenario.Value.Item1;
                MainPage.ScenarioSeed = startingScenario.Value.Item2;
                Frame.Navigate(typeof(MainPage));
            }
            else
            {
                StartingSeedText = SeedText.Text;
                currentSeedSuggestions = new Dictionary<string, (int, string)>();
                DescriptionText.Text = string.Empty;
                SeedSuggestions.Items.Clear();
                ScenariosList.Items.Clear();
                foreach(string scenarioName in ScenarioRegister.Scenarios)
                {
                    ScenariosList.Items.Add(scenarioName);
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ScenariosList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeedText.Text = StartingSeedText;
            DescriptionText.Text = string.Empty;
            SeedSuggestions.Items.Clear();
            currentSeedSuggestions.Clear();
            if(ScenariosList.SelectedItem is string scenarioName)
            {
                ScenarioRegistration scenarioDetails = ScenarioRegister.GetScenarioDetails(scenarioName);
                DescriptionText.Text = scenarioDetails.Description;

                Dictionary<int, string> suggestions = ScenarioRegister.GetSuggestions(scenarioName);
                if(suggestions.Count > 0)
                {
                    int maxSeedLength = suggestions.Select(x => x.Key).Max().ToString().Length;

                    foreach(KeyValuePair<int, string> suggestion in suggestions)
                    {
                        string seedDescription = $"{suggestion.Key.ToString($"D{maxSeedLength}")} : {suggestion.Value}";
                        currentSeedSuggestions.Add(seedDescription, (suggestion.Key, suggestion.Value));
                        SeedSuggestions.Items.Add(seedDescription);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the SeedSuggestions control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SeedText.Text = StartingSeedText;
            if(SeedSuggestions.SelectedItem is string seedDescription)
            {
                if(currentSeedSuggestions.TryGetValue(seedDescription, out var seedDetails))
                {
                    SeedText.Text = seedDetails.Item1.ToString();
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the LaunchScenarioRunner control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LaunchScenarioRunner_Click(object sender, RoutedEventArgs e)
        {
            if(ScenariosList.SelectedItem is string scenarioName)
            {
                ScenarioRunner.ScenarioName = scenarioName;
                ScenarioRunner.ScenarioSeed = int.TryParse(SeedText.Text, out var seed) ? seed : (int?)null;
                Frame.Navigate(typeof(ScenarioRunner));
            }
        }

        /// <summary>
        /// Handles the Click event of the LaunchScenarioUI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void LaunchScenarioUI_Click(object sender, RoutedEventArgs e)
        {
            if(ScenariosList.SelectedItem is string scenarioName)
            {
                MainPage.ScenarioName = scenarioName;
                MainPage.ScenarioSeed = int.TryParse(SeedText.Text, out var seed) ? seed : (int?)null;
                Frame.Navigate(typeof(MainPage));
            }
        }
    }
}

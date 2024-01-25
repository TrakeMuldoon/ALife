using ALifeUni.ScenarioRunners;
using System;
using System.Threading;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ALifeUni
{
    /// <summary>
    /// A page displaying the scenario runner
    /// </summary>
    public sealed partial class ScenarioRunner : Page, System.IDisposable
    {
        /// <summary>
        /// True to automatically start the scenario runner on entering the page
        /// </summary>
        public static bool AutoStartScenarioRunner = false;

        /// <summary>
        /// The scenario name
        /// </summary>
        public static string ScenarioName = string.Empty;

        /// <summary>
        /// The scenario seed
        /// </summary>
        public static int? ScenarioSeed = null;

        /// <summary>
        /// The runner
        /// </summary>
        private UiScenarioRunner runner = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScenarioRunner"/> class.
        /// </summary>
        public ScenarioRunner()
        {
            InitializeComponent();
            _ = GetOrResetScenarioParameters(true);

            ConsoleText.Text = string.Empty;
            SeedText.Text = string.Empty;
            if(AutoStartScenarioRunner)
            {
                StartScenarioRunner();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            runner.StopRunner(true);
            runner.Dispose();
        }

        /// <summary>
        /// Gets the or reset scenario parameters.
        /// </summary>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        /// <returns></returns>
        private (int, int, int, int) GetOrResetScenarioParameters(bool reset = false)
        {
            // get the number of scenarios we want to execute
            if(!int.TryParse(NumberExecutions.Text, out int seedCount) || reset)
            {
                seedCount = ScenarioRunners.Constants.DEFAULT_NUMBER_SEEDS_EXECUTED;
                NumberExecutions.Text = seedCount.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(NumberTurns.Text, out int maxTurns) || reset)
            {
                maxTurns = ScenarioRunners.Constants.DEFAULT_TOTAL_TURNS;
                NumberTurns.Text = maxTurns.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(TurnBatch.Text, out int turnBatch) || reset)
            {
                turnBatch = ScenarioRunners.Constants.DEFAULT_TURN_BATCH;
                TurnBatch.Text = turnBatch.ToString();
            }

            // get the number of turns we want per scenario
            if(!int.TryParse(UpdateFrequency.Text, out int updateFrequency) || reset)
            {
                updateFrequency = ScenarioRunners.Constants.DEFAULT_UPDATE_FREQUENCY;
                UpdateFrequency.Text = updateFrequency.ToString();
            }

            return (seedCount, maxTurns, turnBatch, updateFrequency);
        }

        /// <summary>
        /// Handles the Click event of the Restart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void Restart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(!runner.IsStopped)
            {
                StopRunner();
            }

            ConsoleText.Text += $"{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}------------------------------------------------------{Environment.NewLine}------------------------------------------------------{Environment.NewLine}------------------------------------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}";

            StartScenarioRunner();
        }

        /// <summary>
        /// Handles the Click event of the ReturntoLauncher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void ReturntoLauncher_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if((bool)!runner?.IsStopped)
            {
                StopRunner();
            }
            _ = Frame.Navigate(typeof(Launcher));
        }

        /// <summary>
        /// Handles the Click event of the Start control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void Start_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(!runner?.IsStopped ?? true)
            {
                StartScenarioRunner();
            }
        }

        /// <summary>
        /// Starts the scenario runner.
        /// </summary>
        private void StartScenarioRunner()
        {
            (int seedCount, int maxTurns, int turnBatch, int updateFrequency) = GetOrResetScenarioParameters();

            runner = new UiScenarioRunner(ConsoleText, SeedText, ScenarioName, ScenarioSeed, numberSeedsToExecute: seedCount, totalTurns: maxTurns, turnBatch: turnBatch, updateFrequency: updateFrequency);
        }

        /// <summary>
        /// Handles the Click event of the Stop control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void Stop_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StopRunner();
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        private void StopRunner()
        {
            if(runner != null)
            {
                runner.StopRunner(true);
                // add some extra sleep time to make sure we're done writing to the console
                Thread.Sleep(500);
            }
        }
    }
}

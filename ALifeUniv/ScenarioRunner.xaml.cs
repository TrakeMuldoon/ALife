using System;
using System.Threading;
using ALifeUni.ScenarioRunners;
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
            NumberExecutions.Text = ScenarioRunners.Constants.DEFAULT_NUMBER_SEEDS_EXECUTED.ToString();
            NumberTurns.Text = ScenarioRunners.Constants.DEFAULT_TOTAL_TURNS.ToString();
            ConsoleText.Text = string.Empty;
            SeedText.Text = string.Empty;
            //StartScenarioRunner();
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
        /// Handles the Click event of the Restart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs"/> instance containing the event data.</param>
        private void Restart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!runner.IsStopped)
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
            if (!runner.IsStopped)
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
            if (!(runner?.IsStopped ?? true))
            {
                StopRunner();
            }
            StartScenarioRunner();
        }

        /// <summary>
        /// Starts the scenario runner.
        /// </summary>
        private void StartScenarioRunner()
        {
            // get the number of scenarios we want to execute
            if (!int.TryParse(NumberExecutions.Text, out var seedCount))
            {
                seedCount = ScenarioRunners.Constants.DEFAULT_NUMBER_SEEDS_EXECUTED;
                NumberExecutions.Text = seedCount.ToString();
            }

            // get the number of turns we want per scenario
            if (!int.TryParse(NumberTurns.Text, out var maxTurns))
            {
                maxTurns = ScenarioRunners.Constants.DEFAULT_TOTAL_TURNS;
                NumberTurns.Text = maxTurns.ToString();
            }

            runner = new UiScenarioRunner(ConsoleText, ScenarioName, ScenarioSeed, numberSeedsToExecute: seedCount, totalTurns: maxTurns);
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
            if (runner != null)
            {
                runner.StopRunner(true);
                // add some extra sleep time to make sure we're done writing to the console
                Thread.Sleep(500);
            }
        }
    }
}

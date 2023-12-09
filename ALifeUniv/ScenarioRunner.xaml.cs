using ALifeUni.Runners;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ALifeUni
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScenarioRunner : Page
    {
        public static string ScenarioName = string.Empty;
        public static int? ScenarioSeed = null;
        private Task runnerTask = null;
        UiScenarioRunner runner;

        public ScenarioRunner()
        {
            this.InitializeComponent();
            StartScenarioRunner();
        }

        private void StartScenarioRunner()
        {
            ConsoleText.Text = string.Empty;
            runner = new UiScenarioRunner(ConsoleText);
            runnerTask = new Task(() => runner.ExecuteRunner(ScenarioName, ScenarioSeed));
            runnerTask.Start();
        }
    }
}

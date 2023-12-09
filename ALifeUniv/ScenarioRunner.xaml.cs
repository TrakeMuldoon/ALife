using ALifeUni.Runners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

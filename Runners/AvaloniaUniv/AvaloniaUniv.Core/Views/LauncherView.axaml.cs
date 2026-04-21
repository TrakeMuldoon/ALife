using ALife.Core.Scenarios;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaUniv.Core.ViewModels;

namespace AvaloniaUniv.Core.Views;

public partial class LauncherView : UserControl
{
    public LauncherView()
    {
        DataContext = new LauncherViewModel();
        InitializeComponent();

        (string, int?, AutoStartMode)? autoStart = ScenarioRegister.GetAutoStartScenario();
        if (autoStart != null)
        {
            Vm.SelectedScenario = autoStart.Value.Item1;
            Vm.CurrentSeedText = autoStart.Value.Item2?.ToString() ?? string.Empty;
            StartSimulation(autoStart.Value.Item3);
        }
    }

    private LauncherViewModel Vm => (LauncherViewModel)DataContext!;

    public void LaunchGui_Click(object sender, RoutedEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(Vm.SelectedScenario))
            StartSimulation(AutoStartMode.AutoStartVisual);
    }

    public void LaunchRunner_Click(object sender, RoutedEventArgs args)
    {
        if (!string.IsNullOrWhiteSpace(Vm.SelectedScenario))
            StartSimulation(AutoStartMode.AutoStartConsole);
    }

    public void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selected = ((ListBox)sender).SelectedItem?.ToString() ?? string.Empty;
        Vm.SelectScenario(selected);
    }

    public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selected = ((ListBox)sender).SelectedItem?.ToString() ?? string.Empty;
        Vm.SelectSeed(selected);
    }

    private void StartSimulation(AutoStartMode mode)
    {
        int? seed = int.TryParse(Vm.CurrentSeedText, out int s) ? s : null;

        ViewModelBase vm = mode switch
        {
            AutoStartMode.AutoStartConsole => new BatchRunnerViewModel(Vm.SelectedScenario, seed, Vm.AutoStartScenarioRunner),
            AutoStartMode.AutoStartVisual => new SimulatorViewModel(Vm.SelectedScenario, seed),
            _ => throw new System.Exception($"Unhandled AutoStartMode: {mode}")
        };

        var windowVm = (MainWindowViewModel)Parent!.DataContext!;
        windowVm.CurrentViewModel = vm;
    }
}

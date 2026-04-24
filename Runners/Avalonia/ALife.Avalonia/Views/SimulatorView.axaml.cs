using ALife.Avalonia.ViewModels;
using ALife.Avalonia.Views;
using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ALife.Avalonia.Views;

public partial class SimulatorView : UserControl, IDisposable
{
    private CancellationTokenSource _cts = new();
    private Task? _perfUpdateTask;
    private bool _disposed;
    private bool _wasRunningBeforeDescendantOpen;
    private BrainViewerWindow? _brainViewer;
    private readonly List<Window> _childWindows = new();

    public SimulatorView()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) =>
        {
            if (TopLevel.GetTopLevel(this) is Window w)
                w.Closing += (_, _) => Dispose();
        };

        // Wire up layers and agent settings once the canvas is ready
        LayersList.ItemsSource = TheWorldCanvas.Simulation.Layers;
        AgentUI.DataContext = TheWorldCanvas.Simulation.AgentUiSettings;

        ZoomBorder.ZoomChanged += (_, _) => UpdateZoomLabel();
        TheWorldCanvas.AllAgentsDied += (_, _) => SetRunState(false);

        SetRunState(true);

        _perfUpdateTask = Task.Run(() => UpdatePerfLoop(_cts.Token));
    }

    private SimulatorViewModel Vm => (SimulatorViewModel)DataContext!;

    public void ReturnToLauncher_Click(object sender, RoutedEventArgs args)
    {
        Dispose();
        var windowVm = (MainWindowViewModel)Parent!.DataContext!;
        windowVm.CurrentViewModel = new LauncherViewModel();
    }

    public void Reset_Click(object sender, RoutedEventArgs args)
    {
        if (!int.TryParse(SeedBox.Text, out int seed))
        {
            seed = new Random().Next();
            SeedBox.Text = seed.ToString();
        }
        TheWorldCanvas.StartingSeed = seed;
        TheWorldCanvas.Simulation.InitializeSimulation();
        SetRunState(true);
    }

    public void Random_Click(object sender, RoutedEventArgs args)
    {
        SeedBox.Text = new Random().Next().ToString();
        Reset_Click(sender, args);
    }

    public void PlayPause_Click(object sender, RoutedEventArgs args) => SetRunState(!TheWorldCanvas.IsEnabled);

    public void OneTurn_Click(object sender, RoutedEventArgs args)
    {
        SetRunState(false);
        TheWorldCanvas.ExecuteTick();
        TheWorldCanvas.InvalidateVisual();
    }

    private static readonly double[] SpeedValues = { 1.0, 2.0, 10.0, 30.0, 60.0, 90.0, 120.0, 240.0, 500.0, 750.0, 1000.0, 1500.0, 2000.0, 5000.0, double.PositiveInfinity };
    private static readonly string[] SpeedLabels = { "1×", "2×", "10×", "30×", "60×", "90×", "120×", "240×", "500×", "750×", "1000×", "1500×", "2000×", "5000×", "∞" };

    public void SpeedSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs args)
    {
        int index = (int)Math.Round(args.NewValue);
        index = Math.Clamp(index, 0, SpeedValues.Length - 1);
        double speed = SpeedValues[index];
        TheWorldCanvas.SetSimulationSpeed(speed);
        SpeedLabel.Text = SpeedLabels[index];
        CustomTpsBox.Text = double.IsInfinity(speed) ? "∞" : ((int)speed).ToString();
    }

    public void CustomTps_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            ApplyCustomTps();
    }

    public void CustomTps_LostFocus(object sender, RoutedEventArgs e)
    {
        ApplyCustomTps();
    }

    private void ApplyCustomTps()
    {
        string text = CustomTpsBox.Text?.Trim() ?? "";
        double speed;
        if (text == "∞")
        {
            speed = double.PositiveInfinity;
        }
        else if (int.TryParse(text, out int tps) && tps >= 1)
        {
            speed = Math.Clamp(tps, 1, 10000);
        }
        else
        {
            return; // invalid — leave as-is
        }

        TheWorldCanvas.SetSimulationSpeed(speed);

        // Sync slider to a preset if the value matches one exactly.
        int matchIndex = -1;
        for (int i = 0; i < SpeedValues.Length; i++)
            if (SpeedValues[i] == speed) { matchIndex = i; break; }

        if (matchIndex >= 0)
        {
            SpeedSlider.Value = matchIndex;
            SpeedLabel.Text = SpeedLabels[matchIndex];
        }
        else
        {
            SpeedLabel.Text = double.IsInfinity(speed) ? "∞" : $"{(int)speed}×";
        }

        CustomTpsBox.Text = double.IsInfinity(speed) ? "∞" : ((int)speed).ToString();
    }

    public void ZoomIn_Click(object sender, RoutedEventArgs args)
    {
        ZoomBorder.ZoomIn();
        UpdateZoomLabel();
    }

    public void ZoomOut_Click(object sender, RoutedEventArgs args)
    {
        ZoomBorder.ZoomOut();
        UpdateZoomLabel();
    }

    public void ZoomReset_Click(object sender, RoutedEventArgs args)
    {
        ZoomBorder.ResetMatrix();
        UpdateZoomLabel();
    }

    private void UpdateZoomLabel()
    {
        ZoomLabel.Text = $"{ZoomBorder.ZoomX:F2}x";
    }

    public void FF_Click(object sender, RoutedEventArgs args)
    {
        if (!TheWorldCanvas.Simulation.IsInitialized) return;
        if (!int.TryParse(FFTurns.Text, out int ticks) || ticks <= 0) return;

        SetRunState(false);
        Task.Run(() =>
        {
            for (int i = 0; i < ticks; i++)
                ALife.Core.Planet.World.ExecuteOneTurn();
            Dispatcher.UIThread.Post(() =>
            {
                TheWorldCanvas.TurnCount = ALife.Core.Planet.World.Turns;
                TheWorldCanvas.InvalidateVisual();
            });
        });
    }

    public void ShowGeneology_Changed(object sender, RoutedEventArgs args)
    {
        bool show = ShowGeneologyBox.IsChecked == true;
        TheWorldCanvas.SetGeneologyVisible(show);
        if (Vm != null) Vm.ShowGeneology = show;
    }

    public void GoToParent_Click(object sender, RoutedEventArgs args)
    {
        var parent = Vm.SelectedAgent?.Parent;
        if (parent == null || !parent.Alive) return;
        Vm.SelectedAgent = parent;
        TheWorldCanvas.SetSelectedAgent(parent);
    }

    public void PopOutAgentDetails_Click(object sender, RoutedEventArgs args)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime) return;
        if (Vm.SelectedAgent == null) return;

        var detailsVm = new AgentDetailsViewModel(Vm.SelectedAgent);
        var win = new AgentDetailsWindow(detailsVm);
        _childWindows.Add(win);
        win.Closed += (_, _) => _childWindows.Remove(win);
        win.Show();
    }

    public void PopOutBrain_Click(object sender, RoutedEventArgs args)
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime)
            return;

        if (_brainViewer?.IsVisible == true)
        {
            _brainViewer.Activate();
            return;
        }

        _brainViewer = new BrainViewerWindow { DataContext = Vm };
        _childWindows.Add(_brainViewer);
        _brainViewer.Closed += (_, _) => { _childWindows.Remove(_brainViewer!); _brainViewer = null; };
        _brainViewer.Show();
    }

    private void AgentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox cb || cb.SelectedItem is not Agent agent) return;
        Vm.SelectedAgent = agent;
        TheWorldCanvas.SetSelectedAgent(agent);
    }

    private void DescendantComboBox_DropDownOpened(object sender, EventArgs e)
    {
        _wasRunningBeforeDescendantOpen = TheWorldCanvas.IsEnabled;
        Vm.FreezeDescendantUpdates = true;
        SetRunState(false);
    }

    private void DescendantComboBox_DropDownClosed(object sender, EventArgs e)
    {
        Vm.FreezeDescendantUpdates = false;
        if (_wasRunningBeforeDescendantOpen)
            SetRunState(true);
    }

    private void DescendantComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox cb || cb.SelectedItem is not Agent agent) return;
        // Defer until after the SelectionChanged event finishes — the SelectedAgent setter
        // clears DescendantsOfSelected, which crashes if the ComboBox is still mid-event.
        Dispatcher.UIThread.Post(() =>
        {
            Vm.SelectedAgent = agent;
            TheWorldCanvas.SetSelectedAgent(agent);
        });
    }

    private void AgentComboBox_DropDownOpened(object sender, EventArgs e)
    {
        if (!ALife.Core.Planet.HasWorld) return;
        _wasRunningBeforeDescendantOpen = TheWorldCanvas.IsEnabled;
        Vm.FreezeDescendantUpdates = true;
        SetRunState(false);
        var agents = ALife.Core.Planet.World.AllActiveObjects
            .OfType<Agent>()
            .Where(ag => ag.Alive)
            .OrderBy(ag => TheWorldCanvas.GetBirthTurn(ag.IndividualLabel));
        Vm.UpdateAliveAgents(agents);
        if (sender is ComboBox cb)
            cb.SelectedItem = Vm.SelectedAgent;
    }

    private void AgentComboBox_DropDownClosed(object sender, EventArgs e)
    {
        Vm.FreezeDescendantUpdates = false;
        if (_wasRunningBeforeDescendantOpen)
            SetRunState(true);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cts.Cancel();
            TheWorldCanvas.StopAll();
            foreach (var w in _childWindows.ToList())
                w.Close();
            _childWindows.Clear();
            _brainViewer = null;
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private void SetRunState(bool running)
    {
        TheWorldCanvas.IsEnabled = running;
        if (Vm != null) Vm.IsEnabled = running;

        PlayPauseButton.Content = running ? "⏸ Pause" : "▶ Resume";
        OneTurnButton.IsEnabled = !running;
    }

    private void UpdatePerfLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (Vm == null) return;
                    double tps = Vm.TicksPerSecond;
                    double fps = Vm.FramesPerSecond;
                    TpsLabel.Text = tps > 0 ? $"{tps:N0}" : "---";
                    FpsLabel.Text = fps > 0 ? $"{fps:0.00}" : "---";
                });
            }
            catch { }
            Thread.Sleep(500);
        }
    }
}

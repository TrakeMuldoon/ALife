using ALife.Core.WorldObjects.Agents;
using ALife.Rendering;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using AvaloniaUniv.Core.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaUniv.Core.Views;

public partial class SimulatorView : UserControl, IDisposable
{
    private CancellationTokenSource _cts = new();
    private Task? _perfUpdateTask;
    private bool _disposed;
    private BrainViewerWindow? _brainViewer;

    public SimulatorView()
    {
        InitializeComponent();

        // Wire up layers and agent settings once the canvas is ready
        LayersList.ItemsSource = TheWorldCanvas.Simulation.Layers;
        AgentUI.DataContext = TheWorldCanvas.Simulation.AgentUiSettings;

        ZoomBorder.ZoomChanged += (_, _) => UpdateZoomLabel();

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

    public void Pause_Click(object sender, RoutedEventArgs args) => SetRunState(false);
    public void Play_Click(object sender, RoutedEventArgs args) => SetRunState(true);

    public void OneTurn_Click(object sender, RoutedEventArgs args)
    {
        SetRunState(false);
        TheWorldCanvas.ExecuteTick();
        TheWorldCanvas.InvalidateVisual();
    }

    private static readonly double[] SpeedValues = { 1.0, 2.0, 10.0, 30.0, 60.0, 90.0, 120.0, 240.0, double.PositiveInfinity };
    private static readonly string[] SpeedLabels = { "1x", "2x", "10x", "30x", "60x", "90x", "120x", "240x", "∞" };

    public void SpeedSlider_ValueChanged(object sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs args)
    {
        int index = (int)Math.Round(args.NewValue);
        index = Math.Clamp(index, 0, SpeedValues.Length - 1);
        TheWorldCanvas.SetSimulationSpeed(SpeedValues[index]);
        SpeedLabel.Text = SpeedLabels[index];
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
        _brainViewer.Closed += (_, _) => _brainViewer = null;
        _brainViewer.Show();
    }

    private void AgentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox cb) return;
        if (cb.SelectedItem is not Agent agent) return;

        Vm.SelectedAgent = agent;
        TheWorldCanvas.SetSelectedAgent(agent);
    }

    private void AgentComboBox_DropDownOpened(object sender, EventArgs e)
    {
        if (!ALife.Core.Planet.HasWorld) return;
        var agents = ALife.Core.Planet.World.AllActiveObjects
            .OfType<Agent>()
            .Where(ag => ag.Alive);
        Vm.UpdateAliveAgents(agents);
        if (sender is ComboBox cb)
            cb.SelectedItem = Vm.SelectedAgent;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _cts.Cancel();
            TheWorldCanvas.Timer?.Stop();
            _brainViewer?.Close();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    private void SetRunState(bool running)
    {
        TheWorldCanvas.IsEnabled = running;
        if (Vm != null) Vm.IsEnabled = running;

        PauseButton.IsEnabled = running;
        PlayButton.IsEnabled = !running;
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
                    if (Vm != null)
                        PerformanceLabel.Text = Vm.PerformancePerTickLabel;
                });
            }
            catch { }
            Thread.Sleep(500);
        }
    }
}

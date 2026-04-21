using ALife.Rendering;
using Avalonia.Controls;
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

    public SimulatorView()
    {
        InitializeComponent();

        // Wire up layers and agent settings once the canvas is ready
        LayersList.ItemsSource = TheWorldCanvas.Simulation.Layers;
        AgentUI.DataContext = TheWorldCanvas.Simulation.AgentUiSettings;

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

    public void Speed_Click(object sender, RoutedEventArgs args)
    {
        if (sender is Button b && int.TryParse(b.Tag?.ToString(), out int speed))
        {
            TheWorldCanvas.SetSimulationSpeed(speed);
        }
    }

    public void SpeedMax_Click(object sender, RoutedEventArgs args)
    {
        TheWorldCanvas.SetSimulationSpeed((int)SimulationSpeed.VeryVeryVeryFast);
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

    public void Dispose()
    {
        if (!_disposed)
        {
            _cts.Cancel();
            TheWorldCanvas.Timer?.Stop();
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

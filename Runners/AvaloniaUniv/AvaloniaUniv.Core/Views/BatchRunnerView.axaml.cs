using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaUniv.Core.ViewModels;

namespace AvaloniaUniv.Core.Views;

public partial class BatchRunnerView : UserControl
{
    public BatchRunnerView()
    {
        InitializeComponent();
    }

    private BatchRunnerViewModel Vm => (BatchRunnerViewModel)DataContext!;

    public void ReturnToLauncher_Click(object sender, RoutedEventArgs args)
    {
        Vm.StopRunner();
        var windowVm = (MainWindowViewModel)Parent!.DataContext!;
        windowVm.CurrentViewModel = new LauncherViewModel();
    }

    public void Start_Click(object sender, RoutedEventArgs args) => Vm.StartRunner();
    public void Stop_Click(object sender, RoutedEventArgs args) => Vm.StopRunner();
    public void Restart_Click(object sender, RoutedEventArgs args) => Vm.StartRunner();
}

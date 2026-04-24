using ALife.Avalonia.ViewModels;

namespace ALife.Avalonia.ALifeImplementations;

public class SeedLogger(BatchRunnerViewModel vm) : AvaloniaLogger(vm)
{
    protected override void WriteInternal(string message)
    {
        _vm.SeedLog += message;
    }
}

using ALife.Avalonia.ViewModels;

namespace ALife.Avalonia.ALifeImplementations;

public class ConsoleLogger(BatchRunnerViewModel vm) : AvaloniaLogger(vm)
{
    protected override void WriteInternal(string message)
    {
        _vm.ConsoleLog += message;
    }
}

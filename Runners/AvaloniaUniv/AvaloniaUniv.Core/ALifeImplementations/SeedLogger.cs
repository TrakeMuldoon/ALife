using AvaloniaUniv.Core.ViewModels;

namespace AvaloniaUniv.Core.ALifeImplementations;

public class SeedLogger(BatchRunnerViewModel vm) : AvaloniaLogger(vm)
{
    protected override void WriteInternal(string message)
    {
        _vm.SeedLog += message;
    }
}

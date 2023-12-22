using ALife.Avalonia.ViewModels;

namespace ALife.Avalonia.ALifeImplementations
{
    /// <summary>
    /// Defines the logger used for outputting to the Seed Textbox
    /// </summary>
    /// <seealso cref="ALife.Avalonia.ALifeImplementations.AvaloniaLogger"/>
    public class SeedLogger(BatchRunnerViewModel vm) : AvaloniaLogger(vm)
    {
        /// <summary>
        /// Writes the message to the actual output.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void WriteInternal(string message)
        {
            _vm.SeedLog += message;
        }
    }
}

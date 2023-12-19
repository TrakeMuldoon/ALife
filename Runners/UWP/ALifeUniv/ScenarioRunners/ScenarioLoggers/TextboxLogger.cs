using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ALifeUni.ScenarioRunners.ScenarioLoggers
{
    /// <summary>
    /// A logger for writing to a UI textbox
    /// </summary>
    /// <seealso cref="ALifeUni.ScenarioRunners.Logger"/>
    public class TextboxLogger : Logger
    {
        /// <summary>
        /// The console box
        /// </summary>
        private readonly TextBox consoleBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextboxLogger"/> class.
        /// </summary>
        /// <param name="consoleBox">The console box.</param>
        public TextboxLogger(TextBox consoleBox)
        {
            this.consoleBox = consoleBox;
        }

        /// <summary>
        /// Writes the message to the actual output.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void WriteInternal(string message)
        {
            Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                consoleBox.Text += message;
                ScrollToBottom(consoleBox);
            }).AsTask().Wait();
        }

        /// <summary>
        /// Scrolls to bottom.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        private void ScrollToBottom(TextBox textBox)
        {
            var grid = (Grid)VisualTreeHelper.GetChild(textBox, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer))
                {
                    continue;
                }
                _ = ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f, true);
                break;
            }
        }
    }
}

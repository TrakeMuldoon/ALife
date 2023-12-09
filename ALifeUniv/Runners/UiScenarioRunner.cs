using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace ALifeUni.Runners
{
    public class UiScenarioRunner : AbstractScenarioRunner
    {
        private TextBox consoleBox;
        private ConcurrentQueue<string> messageQueue;

        public UiScenarioRunner(TextBox consoleBox)
        {
            this.consoleBox = consoleBox;
            messageQueue = new ConcurrentQueue<string>();
            Task writeMessages = new Task(() => WriteInternal());
            writeMessages.Start();
        }

        protected override bool ShouldStopRunner()
        {
            return true;
        }

        protected override void StopRunner()
        {
            // blank.
        }

        protected override void Write(string message)
        {
            messageQueue.Enqueue(message);
        }

        private async void WriteInternal()
        {
            while(true)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    while(messageQueue.TryDequeue(out string message))
                    {
                        consoleBox.Text += message;
                    }
                });
            }
        }
    }
}

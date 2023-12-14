/*
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace ALifeUni.ScenarioRunners
{
    /// <summary>
    /// A ScenarioRunner that writes to a UI textbox
    /// </summary>
    /// <seealso cref="ALifeUni.ScenarioRunners.AbstractScenarioRunner"/>
    /// <seealso cref="System.IDisposable"/>
    public class UiScenarioRunnerOld : AbstractScenarioRunner, IDisposable
    {
        /// <summary>
        /// The console box
        /// </summary>
        private readonly TextBox consoleBox;

        /// <summary>
        /// The message queue
        /// </summary>
        private readonly ConcurrentQueue<string> messageQueue;

        /// <summary>
        /// Flag to determine if we are running or not
        /// </summary>
        private bool running = true;

        /// <summary>
        /// The write messages
        /// </summary>
        private Task writeMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="UiScenarioRunnerOld"/> class.
        /// </summary>
        /// <param name="consoleBox">The console box.</param>
        public UiScenarioRunnerOld(TextBox consoleBox)
        {
            this.consoleBox = consoleBox;
            Start();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is logger stopped.
        /// </summary>
        /// <value><c>true</c> if this instance is logger stopped; otherwise, <c>false</c>.</value>
        public bool IsLoggerStopped => writeMessages.IsCompleted;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            Dispose(true);
            running = false;
            while (!writeMessages.IsCompleted)
            {
                Thread.Sleep(100);
            }
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void StartLogger()
        {
            messageQueue.Clear();
            running = true;
            writeMessages = new Task(() => WriteInternal());
            writeMessages.Start();
        }

        /// <summary>
        /// Stops the logger asynchronous.
        /// </summary>
        public async Task StopLoggerAsync()
        {
            running = false;
            while (!writeMessages.IsCompleted)
            {
                Thread.Sleep(100);
            }

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                while (messageQueue.TryDequeue(out var message))
                {
                    consoleBox.Text += message;
                }
            });
        }

        /// <summary>
        /// Checks if we should stop the runner or not
        /// </summary>
        /// <returns>True to stop runner, false otherwise</returns>
        protected override bool ShouldStopRunner()
        {
            return true;
        }

        /// <summary>
        /// Stops the runner.
        /// </summary>
        protected override void StopRunner()
        {
            StopLoggerAsync().Wait();
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected override void Write(string message)
        {
            messageQueue.Enqueue(message);
        }

        /// <summary>
        /// Writes the internal.
        /// </summary>
        private async void WriteInternal()
        {
            while (running)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    while (messageQueue.TryDequeue(out var message))
                    {
                        consoleBox.Text += message;
                    }
                });
            }
        }
    }
}
*/

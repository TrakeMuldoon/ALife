using System;
using System.Threading;
using System.Threading.Tasks;
using ALife.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace ALife.Avalonia.Controls.SingularRunnerControls
{
    public partial class SingularRunnerTopBar : UserControl, IDisposable
    {
        /// <summary>
        /// The text update thread cancellation token
        /// </summary>
        private CancellationTokenSource _cancellationToken;

        /// <summary>
        /// The update task
        /// </summary>
        private Task _updateTask;

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularRunnerTopBar"/> class.
        /// </summary>
        public SingularRunnerTopBar()
        {
            InitializeComponent();

            string baseNum = "---";
            string startingSpaces = new string(' ', SingularRunnerViewModel.MAX_CHARACTERS_FOR_TICKS - baseNum.Length);

            string newLabel = $"TPS: {startingSpaces}{baseNum} | FPS: {startingSpaces}{baseNum}";
            Performance.Text = newLabel;

            _cancellationToken = new CancellationTokenSource();
            CancellationToken ct = _cancellationToken.Token;
            _updateTask = Task.Run(() => UpdateTask(ct), ct);
        }

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>The view model.</value>
        public SingularRunnerViewModel ViewModel => (SingularRunnerViewModel)DataContext;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles the Click event of the ReturntoLauncher control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void ReturntoLauncher_Click(object sender, RoutedEventArgs args)
        {
            MainWindowViewModel? windowMvm = (MainWindowViewModel)Parent.Parent.Parent.DataContext;
            windowMvm.CurrentViewModel = new LauncherViewModel();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                if(disposing)
                {
                    _cancellationToken.Cancel();
                    while(!_updateTask.IsCanceled && !_updateTask.IsCompleted && !_updateTask.IsFaulted)
                    {
                        Thread.Sleep(100);
                    }
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Updates the task.
        /// </summary>
        /// <param name="token">The token.</param>
        private void UpdateTask(CancellationToken token)
        {
            while(true)
            {
                if(token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                // With bindings, TPS updates, but for some reason FPS does _not_ update. So we're doing it manually
                try
                {
                    // Start the job on the ui thread and return immediately.
                    Dispatcher.UIThread.Post(() => UpdateTextboxes());

                    // Start the job on the ui thread and wait for the result.
                    Dispatcher.UIThread.InvokeAsync(UpdateTextboxes).Wait();

                    // This invocation would cause an exception because we are running on a worker thread:
                    // System.InvalidOperationException: 'Call from invalid thread'
                }
                catch(Exception)
                {
                    throw; // Todo: Handle exception.
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Updates the textboxes.
        /// </summary>
        private void UpdateTextboxes()
        {
            // We can update with bindings, but then it is illegible since it updates so frequently. So doing it
            // manually instead.
            this.Performance.Text = ViewModel?.PerformancePerTickLabel;
        }
    }
}

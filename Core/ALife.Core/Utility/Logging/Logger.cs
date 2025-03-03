using Microsoft.VisualBasic;
using System.Collections.Concurrent;
using System.Text;

namespace ALife.Core.Utility.Logging
{
    /// <summary>
    /// Defines a logger class.
    /// </summary>
    public class Logger : IDisposable
    {
        /// <summary>
        /// The should stop
        /// </summary>
        protected bool shouldStop = false;

        /// <summary>
        /// The maximum messages
        /// </summary>
        private const int MaxMessages = 5;

        /// <summary>
        /// The message queue
        /// </summary>
        private ConcurrentQueue<string> messageQueue = new();

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// The task to write messages
        /// </summary>
        private Task? task = null;
        
        /// <summary>
        /// Occurs whenever there is a message to be logged.
        /// </summary>
        public EventHandler<LogMessageEventArgs>? LogMessage = null;

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value><c>true</c> if this instance is running; otherwise, <c>false</c>.</value>
        public bool IsRunning => (!task?.IsCompleted ?? false) && (!task?.IsCanceled ?? false);

        /// <summary>
        /// Gets a value indicating whether this instance is stopped.
        /// </summary>
        /// <value><c>true</c> if this instance is stopped; otherwise, <c>false</c>.</value>
        public bool IsStopped => !IsRunning;

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
        /// Starts the logger.
        /// </summary>
        public void StartLogger(CancellationToken token)
        {
            if(IsRunning)
            {
                StopLogger(true);
            }
            shouldStop = false;
            messageQueue = new ConcurrentQueue<string>();
            task = new Task(() => WriteManager(token), token);
            task.Start();
        }

        /// <summary>
        /// Stops the logger.
        /// </summary>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        public void StopLogger(bool wait = false)
        {
            //shouldStop = true;
            messageQueue = new ConcurrentQueue<string>();

            while(IsRunning && wait)
            {
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Write(string message)
        {
            if(IsRunning)
            {
                messageQueue.Enqueue(message);
            }
        }

        /// <summary>
        /// Writes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Write(int message)
        {
            Write(message.ToString());
        }

        /// <summary>
        /// The thread controlling when messages are logged.
        /// </summary>
        private void WriteManager(CancellationToken ct)
        {
            while(true)
            {
                if(ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
                string? nextMessage = DequeueMessages();
                if(nextMessage != null)
                {
                    LogMessage?.Invoke(this, new LogMessageEventArgs(message: nextMessage));
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Dequeues the messages from the queue.
        /// </summary>
        /// <returns>The next message to log, or null.</returns>
        private string? DequeueMessages()
        {
            if(messageQueue.Count > 0)
            {
                StringBuilder sb = new();
                for(int i = 0; i < MaxMessages; i++)
                {
                    if(messageQueue.Count > 1 && messageQueue.TryDequeue(out string message))
                    {
                        _ = sb.Append(message);
                    }
                    else
                    {
                        break;
                    }
                }
                return sb.ToString();
            }
            
            return null;
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLine(string message)
        {
            Write($"{message}{Environment.NewLine}");
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteLine(int message)
        {
            Write($"{message}{Environment.NewLine}");
        }

        /// <summary>
        /// Writes the line seperator.
        /// </summary>
        /// <param name="numLines">The number lines.</param>
        public void WriteLineSeperator(int numLines)
        {
            for(int i = 0; i < numLines; i++)
            {
                Write($"{LoggingConstants.LINE_SEPERATOR}{Environment.NewLine}");
            }
        }

        /// <summary>
        /// Writes the new line.
        /// </summary>
        /// <param name="numLines">The number lines.</param>
        public void WriteNewLine(int numLines)
        {
            for(int i = 0; i < numLines; i++)
            {
                Write($" {Environment.NewLine}");
            }
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
                    StopLogger(true);
                }

                disposedValue = true;
            }
        }
    }
}
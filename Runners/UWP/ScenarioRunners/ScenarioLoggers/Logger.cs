using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ALifeUni.ScenarioRunners.ScenarioLoggers
{
    public abstract class Logger : IDisposable
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
        private readonly ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// The task to write messages
        /// </summary>
        private Task task = null;

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
            messageQueue.Clear();
            task = new Task(() => WriteController(token), token);
            task.Start();
        }

        /// <summary>
        /// Stops the logger.
        /// </summary>
        /// <param name="wait">if set to <c>true</c> [wait].</param>
        public void StopLogger(bool wait = false)
        {
            //shouldStop = true;
            messageQueue.Clear();

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
        /// Controller handling how long we should attempt to write messages
        /// </summary>
        public void WriteController(CancellationToken ct)
        {
            while(true)
            {
                if(ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < MaxMessages; i++)
                {
                    if(messageQueue.TryDequeue(out string message))
                    {
                        _ = sb.Append(message);
                    }
                    else
                    {
                        break;
                    }
                }

                string output = sb.ToString();
                if(!string.IsNullOrWhiteSpace(output))
                {
                    WriteInternal(output);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
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
                Write($"{Constants.LINE_SEPERATOR}{Environment.NewLine}");
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

        /// <summary>
        /// Writes the message to the actual output.
        /// </summary>
        /// <param name="message">The message.</param>
        protected abstract void WriteInternal(string message);
    }
}

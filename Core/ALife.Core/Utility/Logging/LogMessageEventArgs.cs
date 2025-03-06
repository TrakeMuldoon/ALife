namespace ALife.Core.Utility.Logging;

/// <summary>
/// Arguments for when an event to log a message is raised.
/// </summary>
public class LogMessageEventArgs(string message) : EventArgs
{
    /// <summary>
    /// The message to log.
    /// </summary>
    public string Message = message;
}

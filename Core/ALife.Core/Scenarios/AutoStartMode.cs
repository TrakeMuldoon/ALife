namespace ALife.Core.Scenarios
{
    /// <summary>
    /// The auto start mode. Only one scenario can be registered with either AutoStartVisual or AutoStartConsole
    /// </summary>
    public enum AutoStartMode
    {
        /// <summary>
        /// Do not automatically start a scenario
        /// </summary>
        None,

        /// <summary>
        /// Automatically start the visual interface
        /// </summary>
        AutoStartVisual,

        /// <summary>
        /// Automatically start the console interface
        /// </summary>
        AutoStartConsole,
    }
}

namespace ALife.Rendering
{
    /// <summary>
    /// Agent UI Settings
    /// </summary>
    public class AgentUISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether [show senses].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show senses]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSenses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show sense bounding boxes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show sense bounding boxes]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSenseBoundingBoxes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentUISettings"/> class.
        /// </summary>
        public AgentUISettings() { }
    }
}

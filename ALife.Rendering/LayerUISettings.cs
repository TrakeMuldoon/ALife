using ALife.Core;
using System.Diagnostics;

namespace ALife.Rendering
{
    /// <summary>
    /// Settings for a UI Layer
    /// </summary>
    [DebuggerDisplay("{LayerName}:{ShowLayer}")]
    public class LayerUISettings
    {
        /// <summary>
        /// Gets or sets the name of the layer.
        /// </summary>
        /// <value>
        /// The name of the layer.
        /// </value>
        public string LayerName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show objects].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show objects]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowObjects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show bounding boxes].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show bounding boxes]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowBoundingBoxes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerUISettings"/> class.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        public LayerUISettings(string layerName) : this(layerName, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerUISettings"/> class.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="showLayer">if set to <c>true</c> [show layer].</param>
        public LayerUISettings(string layerName, bool showLayer) : this(layerName, showLayer, showLayer) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerUISettings"/> class.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="showObjects">if set to <c>true</c> [show objects].</param>
        /// <param name="showBoundingBoxes">if set to <c>true</c> [show bounding boxes].</param>
        public LayerUISettings(string layerName, bool showObjects, bool showBoundingBoxes)
        {
            LayerName = layerName;
            ShowObjects = showObjects;
            ShowBoundingBoxes = showBoundingBoxes;
        }

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        /// <returns></returns>
        public static List<LayerUISettings> GetDefaultSettings()
        {
            List<LayerUISettings> settingsList = new List<LayerUISettings>
            {
                new LayerUISettings(ReferenceValues.CollisionLevelZone, true),
                new LayerUISettings(ReferenceValues.CollisionLevelPhysical, true),
                new LayerUISettings(ReferenceValues.CollisionLevelSound),
                new LayerUISettings(ReferenceValues.CollisionLevelDead)
            };

            return settingsList;
        }
    }
}

using System.Text.Json.Serialization;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// Information on how to render the component.
    /// </summary>
    public struct RenderComponentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponentInfo"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <param name="debugColour">The debug colour.</param>
        /// <param name="drawComponent">The draw component.</param>
        [JsonConstructor]
        public RenderComponentInfo(Colour colour, Colour debugColour, RenderComponent drawComponent)
        {
            Colour = colour;
            DebugColour = debugColour;
            DrawComponent = drawComponent;
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        Colour Colour { get; set; }

        /// <summary>
        /// Gets or sets the debug colour.
        /// </summary>
        /// <value>The debug colour.</value>
        Colour DebugColour { get; set; }

        /// <summary>
        /// Gets the draw component.
        /// </summary>
        /// <value>The draw component.</value>
        RenderComponent DrawComponent { get; }
    }
}

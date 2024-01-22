using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry
{
    /// <summary>
    /// The render components for a shape.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct ShapeRenderComponents
    {
        /// <summary>
        /// The fill component
        /// </summary>
        private ShapeRenderComponent _fillComponent;

        /// <summary>
        /// The outline component
        /// </summary>
        private ShapeRenderComponent _outlineComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeRenderComponents"/> struct.
        /// </summary>
        /// <param name="fillComponent">The fill component.</param>
        /// <param name="outlineComponent">The outline component.</param>
        [JsonConstructor]
        public ShapeRenderComponents(ShapeRenderComponent fillComponent, ShapeRenderComponent outlineComponent)
        {
            _fillComponent = fillComponent;
            _outlineComponent = outlineComponent;
        }

        /// <summary>
        /// Gets or sets the fill component.
        /// </summary>
        /// <value>The fill component.</value>
        [JsonIgnore]
        public ShapeRenderComponent FillComponent
        {
            get => _fillComponent;
            set => _fillComponent = value;
        }

        /// <summary>
        /// Gets or sets the outline component.
        /// </summary>
        /// <value>The outline component.</value>
        [JsonIgnore]
        public ShapeRenderComponent OutlineComponent
        {
            get => _outlineComponent;
            set => _outlineComponent = value;
        }

        /// <summary>
        /// Sets the fill colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillColour(Colour colour)
        {
            _fillComponent.Colour = colour;
        }

        /// <summary>
        /// Sets the fill debug colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillDebugColour(Colour colour)
        {
            _fillComponent.DebugColour = colour;
        }

        /// <summary>
        /// Sets the outline colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetOutlineColour(Colour colour)
        {
            _outlineComponent.Colour = colour;
        }

        /// <summary>
        /// Sets the outline debug colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetOutlineDebugColour(Colour colour)
        {
            _outlineComponent.DebugColour = colour;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"FillComponent={FillComponent}, OutlineComponent={OutlineComponent}";
        }
    }
}

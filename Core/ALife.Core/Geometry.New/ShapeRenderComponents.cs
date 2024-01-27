using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Geometry.New
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
        [JsonIgnore]
        private ShapeRenderComponent _fillComponent;

        /// <summary>
        /// The outline component
        /// </summary>
        [JsonIgnore]
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
        [JsonPropertyName("fillComponent")]
        public ShapeRenderComponent FillComponent
        {
            get => _fillComponent;
            set => _fillComponent = value;
        }

        /// <summary>
        /// Gets or sets the outline component.
        /// </summary>
        /// <value>The outline component.</value>
        [JsonPropertyName("outlineComponent")]
        public ShapeRenderComponent OutlineComponent
        {
            get => _outlineComponent;
            set => _outlineComponent = value;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ShapeRenderComponents left, ShapeRenderComponents right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ShapeRenderComponents left, ShapeRenderComponents right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is ShapeRenderComponents components &&
                components.FillComponent == FillComponent &&
                components.OutlineComponent == OutlineComponent;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(FillComponent, OutlineComponent);
        }

        /// <summary>
        /// Sets the fill colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillColour(IColour colour)
        {
            _fillComponent.SetColour(colour);
        }

        /// <summary>
        /// Sets the fill debug colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillDebugColour(IColour colour)
        {
            _fillComponent.SetDebugColour(colour);
        }

        /// <summary>
        /// Sets the outline colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetOutlineColour(IColour colour)
        {
            _outlineComponent.SetColour(colour);
        }

        /// <summary>
        /// Sets the outline debug colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetOutlineDebugColour(IColour colour)
        {
            _outlineComponent.SetDebugColour(colour);
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
using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;

namespace ALife.Core.Geometry.New
{
    /// <summary>
    /// Information on how to render the component.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct ShapeRenderComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeRenderComponent"/> struct.
        /// </summary>
        /// <param name="colour">The colour.</param>
        /// <param name="debugColour">The debug colour.</param>
        [JsonConstructor]
        public ShapeRenderComponent(IColour colour, IColour debugColour)
        {
            Colour = colour;
            DebugColour = debugColour;
        }

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        [JsonPropertyName("colour")]
        public IColour Colour { get; private set; }

        /// <summary>
        /// Gets or sets the debug colour.
        /// </summary>
        /// <value>The debug colour.</value>
        [JsonPropertyName("debugColour")]
        public IColour DebugColour { get; private set; }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ShapeRenderComponent left, ShapeRenderComponent right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ShapeRenderComponent left, ShapeRenderComponent right)
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
            return obj is ShapeRenderComponent component &&
                component.Colour == Colour &&
                component.DebugColour == DebugColour;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(Colour, DebugColour);
        }

        /// <summary>
        /// Sets the colour.
        /// </summary>
        /// <param name="colour">The new colour.</param>
        public void SetColour(IColour colour)
        {
            Colour = colour;
        }

        /// <summary>
        /// Sets the debug colour.
        /// </summary>
        /// <param name="colour">The new debug colour.</param>
        public void SetDebugColour(IColour colour)
        {
            DebugColour = colour;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Colour={Colour}, DebugColour={DebugColour}";
        }
    }
}
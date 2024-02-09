using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.Utility;
using ALife.Core.Utility.Colours;

namespace ALife.Core.CollisionDetection.Geometry
{
    /// <summary>
    /// Represents a layer of colouration for a shape.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public struct ColourationLayer
    {
        /// <summary>
        /// The fill colour
        /// </summary>
        [JsonIgnore]
        private IColour _fillColour;

        /// <summary>
        /// The outline colour
        /// </summary>
        [JsonIgnore]
        private IColour _outlineColour;

        /// <summary>
        /// The outline stroke width
        /// </summary>
        [JsonIgnore]
        private double _outlineStrokeWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColourationLayer"/> struct.
        /// </summary>
        /// <param name="fillColour">The fill colour.</param>
        /// <param name="outlineColour">The outline colour.</param>
        /// <param name="outlineStrokeWidth">Width of the outline stroke.</param>
        [JsonConstructor]
        public ColourationLayer(IColour fillColour, IColour outlineColour, double outlineStrokeWidth)
        {
            _fillColour = fillColour;
            _outlineColour = outlineColour;
            _outlineStrokeWidth = outlineStrokeWidth;
        }

        /// <summary>
        /// Gets the fill colour.
        /// </summary>
        /// <value>The fill colour.</value>
        [JsonPropertyName("fillColour")]
        public IColour FillColour => _fillColour;

        /// <summary>
        /// Gets the outline colour.
        /// </summary>
        /// <value>The outline colour.</value>
        [JsonPropertyName("outlineColour")]
        public IColour OutlineColour => _outlineColour;

        /// <summary>
        /// Gets the width of the outline stroke.
        /// </summary>
        /// <value>The width of the outline stroke.</value>
        [JsonPropertyName("outlineStrokeWidth")]
        public double OutlineStrokeWidth => _outlineStrokeWidth;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCodeHelper.Combine(_fillColour, _outlineColour, _outlineStrokeWidth);
        }

        /// <summary>
        /// Updates the fill colour.
        /// </summary>
        /// <param name="fillColour">The fill colour.</param>
        public void SetFillColour(IColour fillColour)
        {
            _fillColour = fillColour;
        }

        /// <summary>
        /// Sets the layer.
        /// </summary>
        /// <param name="fillColour">The fill colour.</param>
        /// <param name="outlineColour">The outline colour.</param>
        /// <param name="outlineStrokeWidth">Width of the outline stroke.</param>
        public void SetLayer(IColour fillColour, IColour outlineColour, double outlineStrokeWidth)
        {
            _fillColour = fillColour;
            _outlineColour = outlineColour;
            _outlineStrokeWidth = outlineStrokeWidth;
        }

        /// <summary>
        /// Updates the outline colour.
        /// </summary>
        /// <param name="outlineColour">The outline colour.</param>
        public void SetOutlineColour(IColour outlineColour)
        {
            _outlineColour = outlineColour;
        }

        /// <summary>
        /// Updates the width of the outline stroke.
        /// </summary>
        /// <param name="outlineStrokeWidth">Width of the outline stroke.</param>
        public void SetOutlineStrokeWidth(double outlineStrokeWidth)
        {
            _outlineStrokeWidth = outlineStrokeWidth;
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Fill: {_fillColour}, Outline: {_outlineColour}, Width: {_outlineStrokeWidth}";
        }
    }
}

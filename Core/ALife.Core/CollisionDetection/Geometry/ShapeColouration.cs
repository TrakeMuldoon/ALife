using ALife.Core.Utility.Colours;

namespace ALife.Core.CollisionDetection.Geometry
{
    /// <summary>
    /// Defines the colouration of a shape. This is used in rendering and with some agent behaviours.
    /// </summary>
    public struct ShapeColouration
    {
        /// <summary>
        /// The colouration
        /// </summary>
        private ColourationLayer _colouration;

        /// <summary>
        /// The debug colouration
        /// </summary>
        private ColourationLayer _debugColouration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeColouration"/> struct.
        /// </summary>
        /// <param name="colouration">The colouration.</param>
        /// <param name="debugColouration">The debug colouration.</param>
        public ShapeColouration(ColourationLayer colouration, ColourationLayer debugColouration)
        {
            _colouration = colouration;
            _debugColouration = debugColouration;
        }

        /// <summary>
        /// Gets the colouration.
        /// </summary>
        /// <value>The colouration.</value>
        public ColourationLayer Colouration => _colouration;

        /// <summary>
        /// Gets the debug colouration.
        /// </summary>
        /// <value>The debug colouration.</value>
        public ColourationLayer DebugColouration => _debugColouration;

        /// <summary>
        /// Sets the colouration layer.
        /// </summary>
        /// <param name="colouration">The colouration.</param>
        public void SetColourationLayer(ColourationLayer colouration)
        {
            _colouration = colouration;
        }

        /// <summary>
        /// Sets the debug colouration layer.
        /// </summary>
        /// <param name="colouration">The colouration.</param>
        public void SetDebugColourationLayer(ColourationLayer colouration)
        {
            _debugColouration = colouration;
        }

        /// <summary>
        /// Sets the debug fill colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetDebugFillColour(IColour colour)
        {
            _debugColouration.SetFillColour(colour);
        }

        /// <summary>
        /// Sets the debug outline colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetDebugOutlineColour(IColour colour)
        {
            _debugColouration.SetOutlineColour(colour);
        }

        /// <summary>
        /// Sets the width of the debug outline stroke.
        /// </summary>
        /// <param name="width">The width.</param>
        public void SetDebugOutlineStrokeWidth(double width)
        {
            _debugColouration.SetOutlineStrokeWidth(width);
        }

        /// <summary>
        /// Sets the fill colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillColour(IColour colour)
        {
            _colouration.SetFillColour(colour);
        }

        /// <summary>
        /// Sets the outline colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetOutlineColour(IColour colour)
        {
            _colouration.SetOutlineColour(colour);
        }

        /// <summary>
        /// Sets the width of the outline stroke.
        /// </summary>
        /// <param name="width">The width.</param>
        public void SetOutlineStrokeWidth(double width)
        {
            _colouration.SetOutlineStrokeWidth(width);
        }
    }
}

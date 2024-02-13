using ALife.Core.CommonInterfaces;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Contains information about the colour of a shape.
    /// </summary>
    /// <seealso cref="ALife.Core.CommonInterfaces.IDeepCloneable&lt;ALife.Core.Shapes.ColourInfo&gt;"/>
    public struct ColourInfo : IDeepCloneable<ColourInfo>
    {
        /// <summary>
        /// The border colour
        /// </summary>
        public IColour BorderColour;

        /// <summary>
        /// The border width
        /// </summary>
        public double BorderWidth;

        /// <summary>
        /// The fill colour
        /// </summary>
        public IColour FillColour;

        /// <summary>
        /// The render border
        /// </summary>
        public bool RenderBorder;

        /// <summary>
        /// The render fill
        /// </summary>
        public bool RenderFill;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColourInfo"/> struct.
        /// </summary>
        /// <param name="_">if set to <c>true</c> [].</param>
        public ColourInfo(bool _ = false)
        {
            FillColour = Colour.White;
            BorderColour = Colour.Black;
            BorderWidth = 0;
            RenderBorder = false;
            RenderFill = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColourInfo"/> struct.
        /// </summary>
        /// <param name="fillColour">The fill colour.</param>
        /// <param name="borderColour">The border colour.</param>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="renderFill">if set to <c>true</c> [render fill].</param>
        /// <param name="renderBorder">if set to <c>true</c> [render border].</param>
        public ColourInfo(IColour fillColour, IColour borderColour, double borderWidth, bool renderFill, bool renderBorder)
        {
            FillColour = fillColour;
            BorderColour = borderColour;
            BorderWidth = borderWidth;
            RenderFill = renderFill;
            RenderBorder = renderBorder;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColourInfo"/> struct.
        /// </summary>
        /// <param name="colourInfo">The colour information.</param>
        public ColourInfo(ColourInfo colourInfo)
        {
            FillColour = colourInfo.FillColour;
            BorderColour = colourInfo.BorderColour;
            BorderWidth = colourInfo.BorderWidth;
            RenderFill = colourInfo.RenderFill;
            RenderBorder = colourInfo.RenderBorder;
        }

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public ColourInfo Clone()
        {
            return new ColourInfo(this);
        }

        /// <summary>
        /// Sets the border colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetBorderColour(IColour colour)
        {
            BorderColour = colour;
        }

        /// <summary>
        /// Sets the width of the border.
        /// </summary>
        /// <param name="width">The width.</param>
        public void SetBorderWidth(double width)
        {
            BorderWidth = width;
        }

        /// <summary>
        /// Sets the fill colour.
        /// </summary>
        /// <param name="colour">The colour.</param>
        public void SetFillColour(IColour colour)
        {
            FillColour = colour;
        }

        /// <summary>
        /// Sets the render border.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        public void SetRenderBorder(bool render)
        {
            RenderBorder = render;
        }

        /// <summary>
        /// Sets the render fill.
        /// </summary>
        /// <param name="render">if set to <c>true</c> [render].</param>
        public void SetRenderFill(bool render)
        {
            RenderFill = render;
        }
    }
}

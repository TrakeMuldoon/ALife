using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;
using ALife.Core.Utility;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// Defines a circle.
    /// </summary>
    /// <seealso cref="Shape"/>
    [DebuggerDisplay("{ToString()}")]
    public class Circle : Shape
    {
        /// <summary>
        /// The radius
        /// </summary>
        private double _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="fillColour">The fill colour.</param>
        /// <param name="fillDebugColour">The fill debug colour.</param>
        /// <param name="outlineColour">The outline colour.</param>
        /// <param name="outlineDebugColour">The outline debug colour.</param>
        public Circle(double x, double y, double radius, Angle orientation, Colour fillColour, Colour fillDebugColour, Colour outlineColour, Colour outlineDebugColour) : base(x, y, orientation, new ShapeRenderComponent(fillColour, fillDebugColour), new ShapeRenderComponent(outlineColour, outlineDebugColour))
        {
            _radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="orientation">The orientation.</param>
        public Circle(double x, double y, double radius, Nullable<Angle> orientation = null) : this(x, y, radius, orientation ?? Angle.Zero, DefinedColours.White, DefinedColours.White, DefinedColours.White, DefinedColours.White)
        {
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        [JsonIgnore]
        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                RecalculateShapeData();
            }
        }

        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public override Shape CloneInstance()
        {
            Circle newInstance = new Circle(CentrePoint.X, CentrePoint.Y, Radius, Orientation, RenderComponents.FillComponent.Colour, RenderComponents.FillComponent.DebugColour, RenderComponents.OutlineComponent.Colour, RenderComponents.OutlineComponent.DebugColour);
            return newInstance;
        }

        /// <summary>
        /// Triggers the recalculations of shape data for the current instance.
        /// </summary>
        public override void RecalculateOwnShapeData()
        {
            Point actualCentrePoint = _centrePoint;
            // If this shape has a parent, then the centre point is relative to the parent's centre point. And the
            // current instance's orientation relative to the parent's orientation.
            if(Parent != null)
            {
                Matrix translationMatrix = Matrix.CreateFromTranslationAndAngle(Orientation, _centrePoint);
                actualCentrePoint = Point.FromTransformation(actualCentrePoint, translationMatrix);
            }

            // For a circle, the bounding box is the same as the axis aligned bounding box
            _boundingBox = new BoundingBox(actualCentrePoint.X - Radius, actualCentrePoint.Y - Radius, actualCentrePoint.X + Radius, actualCentrePoint.Y + Radius);
            _axisAlignedBoundingBox = _boundingBox;
        }
    }
}

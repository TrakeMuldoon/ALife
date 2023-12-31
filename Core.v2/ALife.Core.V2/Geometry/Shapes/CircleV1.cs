using System.Collections.Generic;
using System.Diagnostics;
using ALife.Core.Utility.Colours;

namespace ALife.Core.Geometry.Shapes
{
    /// <summary>
    /// Defines a circle.
    /// </summary>
    /// <seealso cref="ALife.Core.Geometry.IShape"/>
    [DebuggerDisplay("{ToString()}")]
    public class CircleV1 : IShape
    {
        /// <summary>
        /// The angle
        /// </summary>
        private Angle _angle;

        /// <summary>
        /// The bounding box
        /// </summary>
        private BoundingBox _boundingBox;

        /// <summary>
        /// The centre point
        /// </summary>
        private Point _centrePoint;

        /// <summary>
        /// The child shapes
        /// </summary>
        private List<IShape> _childShapes;

        /// <summary>
        /// The parent shape
        /// </summary>
        private IShape _parentShape;

        /// <summary>
        /// The radius
        /// </summary>
        private double _radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleV1"/> class.
        /// </summary>
        /// <param name="centrePoint">The centre point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="colour">The colour.</param>
        /// <param name="debugColour">The debug colour.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="orientation">The orientation.</param>
        public CircleV1(Point centrePoint, double radius, Colour colour, Colour debugColour, IShape parent, Angle orientation)
        {
            _centrePoint = centrePoint;
            _radius = radius;
            Colour = colour;
            DebugColour = debugColour;
            _parentShape = parent;
            _angle = orientation;
            _childShapes = null;
            _boundingBox = CalculateBoundingBox();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleV1"/> class.
        /// </summary>
        /// <param name="centrePoint">The centre point.</param>
        /// <param name="radius">The radius.</param>
        public CircleV1(Point centrePoint, double radius) : this(centrePoint, radius, DefinedColours.White, DefinedColours.White, null, Angle.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleV1"/> class.
        /// </summary>
        /// <param name="centrePoint">The centre point.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="parent">The parent.</param>
        public CircleV1(Point centrePoint, double radius, IShape parent) : this(centrePoint, radius, DefinedColours.White, DefinedColours.White, parent, Angle.Zero)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircleV1"/> class.
        /// </summary>
        /// <param name="circle">The circle.</param>
        public CircleV1(CircleV1 circle) : this(circle.CentrePoint, circle.Radius, circle.Colour, circle.DebugColour, circle.ParentShape, circle.GetOrientation())
        {
            if(circle._childShapes != null)
            {
                _childShapes = new List<IShape>();
                foreach(IShape childShape in circle._childShapes)
                {
                    _childShapes.Add(childShape.Clone());
                }
            }
        }

        /// <summary>
        /// Gets the axis aligned bounding box.
        /// </summary>
        /// <value>The axis aligned bounding box.</value>
        public BoundingBox AxisAlignedBoundingBox => BoundingBox;

        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        /// <value>The bounding box.</value>
        public BoundingBox BoundingBox => _boundingBox;

        /// <summary>
        /// Gets or sets the centre point.
        /// </summary>
        /// <value>The centre point.</value>
        public Point CentrePoint
        {
            get => _centrePoint;
            set
            {
                _centrePoint = value;
                _boundingBox = CalculateBoundingBox();
                UpdateChildShapes();
            }
        }

        /// <summary>
        /// Gets or sets the child shapes.
        /// </summary>
        /// <value>The child shapes.</value>
        public List<IShape> ChildShapes => _childShapes;

        /// <summary>
        /// Gets or sets the colour.
        /// </summary>
        /// <value>The colour.</value>
        public Colour Colour { get; set; }

        /// <summary>
        /// Gets or sets the debug colour.
        /// </summary>
        /// <value>The debug colour.</value>
        public Colour DebugColour { get; set; }

        /// <summary>
        /// Gets the parent shape.
        /// </summary>
        /// <value>The parent shape.</value>
        public IShape ParentShape => _parentShape;

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public double Radius
        {
            get => _radius;
            set
            {
                _radius = value;
                _boundingBox = CalculateBoundingBox();
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public IShape Clone()
        {
            CircleV1 output = new CircleV1(this);
            return output;
        }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        public Angle GetOrientation()
        {
            return _angle;
        }

        /// <summary>
        /// Updates this instance based on the parent shape of this instance.
        /// </summary>
        public void ParentUpdated()
        {
            // for a circle, all we need to do is update our orientation to reflect the parent's orientation
            _angle = _angle + _parentShape.GetOrientation();
            UpdateChildShapes();
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            CalculateBoundingBox();
        }

        /// <summary>
        /// Sets the orientation.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public void SetOrientation(Angle orientation)
        {
            _angle = orientation;
            UpdateChildShapes();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Circle: CentrePoint={CentrePoint}, Radius={Radius}";
        }

        /// <summary>
        /// Updates the child shapes.
        /// </summary>
        public void UpdateChildShapes()
        {
            if(_childShapes == null)
            {
                return;
            }

            for(var i = 0; i < _childShapes.Count; i++)
            {
                _childShapes[i].ParentUpdated();
            }
        }

        /// <summary>
        /// Calculates the bounding box.
        /// </summary>
        /// <returns>The updated bounding box.</returns>
        private BoundingBox CalculateBoundingBox()
        {
            BoundingBox output = new BoundingBox(_centrePoint.X - Radius, _centrePoint.Y - Radius, _centrePoint.X + Radius, _centrePoint.Y + Radius);
            return output;
        }
    }
}

using System.Collections.Generic;
using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// An abstract class that defines a shape.
    /// </summary>
    public abstract class Shape
    {
        /// <summary>
        /// The children of this shape.
        /// </summary>
        public List<Shape> Children;

        /// <summary>
        /// The parent shape of this shape.
        /// </summary>
        public Shape Parent;

        /// <summary>
        /// The render components
        /// </summary>
        public ShapeRenderComponents RenderComponents;

        /// <summary>
        /// The axis aligned bounding box
        /// </summary>
        protected BoundingBox _axisAlignedBoundingBox;

        /// <summary>
        /// The bounding box
        /// </summary>
        protected BoundingBox _boundingBox;

        /// <summary>
        /// The centre point
        /// </summary>
        protected Point _centrePoint;

        /// <summary>
        /// The orientation
        /// </summary>
        protected Angle _orientation;

        /// <summary>
        /// The previous centre point
        /// </summary>
        protected Point PreviousCentrePoint;

        /// <summary>
        /// The previous orientation
        /// </summary>
        protected Angle PreviousOrientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="fillComponent">The fill component.</param>
        /// <param name="outlineComponent">The outline component.</param>
        public Shape(double x, double y, Angle orientation, ShapeRenderComponent fillComponent, ShapeRenderComponent outlineComponent)
        {
            Children = null;
            Parent = null;
            _centrePoint = new Point(x, y);
            PreviousCentrePoint = _centrePoint;
            _orientation = orientation;
            PreviousOrientation = _orientation;
            RenderComponents = new ShapeRenderComponents(fillComponent, outlineComponent);
            RecalculateShapeData();
        }

        /// <summary>
        /// Gets the axis aligned bounding box.
        /// </summary>
        /// <value>The axis aligned bounding box.</value>
        public BoundingBox AxisAlignedBoundingBox => _axisAlignedBoundingBox;

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
                RecalculateShapeData();
                PreviousCentrePoint = _centrePoint;
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public Angle Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                RecalculateShapeData();
                PreviousOrientation = _orientation;
            }
        }

        /// <summary>
        /// Adds a child to this shape.
        /// </summary>
        /// <param name="child">The child.</param>
        public void AddChild(Shape child)
        {
            if(Children == null)
            {
                Children = new List<Shape>();
            }

            Children.Add(child);
            child.SetParent(this);
            child.RecalculateShapeData();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <param name="includeParentLink">if set to <c>true</c> [include parent link].</param>
        /// <param name="cloneChildren">if set to <c>true</c> [clone children].</param>
        /// <returns>The cloned instance.</returns>
        public Shape Clone(bool includeParentLink = false, bool cloneChildren = false)
        {
            Shape newInstance = CloneInstance();
            if(includeParentLink)
            {
                newInstance.Parent = Parent;
            }

            if(cloneChildren)
            {
                newInstance.Children = new List<Shape>();
                foreach(Shape child in Children)
                {
                    newInstance.Children.Add(child.Clone(true, true));
                }
            }

            return newInstance;
        }

        /// <summary>
        /// Clones the instance.
        /// </summary>
        /// <returns>The cloned instance.</returns>
        public abstract Shape CloneInstance();

        /// <summary>
        /// Triggers the recalculations of shape data for the current instance.
        /// </summary>
        public abstract void RecalculateOwnShapeData();

        /// <summary>
        /// Triggers the recalculations of shape data.
        /// </summary>
        public void RecalculateShapeData()
        {
            RecalculateOwnShapeData();
            if(Children != null)
            {
                foreach(Shape child in Children)
                {
                    child.RecalculateShapeData();
                }
            }
        }

        /// <summary>
        /// Sets the centrepoint x of this shape.
        /// </summary>
        /// <param name="x">The x.</param>
        public void SetCentreX(double x)
        {
            _centrePoint.X = x;
            RecalculateShapeData();
            PreviousCentrePoint = _centrePoint;
        }

        /// <summary>
        /// Sets the centrepoint y of this shape.
        /// </summary>
        /// <param name="y">The y.</param>
        public void SetCentreY(double y)
        {
            _centrePoint.Y = y;
            RecalculateShapeData();
            PreviousCentrePoint = _centrePoint;
        }

        /// <summary>
        /// Sets the orientation.
        /// </summary>
        /// <param name="angle">The angle.</param>
        public void SetOrientation(Angle angle)
        {
            _orientation = angle;
            RecalculateShapeData();
            PreviousOrientation = _orientation;
        }

        /// <summary>
        /// Sets the orientation to the specified degrees.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        public void SetOrientationToDegrees(double degrees)
        {
            _orientation.Degrees = degrees;
            RecalculateShapeData();
            PreviousOrientation = _orientation;
        }

        /// <summary>
        /// Sets the orientation to the specified radians.
        /// </summary>
        /// <param name="radians">The radians.</param>
        public void SetOrientationToRadians(double radians)
        {
            _orientation.Radians = radians;
            RecalculateShapeData();
            PreviousOrientation = _orientation;
        }

        /// <summary>
        /// Sets the parent.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public void SetParent(Shape parent)
        {
            Parent = parent;
            RecalculateShapeData();
        }
    }
}

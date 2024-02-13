using System.Collections.Generic;
using ALife.Core.CollisionDetection;
using ALife.Core.CommonInterfaces;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    /// <summary>
    /// An abstract class that defines a shape.
    /// </summary>
    public abstract class Shape : IDeepCloneable<Shape>
    {
        /// <summary>
        /// Any child shapes of this shape. Children will be transformed relative to their parent.
        /// </summary>
        public List<Shape> Children;

        /// <summary>
        /// &gt; The layers this shape will collide with
        /// </summary>
        public SimulationLayer CollisionLayer;

        /// <summary>
        /// The non-debug colour information
        /// </summary>
        public ColourInfo ColourInfo;

        /// <summary>
        /// The debug colour information
        /// </summary>
        public ColourInfo DebugColourInfo;

        /// <summary>
        /// Whether to allow the bounding box of this shape to be merged with its children.
        /// </summary>
        public bool MergeBoundingBoxWithChildren;

        /// <summary>
        /// The parent shape of the current shape...if one exists.
        /// </summary>
        public Shape Parent;

        /// <summary>
        /// The layers this shape will be rendered on
        /// </summary>
        public SimulationLayer RenderLayer;

        /// <summary>
        /// The centre point of this shape. If the shape has a parent, this point is relative to the parent's centre
        /// </summary>
        protected Point _centrePoint;

        /// <summary>
        /// The orientation of the shape. If the shape has a parent, this orientation is relative to the parent's orientation.
        /// </summary>
        protected Angle _orientation;

        /// <summary>
        /// The previous centre point
        /// </summary>
        protected Point _previousCentrePoint;

        /// <summary>
        /// The previous orientation
        /// </summary>
        protected Angle _previousOrientation;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="centrePoint">The centre point.</param>
        /// <param name="allowedToCollide">if set to <c>true</c> [allowed to collide].</param>
        /// <param name="mergeBoundingBoxWithChildren">if set to <c>true</c> [merge bounding box with children].</param>
        /// <param name="orientation">The orientation.</param>
        /// <param name="children">The children.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="colourInfo">The colour information.</param>
        /// <param name="debugColourInfo">The debug colour information.</param>
        public Shape(ShapeArguments arguments)
        {
            _centrePoint = arguments.CentrePoint;
            _orientation = arguments.Orientation ?? new Angle(0);
            ColourInfo = arguments.ColourInfo ?? new ColourInfo();
            DebugColourInfo = arguments.DebugColourInfo ?? new ColourInfo();
            MergeBoundingBoxWithChildren = arguments.MergeBoundingBoxWithChildren;
            Children = arguments.Children;
            Parent = arguments.Parent;
            CollisionLayer = arguments.CollisionLayer;
            RenderLayer = arguments.RenderLayer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shape"/> class.
        /// </summary>
        /// <param name="shape">The shape.</param>
        public Shape(Shape shape) : this(shape.GetShapeArguments())
        {
        }

        /// <summary>
        /// The centre point of this shape. If the shape has a parent, this point is relative to the parent's centre
        /// </summary>
        public Point CentrePoint => _centrePoint;

        /// <summary>
        /// Gets the orientation of the shape. If the shape has a parent, this is relative to the parent. For absolute
        /// orientation, call GetAbsoluteOrientation().
        /// </summary>
        /// <value>The orientation.</value>
        public Angle Orientation => _orientation;

        /// <summary>
        /// Deep clones this instance.
        /// </summary>
        /// <returns>The new cloned instance.</returns>
        public abstract Shape Clone();

        /// <summary>
        /// Gets the absolute orientation for this object.
        /// </summary>
        /// <returns>The absolute orientation.</returns>
        public Angle GetAbsoluteOrientation()
        {
            // if we don't have any parents, our orientation is already absolute...
            if(Parent == null)
            {
                return Orientation;
            }

            // otherwise, get the parent's orientation and add our own.
            // NOTE: this is recursive, so we will go up the chain adding all the orientations together.
            return Parent.GetAbsoluteOrientation() + Orientation;
        }

        /// <summary>
        /// Gets the bounding box for this shape, and potentially its children.
        /// </summary>
        /// <returns>The bounding box for this shape.</returns>
        public BoundingBox? GetBoundingBox(SimulationLayer layer)
        {
            bool shouldCollide = CollisionLayer.HasFlag(layer);
            // if we should merge with our children, merge with them!
            if(MergeBoundingBoxWithChildren)
            {
                List<BoundingBox> boxes = new List<BoundingBox>();
                if(shouldCollide)
                {
                    boxes.Add(GetSelfBoundingBox());
                }

                if(Children != null)
                {
                    foreach(Shape child in Children)
                    {
                        BoundingBox? childBox = child.GetBoundingBox(layer);
                        if(childBox != null)
                        {
                            boxes.Add(childBox.Value);
                        }
                    }
                }

                if(boxes.Count == 0)
                {
                    return null;
                }
                BoundingBox mergedBox = CollisionDetection.BoundingBox.FromBoundingBoxes(boxes.ToArray());
                return mergedBox;
            }
            // if we are not allowed to collide, return null
            else if(!shouldCollide)
            {
                return null;
            }
            // else, return self bounding box
            else
            {
                return GetSelfBoundingBox();
            }
        }

        /// <summary>
        /// Determines whether the specified other is collision.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns><c>true</c> if the specified other is collision; otherwise, <c>false</c>.</returns>
        public bool IsCollision(Shape other, SimulationLayer layer)
        {
            // Get our bounding box and the other's bounding box and compare them
            BoundingBox? selfBoundingBox = GetBoundingBox(layer);
            BoundingBox? otherBoundingBox = other.GetBoundingBox(layer);

            if(selfBoundingBox == null || otherBoundingBox == null)
            {
                return false;
            }

            if(selfBoundingBox.Value.IsCollision(otherBoundingBox.Value))
            {
                return CollisionMarshaller.Marshal.IsCollision(this, other);
            }

            return false;
        }

        /// <summary>
        /// Sets the centre point.
        /// </summary>
        /// <param name="point">The point.</param>
        public void SetCentrePoint(Point point)
        {
            _previousCentrePoint = _centrePoint;
            _centrePoint = point;
            CentrePointUpdated();
        }

        /// <summary>
        /// Sets the centre point.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public void SetCentrePoint(double x, double y)
        {
            _previousCentrePoint = _centrePoint;
            _centrePoint.SetXY(x, y);
            CentrePointUpdated();
        }

        /// <summary>
        /// Sets the centre x coordinate.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        public void SetCentreX(double x)
        {
            _previousCentrePoint = _centrePoint;
            _centrePoint.SetX(x);
            CentrePointUpdated();
        }

        /// <summary>
        /// Sets the centre y coordinate.
        /// </summary>
        /// <param name="y">The y coordinate.</param>
        public void SetCentreY(double y)
        {
            _previousCentrePoint = _centrePoint;
            _centrePoint.SetY(y);
            CentrePointUpdated();
        }

        /// <summary>
        /// Updates the orientation of the shape.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        public void UpdateOrientation(Angle orientation)
        {
            _previousOrientation = _orientation;
            _orientation = orientation;
            OrientationUpdated();
        }

        /// <summary>
        /// Updates the orientation of the shape to the new degrees.
        /// </summary>
        /// <param name="degrees">The degrees.</param>
        public void UpdateOrientationDegrees(double degrees)
        {
            _previousOrientation = _orientation;
            _orientation.Degrees = degrees;
            OrientationUpdated();
        }

        /// <summary>
        /// Updates the orientation of the shape to the new radians.
        /// </summary>
        /// <param name="radians">The radians.</param>
        public void UpdateOrientationRadians(double radians)
        {
            _previousOrientation = _orientation;
            _orientation.Radians = radians;
            OrientationUpdated();
        }

        /// <summary>
        /// A method that executes when the centre point is updated.
        /// </summary>
        protected abstract void CentrePointUpdated();

        /// <summary>
        /// Gets the bounding box for this instance.
        /// </summary>
        /// <returns>The bounding box for this shape.</returns>
        protected abstract BoundingBox GetSelfBoundingBox();

        /// <summary>
        /// Gets the shape arguments for this shape.
        /// </summary>
        /// <returns>The shape arguments.</returns>
        protected ShapeArguments GetShapeArguments()
        {
            ShapeArguments arguments = new ShapeArguments(CentrePoint, Orientation, ColourInfo, DebugColourInfo, MergeBoundingBoxWithChildren, Children, Parent, CollisionLayer, RenderLayer);

            return arguments;
        }

        /// <summary>
        /// A method that executes when the orientation is updated.
        /// </summary>
        protected abstract void OrientationUpdated();
    }
}

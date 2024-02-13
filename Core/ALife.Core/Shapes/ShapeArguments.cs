using System.Collections.Generic;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    public struct ShapeArguments
    {
        /// <summary>
        /// The centre point of this shape. If the shape has a parent, this point is relative to the parent's centre
        /// </summary>
        public Point CentrePoint;

        /// <summary>
        /// Any child shapes of this shape. Children will be transformed relative to their parent.
        /// </summary>
        public List<Shape> Children;

        /// <summary>
        /// The layers this shape will collide with
        /// </summary>
        public SimulationLayer CollisionLayer;

        /// <summary>
        /// The non-debug colour information
        /// </summary>
        public ColourInfo? ColourInfo;

        /// <summary>
        /// The debug colour information
        /// </summary>
        public ColourInfo? DebugColourInfo;

        /// <summary>
        /// Whether to allow the bounding box of this shape to be merged with its children.
        /// </summary>
        public bool MergeBoundingBoxWithChildren;

        /// <summary>
        /// The orientation of the shape. If the shape has a parent, this orientation is relative to the parent's orientation.
        /// </summary>
        public Angle? Orientation;

        /// <summary>
        /// The parent shape of the current shape...if one exists.
        /// </summary>
        public Shape Parent;

        /// <summary>
        /// The layers this shape will be rendered on
        /// </summary>
        public SimulationLayer RenderLayer;

        public ShapeArguments(Point centrePoint, Angle? orientation = null, ColourInfo? colourInfo = null, ColourInfo? debugColourInfo = null, bool mergeBoundingBoxWithChildren = true, List<Shape> children = null, Shape parent = null, SimulationLayer collisionLayer = SimulationLayer.PHYSICAL, SimulationLayer renderLayer = SimulationLayer.PHYSICAL)
        {
            CentrePoint = centrePoint;
            Orientation = orientation;
            ColourInfo = colourInfo;
            DebugColourInfo = debugColourInfo;
            MergeBoundingBoxWithChildren = mergeBoundingBoxWithChildren;
            Children = children;
            Parent = parent;
            CollisionLayer = collisionLayer;
            RenderLayer = renderLayer;
        }
    }
}

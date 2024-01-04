using System.Collections.Generic;
using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    public abstract class Shape
    {
        public BoundingBox BoundingBox;

        public Point CentrePoint;

        public List<Shape> Children;
        public Angle Orientation;
        public Shape Parent;
        public ShapeRenderComponents RenderComponents;
        protected Point PreviousPoint;

        public abstract Shape Clone();

        public abstract void TriggerRecalculations();
    }
}

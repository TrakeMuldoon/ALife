using System.Collections.Generic;
using ALife.Core.CollisionDetection;
using ALife.Core.Geometry;

namespace ALife.Core.Shapes
{
    public abstract class Shape
    {
        public BoundingBox BoundingBox;

        public List<Shape> Children;

        public Shape Parent;

        public ShapeRenderComponents RenderComponents;

        protected Point _centrePoint;

        protected Angle _orientation;

        protected Point PreviousCentrePoint;

        protected Angle PreviousOrientation;

        public Point CentrePoint
        {
            get => _centrePoint;
            set
            {
                _centrePoint = value;
                TriggerRecalculations();
                PreviousCentrePoint = _centrePoint;
            }
        }

        public Angle Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                TriggerRecalculations();
                PreviousOrientation = _orientation;
            }
        }

        public abstract Shape Clone();

        public abstract void TriggerRecalculations();
    }
}

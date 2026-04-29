using System.Collections.Generic;

namespace ALife.Core.Geometry.Shapes
{
    public abstract class Shape
    {
        public List<Shape> Children;
        
        public ShapeColouration Colouration;

        public abstract void Reset;

        public abstract Shape Clone();
        
        public Point CentrePoint
        {
            get;
            set;
        }
        
        public Angle Orientation
        {
            get;
            set;
        }
        
        
    }
}
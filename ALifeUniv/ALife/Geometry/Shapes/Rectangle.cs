using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Rectangle : IShape
    {
        public double Length;
        public double Width;
        public Point TopLeft;

        private double splitDistanceOver2;
        private double radsDiff;

        public Rectangle(double length, double width, Point topLeft, Color color)
        {
            Length = length;
            Width = width;
            TopLeft = topLeft;
            Color = color;
            Orientation = new Angle(0);
            splitDistanceOver2 = Math.Sqrt((length * length) + (width * width)) / 2;
            radsDiff = Math.Atan(length / width);
        }

        public virtual Point CentrePoint
        {
            get
            {
                return ExtraMath.TranslateByVector(TopLeft, Orientation.Radians + radsDiff, splitDistanceOver2);
            }
        }

        public virtual Angle Orientation
        {
            get;
            set;
        }

        public virtual BoundingBox BoundingBox => throw new NotImplementedException();

        public Color Color
        { 
            get; 
            set; 
        }

        public Color DebugColor 
        { 
            get;
            set;
        }

        public virtual ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Rectangle;
        }

        public virtual void Reset()
        {
            
        }
    }
}

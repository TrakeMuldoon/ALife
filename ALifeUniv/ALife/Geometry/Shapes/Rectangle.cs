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
        public double FBLength;
        public double RLWidth;

        public Rectangle(double fbLength, double rlWidth, Point centrePoint, Color color)
        {
            CentrePoint = centrePoint;
            FBLength = fbLength;
            RLWidth = rlWidth;
            Color = color;
            DebugColor = Colors.IndianRed;

            Orientation = new Angle(0);
        }

        public virtual Point CentrePoint
        {
            get;
            set;
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

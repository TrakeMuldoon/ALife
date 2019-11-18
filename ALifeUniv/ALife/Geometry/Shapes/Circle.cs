using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Circle : IShape
    {
        public virtual Point CentrePoint
        {
            get;
            set;
        }
        public virtual float Radius
        {
            get;
            set;
        }

        public BoundingBox BoundingBox
        {
            get
            {
                return new BoundingBox(CentrePoint.X - Radius, CentrePoint.Y - Radius, CentrePoint.X + Radius, CentrePoint.Y + Radius);
            }
        }

        public virtual Angle Orientation
        {
            get;
            set;
        }

        public virtual Color Color
        {
            get;
            set;
        }
        public virtual Color DebugColor
        {
            get;
            set;
        }

        public Circle(Point coords, float radius)
        {
            CentrePoint = coords;
            Radius = radius;
        }

        public Circle(float xVal, float yVal, float radius) : this(new Point(xVal, yVal), radius)
        {
            
        }

        public void Reset()
        {
            //Reset does nothing for Circles, because the bounding box is so easy to calculate
        }

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Circle;
        }
    }
}

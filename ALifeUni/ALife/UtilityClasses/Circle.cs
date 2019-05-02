using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Circle
    {
        public Point CentrePoint;
        public float Radius;

        public Circle(Point coords, float radius)
        {
            CentrePoint = coords;
            Radius = radius;
        }

        public Circle(float xVal, float yVal, float radius) : this(new Point(xVal, yVal), radius)
        {
            
        }
    }
}

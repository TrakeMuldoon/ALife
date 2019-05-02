using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Circle
    {
        public Coordinate CentrePoint;
        public float Radius;

        public Circle(Coordinate coords, float radius)
        {
            CentrePoint = coords;
            Radius = radius;
        }

        public Circle(float xVal, float yVal, float radius) : this(new Coordinate(xVal, yVal), radius)
        {
            
        }
    }
}

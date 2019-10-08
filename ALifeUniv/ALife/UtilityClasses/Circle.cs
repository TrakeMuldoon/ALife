using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

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
        public virtual Point GetCentrePoint()
        {
            return CentrePoint;
        }

        public virtual Angle GetOrientation()
        {
            throw new Exception("Do Not Get Orientation of a raw circle, it has no meaning");
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
            throw new NotImplementedException();
        }

        public BoundingBox GetBoundingBox()
        {
            return new BoundingBox(CentrePoint.X - Radius, CentrePoint.Y - Radius, CentrePoint.X + Radius, CentrePoint.Y + Radius);
        }

        public Type GetShapeType()
        {
            return typeof(Circle);
        }
    }
}

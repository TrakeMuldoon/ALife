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
            Orientation = new Angle(0);
        }

        public void Reset()
        {
            //Reset does nothing for Circles, because the bounding box is so easy to calculate
        }

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Circle;
        }

        public IShape CloneShape()
        {
            Circle cir = new Circle(CentrePoint, Radius);
            cir.Orientation = new Angle(Orientation.Degrees);
            cir.Color = Color.FromArgb(Color.A, Color.R, Color.G, Color.B);
            return cir;
        }
    }
}

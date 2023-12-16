using System.Drawing;

namespace ALife.Core.Geometry.Shapes
{
    public class AARectangle : IShape
    {
        public Geometry.Shapes.Point CentrePoint
        {
            get
            {
                double cpX = TopLeft.X + XWidth / 2;
                double cpY = TopLeft.Y + YHeight / 2;
                return new Geometry.Shapes.Point(cpX, cpY);
            }
            set
            {
                throw new InvalidOperationException("You cannot set the centrepoint of an AARectangle");
            }
        }

        private Angle ori = new Angle(0);

        public AARectangle(Geometry.Shapes.Point topLeft, double xWidth, double yHeight, Color color)
        {
            XWidth = xWidth;
            YHeight = yHeight;
            Color = color;
            TopLeft = topLeft;
        }

        public double XWidth
        {
            get;
            set;
        }
        public double YHeight
        {
            get;
            set;
        }

        public Geometry.Shapes.Point TopLeft
        {
            get;
            set;
        }

        public Angle Orientation
        {
            get { return ori; }
            set
            {
                throw new InvalidOperationException("Orientation cannot be set for AARectangle");
            }
        }
        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(TopLeft.X, TopLeft.Y, TopLeft.X + XWidth, TopLeft.Y + YHeight); }
        }

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

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.AARectangle;
        }

        public void Reset()
        {
            //This does nothing for AARectangles
        }

        public IShape CloneShape()
        {
            return new AARectangle(TopLeft, XWidth, YHeight, Color);
        }
    }
}

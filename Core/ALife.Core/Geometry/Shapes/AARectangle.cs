using ALife.Core.Utility.Colours;
using System;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;

namespace ALife.Core.GeometryOld.Shapes
{
    public class AARectangle : IShape
    {
        public ALife.Core.GeometryOld.Shapes.Point CentrePoint
        {
            get
            {
                double cpX = TopLeft.X + XWidth / 2;
                double cpY = TopLeft.Y + YHeight / 2;
                return new ALife.Core.GeometryOld.Shapes.Point(cpX, cpY);
            }
            set
            {
                throw new InvalidOperationException("You cannot set the centrepoint of an AARectangle");
            }
        }

        private Angle ori = new Angle(0);

        public AARectangle(ALife.Core.GeometryOld.Shapes.Point topLeft, double xWidth, double yHeight, Colour color)
        {
            XWidth = xWidth;
            YHeight = yHeight;
            Colour = color;
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

        public ALife.Core.GeometryOld.Shapes.Point TopLeft
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

        public Colour Colour
        {
            get;
            set;
        }

        public Colour DebugColour
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
            return new AARectangle(TopLeft, XWidth, YHeight, Colour);
        }
    }
}

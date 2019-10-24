using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class AgentSector : Sector
    {
        public override Point CentrePoint
        {
            get;
            set;
        }
        public override BoundingBox BoundingBox
        {
            get
            {
                return GetBoundingBox(CentrePoint, Orientation);
            }
        }
        public override Angle Orientation
        {
            get;
            set;
        }

        public AgentSector(Point centrePoint, Angle orientation)
        {
            CentrePoint = centrePoint;
            Orientation = orientation;
        }
    }

    public class ChildSector : Sector
    {
        public override Angle Orientation
        {
            get;
            set;
        }
        public Angle OrientationAroundParent;
        public Circle Parent;

        public ChildSector(Angle orientationAngle, Angle orientationAroundParent, Circle parent)
        {
            Orientation = orientationAngle;
            OrientationAroundParent = orientationAroundParent;
            Parent = parent;
        }

        public override Point CentrePoint
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;
                double myOriginX = Parent.CentrePoint.X + (Parent.Radius * Math.Cos(startAngle.Radians));
                double myOriginY = Parent.CentrePoint.Y + (Parent.Radius * Math.Sin(startAngle.Radians));
                Point myOriginPoint = new Point(myOriginX, myOriginY);
                return myOriginPoint;
            }
            set
            {

            }
        }

        public override BoundingBox BoundingBox
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;

                return GetBoundingBox(CentrePoint, startAngle + Orientation);
            }
        }
    }

    public abstract class Sector : IShape
    {
        //TODO unhardcode this
        public float Radius = 8;
        public Angle SweepAngle = new Angle(60);

        public abstract BoundingBox BoundingBox
        {
            get;
        }
        public Color Color
        {
            get;
            set;
        }

        public abstract Point CentrePoint
        {
            get;
            set;
        }
        public abstract Angle Orientation
        {
            get;
            set;
        }

        private BoundingBox? myBox = null;
        public void Reset()
        {
            myBox = null;
        }

        public BoundingBox GetBoundingBox(Point xyTranslationFromZero, Angle rotation)
        {
            if (myBox != null)
            {
                try
                {
                    return myBox.Value;
                }
                catch(InvalidOperationException ioe)
                {
                    //swallow, and just build the bb
                }
            }
            Angle absOrientationAngle = rotation;
            Point myOriginPoint = xyTranslationFromZero;

            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            xValues.Add(myOriginPoint.X);
            yValues.Add(myOriginPoint.Y);

            //Get the points that are the edges of the sector
            double startX = myOriginPoint.X + (Radius * Math.Cos(absOrientationAngle.Radians));
            double startY = myOriginPoint.Y + (Radius * Math.Sin(absOrientationAngle.Radians));
            xValues.Add(startX);
            yValues.Add(startY);

            Angle endAngle = absOrientationAngle + SweepAngle;
            double endX = myOriginPoint.X + (Radius * Math.Cos(endAngle.Radians));
            double endY = myOriginPoint.Y + (Radius * Math.Sin(endAngle.Radians));
            xValues.Add(endX);
            yValues.Add(endY);

            //determine which axis lines the sweep crosses, (ie. positive X axis, Positive Y, negative X, negative y)
            if(absOrientationAngle.Degrees + SweepAngle.Degrees < 360)
            {
                //Therefore there is no wraparound. Makes the maths muchs muchs easier
                if(absOrientationAngle.Degrees < 90
                    && endAngle.Degrees > 90)
                {
                    yValues.Add(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180
                    && endAngle.Degrees > 180)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270
                    && endAngle.Degrees > 270)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }
            }
            else
            {
                xValues.Add(myOriginPoint.X + Radius);
                //These if statements cover the potential start locations
                if (absOrientationAngle.Degrees < 90)
                {
                    yValues.Add(myOriginPoint.Y + Radius);
                }
                if (absOrientationAngle.Degrees < 180)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
                if (absOrientationAngle.Degrees < 270)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }

                //These three if statements cover the potential end locations
                if (endAngle.Degrees > 90)
                {
                    yValues.Add(myOriginPoint.Y + Radius);
                }
                if (endAngle.Degrees > 180)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
                if (endAngle.Degrees > 270)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }
            }

            double minX = ExtraMath.MultiMin(xValues.ToArray());
            double minY = ExtraMath.MultiMin(yValues.ToArray());
            double maxX = ExtraMath.MultiMax(xValues.ToArray());
            double maxY = ExtraMath.MultiMax(yValues.ToArray());

            BoundingBox sectorBB = new BoundingBox(minX, minY, maxX, maxY);
            myBox = sectorBB;
            return sectorBB;
        }

        public ShapesEnum GetShape()
        {
            return ShapesEnum.Sector;
        }
        //public Type GetShapeType()
        //{
        //    return typeof(Sector);
        //}
    }
}

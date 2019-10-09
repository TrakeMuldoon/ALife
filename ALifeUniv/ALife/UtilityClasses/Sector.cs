using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class AgentSector : Sector
    {
        public Point CentrePoint;
        public Angle Orientation;

        public AgentSector(Point centrePoint, Angle orientation)
        {
            CentrePoint = centrePoint;
            Orientation = orientation;
        }

        public override BoundingBox GetBoundingBox()
        {
            return GetBoundingBox(CentrePoint, Orientation);
        }

        public override Point GetCentrePoint()
        {
            return CentrePoint;
        }

        public override Angle GetOrientation()
        {
            return Orientation;
        }
    }

    public class ChildSector : Sector
    {
        public Angle OrientationAngle;
        public Angle OrientationAroundParent;
        public Circle Parent;

        public ChildSector(Angle orientationAngle, Angle orientationAroundParent, Circle parent)
        {
            OrientationAngle = orientationAngle;
            OrientationAroundParent = orientationAroundParent;
            Parent = parent;
        }

        public override Point GetCentrePoint()
        {
            Angle startAngle = Parent.GetOrientation() + OrientationAroundParent;
            double myOriginX = Parent.GetCentrePoint().X + (Parent.Radius * Math.Cos(startAngle.Radians));
            double myOriginY = Parent.GetCentrePoint().Y + (Parent.Radius * Math.Sin(startAngle.Radians));
            Point myOriginPoint = new Point(myOriginX, myOriginY);
            return myOriginPoint;
        }

        public override Angle GetOrientation()
        {
            return OrientationAngle;
        }

        public override BoundingBox GetBoundingBox()
        {
            Angle startAngle = Parent.GetOrientation()+ OrientationAroundParent;

            return GetBoundingBox(GetCentrePoint(), startAngle + OrientationAngle);
        }
    }

    public abstract class Sector : IShape
    {
        //TODO unhardcode this
        public float Radius = 8;
        public Angle SweepAngle = new Angle(45);

        public abstract Point GetCentrePoint();
        public abstract Angle GetOrientation();

        private BoundingBox? myBox = null;

        public void Reset()
        {
            myBox = null;
        }

        public abstract BoundingBox GetBoundingBox();

        public BoundingBox GetBoundingBox(Point xyTranslationFromZero, Angle rotation)
        {
            if (myBox != null)
            {
                return myBox.Value;
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
                    xValues.Add(myOriginPoint.X + Radius);
                }
                if(absOrientationAngle.Degrees < 180
                    && endAngle.Degrees > 180)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }
                if(absOrientationAngle.Degrees < 270
                    && endAngle.Degrees > 270)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
            }
            else
            {
                yValues.Add(myOriginPoint.Y + Radius);
                //These if statements cover the potential start locations
                if(absOrientationAngle.Degrees < 90)
                {
                    xValues.Add(myOriginPoint.X + Radius);
                }
                if(absOrientationAngle.Degrees < 180)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }
                if (absOrientationAngle.Degrees < 270)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }

                //These three if statements cover the potential end locations
                if (endAngle.Degrees > 90)
                {
                    xValues.Add(myOriginPoint.X + Radius);
                }
                if (endAngle.Degrees > 180)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }
                if (endAngle.Degrees > 270)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
            }

            double minX = ExtraMath.MultiMin(xValues.ToArray());
            double minY = ExtraMath.MultiMin(yValues.ToArray());
            double maxX = ExtraMath.MultiMax(xValues.ToArray());
            double maxY = ExtraMath.MultiMax(yValues.ToArray());
            BoundingBox sectorBB = new BoundingBox(minX, minY, maxX, maxY);
            //myBox = sectorBB;
            return sectorBB;
        }

        public Type GetShapeType()
        {
            return typeof(Sector);
        }
    }
}

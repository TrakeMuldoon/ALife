using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class Sector : IShape
    {
        public Point CentrePoint;
        public float Radius;
        public Angle SweepAngle;
        public Angle OrientationAngle;

        public BoundingBox GetBoundingBox()
        {
            List<double> xValues = new List<double>();
            List<double> yValues = new List<double>();

            xValues.Add(CentrePoint.X);
            yValues.Add(CentrePoint.Y);

            //Get the points that are the edges of the sector
            double startX = CentrePoint.X + (Radius * Math.Cos(OrientationAngle.Radians));
            double startY = CentrePoint.Y + (Radius * Math.Sin(OrientationAngle.Radians));
            xValues.Add(startX);
            yValues.Add(startY);

            Angle endAngle = OrientationAngle + SweepAngle;
            double endX = CentrePoint.X + (Radius * Math.Cos(endAngle.Radians));
            double endY = CentrePoint.Y + (Radius * Math.Sin(endAngle.Radians));
            xValues.Add(endX);
            yValues.Add(endY);

            //determine which axis lines the sweep crosses, (ie. positive X axis, Positive Y, negative X, negative y)

            if(OrientationAngle.Degrees + SweepAngle.Degrees < 360)
            {
                //Therefore there is no wraparound. Makes the maths muchs muchs easier
                if(OrientationAngle.Degrees < 90
                    && endAngle.Degrees > 90)
                {
                    xValues.Add(CentrePoint.X + Radius);
                }
                if(OrientationAngle.Degrees < 180
                    && endAngle.Degrees > 180)
                {
                    yValues.Add(CentrePoint.Y - Radius);
                }
                if(OrientationAngle.Degrees < 270
                    && endAngle.Degrees > 270)
                {
                    xValues.Add(CentrePoint.X - Radius);
                }
            }
            else
            {
                yValues.Add(CentrePoint.Y + Radius);
                //These if statements cover the potential start locations
                if(OrientationAngle.Degrees < 90)
                {
                    xValues.Add(CentrePoint.X + Radius);
                }
                if(OrientationAngle.Degrees < 180)
                {
                    yValues.Add(CentrePoint.Y - Radius);
                }
                if (OrientationAngle.Degrees < 270)
                {
                    xValues.Add(CentrePoint.X - Radius);
                }

                if (endAngle.Degrees > 90)
                {
                    xValues.Add(CentrePoint.X + Radius);
                }
                if (endAngle.Degrees > 180)
                {
                    yValues.Add(CentrePoint.Y - Radius);
                }
                if (endAngle.Degrees > 270)
                {
                    xValues.Add(CentrePoint.X - Radius);
                }
            }

            double minX = ExtraMath.MultiMin(xValues.ToArray());
            double minY = ExtraMath.MultiMin(yValues.ToArray());
            double maxX = ExtraMath.MultiMax(xValues.ToArray());
            double maxY = ExtraMath.MultiMax(yValues.ToArray());
            BoundingBox sectorBB = new BoundingBox(minX, minY, maxX, maxY);

            return sectorBB;
        }

        public Type GetShapeType()
        {
            return typeof(Sector);
        }
    }
}

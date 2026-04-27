using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using System;
using System.Collections.Generic;

namespace ALife.Core.Geometry.Shapes
{
    public class Sector : IShape
    {
        public float Radius;
        public Angle SweepAngle;

        public virtual BoundingBox BoundingBox
        {
            get
            {
                return GetBoundingBox(Orientation);
            }
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

        public virtual Point CentrePoint
        {
            get;
            set;
        }

        public virtual Angle Orientation
        {
            get;
            set;
        }

        private Point leftPoint;
        public Point LeftPoint
        {
            get
            {
                if(myBox == null)
                {
                    _ = BoundingBox;
                }
                return leftPoint;
            }
        }

        private Point rightPoint;
        public Point RightPoint
        {
            get
            {
                if(myBox == null)
                {
                    _ = BoundingBox;
                }
                return rightPoint;
            }
        }

        private BoundingBox? myBox = null;

        public Sector(Point centrePoint, float radius, Angle sweepAngle, Colour color)
        {
            CentrePoint = centrePoint;
            Radius = radius;
            SweepAngle = sweepAngle;
            Colour = color;
            Orientation = new Angle(0);
        }

        protected Sector(float radius, Angle sweepAngle)
        {
            Radius = radius;
            SweepAngle = sweepAngle;
        }

        public void Reset()
        {
            myBox = null;
        }

        public BoundingBox GetBoundingBox(Angle rotation)
        {
            if(myBox != null)
            {
                try
                {
                    return myBox.Value;
                }
                catch(InvalidOperationException)
                {
                    //swallow, and just build the bb
                }
            }
            Angle absOrientationAngle = rotation;
            Point myOriginPoint = CentrePoint;

            double minX = myOriginPoint.X, maxX = myOriginPoint.X;
            double minY = myOriginPoint.Y, maxY = myOriginPoint.Y;

            void ExpandX(double x) { if(x < minX) minX = x; if(x > maxX) maxX = x; }
            void ExpandY(double y) { if(y < minY) minY = y; if(y > maxY) maxY = y; }

            //Get the points that are the edges of the sector
            leftPoint = GeometryMath.TranslateByVector(myOriginPoint, absOrientationAngle, Radius);
            ExpandX(leftPoint.X);
            ExpandY(leftPoint.Y);

            Angle endAngle = absOrientationAngle + SweepAngle;
            rightPoint = GeometryMath.TranslateByVector(myOriginPoint, endAngle, Radius);
            ExpandX(rightPoint.X);
            ExpandY(rightPoint.Y);

            //determine which axis lines the sweep crosses, (ie. positive X axis, Positive Y, negative X, negative y)
            if(absOrientationAngle.Degrees + SweepAngle.Degrees < 360)
            {
                //Therefore there is no wraparound. Makes the maths muchs muchs easier
                if(absOrientationAngle.Degrees < 90
                    && endAngle.Degrees > 90)
                {
                    ExpandY(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180
                    && endAngle.Degrees > 180)
                {
                    ExpandX(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270
                    && endAngle.Degrees > 270)
                {
                    ExpandY(myOriginPoint.Y - Radius);
                }
            }
            else
            {
                ExpandX(myOriginPoint.X + Radius);
                //These if statements cover the potential start locations
                if(absOrientationAngle.Degrees < 90)
                {
                    ExpandY(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180)
                {
                    ExpandX(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270)
                {
                    ExpandY(myOriginPoint.Y - Radius);
                }

                //These three if statements cover the potential end locations
                if(endAngle.Degrees > 90)
                {
                    ExpandY(myOriginPoint.Y + Radius);
                }
                if(endAngle.Degrees > 180)
                {
                    ExpandX(myOriginPoint.X - Radius);
                }
                if(endAngle.Degrees > 270)
                {
                    ExpandY(myOriginPoint.Y - Radius);
                }
            }

            BoundingBox sectorBB = new BoundingBox(minX, minY, maxX, maxY);
            myBox = sectorBB;
            return sectorBB;
        }

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Sector;
        }

        public virtual IShape CloneShape()
        {
            Sector newSec = new Sector(new Point(CentrePoint.X, CentrePoint.Y), Radius, SweepAngle.Clone(), (Colour)Colour.Clone());
            newSec.Orientation = Orientation.Clone();
            return newSec;
        }
    }
}

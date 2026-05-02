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

            BoundingBox sectorBB = new(myOriginPoint.X, myOriginPoint.Y, myOriginPoint.X, myOriginPoint.Y);

            //Get the points that are the edges of the sector
            leftPoint = GeometryMath.TranslateByVector(myOriginPoint, absOrientationAngle, Radius);
            sectorBB.TransformByPoint(leftPoint);

            Angle endAngle = absOrientationAngle + SweepAngle;
            rightPoint = GeometryMath.TranslateByVector(myOriginPoint, endAngle, Radius);
            sectorBB.TransformByPoint(rightPoint);

            //determine which axis lines the sweep crosses, (ie. positive X axis, Positive Y, negative X, negative y)
            if(absOrientationAngle.Degrees + SweepAngle.Degrees < 360)
            {
                //Therefore there is no wraparound. Makes the maths muchs muchs easier
                if(absOrientationAngle.Degrees < 90
                    && endAngle.Degrees > 90)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180
                    && endAngle.Degrees > 180)
                {
                    sectorBB.TransformByXCoord(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270
                    && endAngle.Degrees > 270)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y - Radius);
                }
            }
            else
            {
                sectorBB.TransformByXCoord(myOriginPoint.X + Radius);
                //These if statements cover the potential start locations
                if(absOrientationAngle.Degrees < 90)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180)
                {
                    sectorBB.TransformByXCoord(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y - Radius);
                }

                //These three if statements cover the potential end locations
                if(endAngle.Degrees > 90)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y + Radius);
                }
                if(endAngle.Degrees > 180)
                {
                    sectorBB.TransformByXCoord(myOriginPoint.X - Radius);
                }
                if(endAngle.Degrees > 270)
                {
                    sectorBB.TransformByYCoord(myOriginPoint.Y - Radius);
                }
            }

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

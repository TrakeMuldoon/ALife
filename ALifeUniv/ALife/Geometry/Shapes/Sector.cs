using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class ChildSector : Sector
    {
        public override Angle Orientation
        {
            get { return RelativeOrientation; }
            set { RelativeOrientation = value; }
        }

        public override Angle RelativeOrientation
        {
            get;
            set;
        }

        public override Angle AbsoluteOrientation
        {
            get
            {
                return Parent.Orientation + OrientationAroundParent + RelativeOrientation;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public Angle OrientationAroundParent;
        public IShape Parent;
        private double distanceFromParentCentre;

        public ChildSector(Angle orientationAroundParent
                            , IShape parent
                            , double distFromParentCentre
                            , Angle relativeOrientationAngle
                            , float radius
                            , Angle sweep) : base(radius, sweep)
        {
            Orientation = relativeOrientationAngle;
            OrientationAroundParent = orientationAroundParent;
            distanceFromParentCentre = distFromParentCentre;
            Parent = parent;
        }

        public override Point CentrePoint
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;
                double myOriginX = Parent.CentrePoint.X + (distanceFromParentCentre * Math.Cos(startAngle.Radians));
                double myOriginY = Parent.CentrePoint.Y + (distanceFromParentCentre * Math.Sin(startAngle.Radians));
                Point myOriginPoint = new Point(myOriginX, myOriginY);
                return myOriginPoint;
            }
            set
            {
                throw new Exception("There should be no setting of ChildSector Centrepoints");
            }
        }

        public override BoundingBox BoundingBox
        {
            get
            {
                return GetBoundingBox(CentrePoint, AbsoluteOrientation);
            }
        }

        //TODO: Ask Jeremy or Bryan about how to implement this properly
        public override IShape CloneShape()
        {
            ChildSector clon = new ChildSector(OrientationAroundParent, Parent, distanceFromParentCentre
                                                , RelativeOrientation, Radius, SweepAngle);
            clon.Color = Color;
            return clon;
        }
    }

    public abstract class Sector : IShape
    {
        public float Radius;
        public Angle SweepAngle;

        public abstract BoundingBox BoundingBox
        {
            get;
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

        public abstract Angle RelativeOrientation
        {
            get;
            set;
        }

        public abstract Angle AbsoluteOrientation
        {
            get;
            set;
        }

        private BoundingBox? myBox = null;

        protected Sector(float radius, Angle sweepAngle)
        {
            Radius = radius;
            SweepAngle = sweepAngle;
        }

        public void Reset()
        {
            myBox = null;
        }

        public BoundingBox GetBoundingBox(Point xyTranslationFromZero, Angle rotation)
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
                if(absOrientationAngle.Degrees < 90)
                {
                    yValues.Add(myOriginPoint.Y + Radius);
                }
                if(absOrientationAngle.Degrees < 180)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
                if(absOrientationAngle.Degrees < 270)
                {
                    yValues.Add(myOriginPoint.Y - Radius);
                }

                //These three if statements cover the potential end locations
                if(endAngle.Degrees > 90)
                {
                    yValues.Add(myOriginPoint.Y + Radius);
                }
                if(endAngle.Degrees > 180)
                {
                    xValues.Add(myOriginPoint.X - Radius);
                }
                if(endAngle.Degrees > 270)
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

        public ShapesEnum GetShapeEnum()
        {
            return ShapesEnum.Sector;
        }

        public abstract IShape CloneShape();
    }
}

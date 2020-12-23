using System;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class ChildSector : Sector, IChildShape
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

        public ChildSector(IShape parent
                            , Angle orientationAroundParent
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
                return ExtraMath.TranslateByVector(Parent.CentrePoint, startAngle.Radians, distanceFromParentCentre);
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
                return GetBoundingBox(AbsoluteOrientation);
            }
        }

        //TODO: Ask Jeremy or Bryan about how to implement this properly
        public override IShape CloneShape()
        {
            throw new NotImplementedException("Cannot clone a ChildShape");
        }
        public IShape CloneChildShape(IShape parent)
        {
            return new ChildSector(parent, OrientationAroundParent.Clone(), distanceFromParentCentre, RelativeOrientation.Clone(), Radius, SweepAngle.Clone());
        }
    }
}

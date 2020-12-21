using ALifeUni.ALife.Utility;
using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class ChildRectangle : Rectangle
    {
        public readonly IShape Parent;
        public readonly Angle OrientationAroundParent;
        public readonly double DistFromParentCentre;

        public ChildRectangle(IShape parent, Angle orientationAroundParent, double distFromParentCentre
                                , double FBLength, double RLWidth)
            : base()
        {
            Orientation = new Angle(0);
            OrientationAroundParent = orientationAroundParent;
            DistFromParentCentre = distFromParentCentre;
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
                return GetBoundingBox(CentrePoint, AbsoluteOrientation);
            }
        }

        //TODO: Ask Jeremy or Bryan about how to implement this properly
        public override IShape CloneShape()
        {
            ChildSector clon = new ChildSector(OrientationAroundParent, Parent, distanceFromParentCentre
                                                , RelativeOrientation, Radius, SweepAngle);
            clon.Color = Color.Clone();
            return clon;
        }
    }
}

using ALifeUni.ALife.Utility;
using System;
using Windows.Foundation;

namespace ALifeUni.ALife.Shapes
{
    public class ChildCircle : Circle, IChildShape
    {
        public readonly IShape Parent;
        public readonly Angle OrientationAroundParent;
        public readonly double DistFromParentCentre;

        public override Point CentrePoint
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;
                return ExtraMath.TranslateByVector(Parent.CentrePoint, startAngle, DistFromParentCentre);
            }
            set
            {
                throw new Exception("There should be no setting of ChildCircle Centrepoints");
            }
        }

        public ChildCircle(IShape parent, Angle orientationAroundParent, double distFromParentCentre
                           , float radius)
            : base(radius)
        {
            Orientation = new Angle(0);
            OrientationAroundParent = orientationAroundParent;
            DistFromParentCentre = distFromParentCentre;
            Parent = parent;
        }

        public override IShape CloneShape()
        {
            throw new NotImplementedException("Cannot clone a ChildShape");
        }
        public IShape CloneChildShape(IShape parent)
        {
            return new ChildCircle(parent, OrientationAroundParent.Clone(), DistFromParentCentre, Radius);
        }
    }
}

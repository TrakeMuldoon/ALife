using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ALifeUni.ALife.UtilityClasses
{
    public class ChildCircle : Circle
    {
        public readonly IShape Parent;
        public readonly Angle OrientationAroundParent;
        public readonly double DistFromParentCentre;

        public override Point CentrePoint
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;
                return ExtraMath.TranslateByVector(Parent.CentrePoint, startAngle.Radians, DistFromParentCentre);
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
    }
}

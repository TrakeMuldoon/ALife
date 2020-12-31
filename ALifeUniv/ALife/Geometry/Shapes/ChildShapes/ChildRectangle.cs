using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.UtilityClasses
{
    public class ChildRectangle : Rectangle, IChildShape
    {
        public readonly IShape Parent;
        public readonly Angle RelativeToParentOrientation;
        public readonly double DistFromParentCentre;

        public override Angle Orientation
        {
            get
            {
                return RelativeToParentOrientation + Parent.Orientation;
            }
        }

        public ChildRectangle(IShape parent, Angle relativeToParentOrientation, double distFromParentCentre
                                , double FBLength, double RLWidth)
            : base(FBLength, RLWidth, Colors.Red)
        {
            RelativeToParentOrientation = relativeToParentOrientation;
            DistFromParentCentre = distFromParentCentre;
            Parent = parent;
        }

        private Point? myCentrePoint;
        public override Point CentrePoint
        {
            get
            {
                if(!myCentrePoint.HasValue)
                {
                    GenerateCentrePoint();
                }
                return myCentrePoint.Value;
            }
            set
            {
                throw new Exception("No setting rectangle centrepoint");
            }
        }

        private void GenerateCentrePoint()
        {
            Angle relativeAngle = Parent.Orientation + RelativeToParentOrientation;
            Point centre = ExtraMath.TranslateByVector(Parent.CentrePoint, relativeAngle, DistFromParentCentre + (FBLength / 2));
            myCentrePoint = centre;
        }

        public override void Reset()
        {
            myCentrePoint = null;
            base.Reset();
        }

        public override IShape CloneShape()
        {
            throw new NotImplementedException("Cannot clone a childshape");
        }
        public IShape CloneChildShape(IShape parent)
        {
            return new ChildRectangle(parent, RelativeToParentOrientation.Clone(), DistFromParentCentre, FBLength, RLWidth);
        }
    }
}

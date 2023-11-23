using ALifeUni.ALife.Utility;
using System;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Shapes
{
    public class ChildRectangle : Rectangle, IChildShape
    {
        public readonly IShape Parent;
        public readonly double DistFromParentCentre;

        public override Angle Orientation
        {
            get
            {
                return RelativeOrientation + Parent.Orientation;
            }
        }

        public Angle RelativeOrientation
        {
            get;
            set;
        }
        public Angle AbsoluteOrientation
        {
            get => Orientation;
            set => throw new NotImplementedException();
        }

        public ChildRectangle(IShape parent, Angle relativeToParentOrientation, double distFromParentCentre
                                , double FBLength, double RLWidth)
            : base(FBLength, RLWidth, Colors.Red)
        {
            RelativeOrientation = relativeToParentOrientation;
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
            Point centre = ExtraMath.TranslateByVector(Parent.CentrePoint, AbsoluteOrientation, DistFromParentCentre + (FBLength / 2));
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
            return new ChildRectangle(parent, RelativeOrientation.Clone(), DistFromParentCentre, FBLength, RLWidth);
        }
    }
}

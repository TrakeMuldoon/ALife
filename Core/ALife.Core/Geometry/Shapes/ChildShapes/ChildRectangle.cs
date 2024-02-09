using ALife.Core.Utility.Colours;
using ALife.Core.Utility.Maths;
using System;
using ALife.Core.GeometryOld;
using ALife.Core.GeometryOld.Shapes;
using ALife.Core.GeometryOld.Shapes.ChildShapes;

namespace ALife.Core.GeometryOld.Shapes.ChildShapes
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
            : base(FBLength, RLWidth, Colour.Red)
        {
            RelativeOrientation = relativeToParentOrientation;
            DistFromParentCentre = distFromParentCentre;
            Parent = parent;
        }

        private ALife.Core.GeometryOld.Shapes.Point? myCentrePoint;
        public override ALife.Core.GeometryOld.Shapes.Point CentrePoint
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
            ALife.Core.GeometryOld.Shapes.Point centre = GeometryMath.TranslateByVector(Parent.CentrePoint, AbsoluteOrientation, DistFromParentCentre + (FBLength / 2));
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

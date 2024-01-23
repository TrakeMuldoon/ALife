using ALife.Core.Utility;
using ALife.Core.Utility.Maths;
using System;

namespace ALife.Core.Geometry.Shapes.ChildShapes
{
    public class ChildSector : Sector, IChildShape
    {
        public override Angle Orientation
        {
            get
            {
                return AbsoluteOrientation;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public Angle RelativeOrientation
        {
            get;
            set;
        }
        public Angle AbsoluteOrientation
        {
            get
            {
                return Parent.Orientation + OrientationAroundParent + RelativeOrientation;
            }
            set => throw new NotImplementedException();
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
            RelativeOrientation = relativeOrientationAngle;
            OrientationAroundParent = orientationAroundParent;
            distanceFromParentCentre = distFromParentCentre;
            Parent = parent;
            Colour = Utility.Colours.Colour.White;
        }

        public override Point CentrePoint
        {
            get
            {
                Angle startAngle = Parent.Orientation + OrientationAroundParent;
                return GeometryMaths.TranslateByVector(Parent.CentrePoint, startAngle, distanceFromParentCentre);
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
                return GetBoundingBox(Orientation);
            }
        }

        public override IShape CloneShape()
        {
            throw new NotImplementedException("Cannot clone a ChildShape");
        }

        public IShape CloneChildShape(IShape parent)
        {
            ChildSector cs = new ChildSector(parent, OrientationAroundParent.Clone(), distanceFromParentCentre, RelativeOrientation.Clone(), Radius, SweepAngle.Clone());
            cs.Colour = this.Colour;
            return cs;
        }
    }
}

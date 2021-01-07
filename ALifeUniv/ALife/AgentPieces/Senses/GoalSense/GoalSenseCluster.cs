using ALifeUni.ALife.Shapes;
using ALifeUni.ALife.Utility;
using System;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public class GoalSenseCluster : SenseCluster
    {
        private IShape targetShape;
        private ChildSector myShape;
        private WorldObject myParent;

        private DistanceToObjectInput distanceInput;
        private RotationToObjectInput rotationInput;
        public GoalSenseCluster(WorldObject parent, string name, IShape targetObject) : base(parent, name)
        {
            targetShape = targetObject;
            myParent = parent;
            myShape = new ChildSector(parent: parent.Shape,
                                      orientationAroundParent: new Angle(0),
                                      distFromParentCentre: 0,
                                      relativeOrientationAngle: new Angle(0),
                                      radius: 6,
                                      sweep: new Angle(1));
            distanceInput = new DistanceToObjectInput(name + ".HowFar");
            rotationInput = new RotationToObjectInput(name + ".RelativeAngle");
            SubInputs.Add(distanceInput);
            SubInputs.Add(rotationInput);
        }

        public override IShape Shape
        {
            get { return myShape; }
        }

        public override void Detect()
        {
            switch(targetShape)
            {
                case AARectangle aar: DetectAgainstAAR(aar); break;
                default: throw new NotImplementedException("We have not implemented distance to other shapes yet");
            }
        }

        private void DetectAgainstAAR(AARectangle aar)
        {
            Point myCP = myShape.CentrePoint;
            BoundingBox targBB = aar.BoundingBox;

            int distanceValue = 0;
            int rotationValue = 0;

            if(targBB.MinX <= myCP.X
                && myCP.X <= targBB.MaxX)
            {
                if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
                {
                    //I am within
                    distanceValue = 0;
                    rotationValue = 0;
                }
                else if(myCP.Y < targBB.MinY)
                {
                    //below
                    distanceValue = (int)(targBB.MinY - myCP.Y);
                    rotationValue = CalculateRotationFrom(270);
                }
                else //I am above it
                {
                    distanceValue = (int)(myCP.Y - targBB.MaxY);
                    rotationValue = CalculateRotationFrom(90);
                }
            }
            else if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
            {
                //I am to the left or the right
                if(myCP.X < targBB.MinX)
                {
                    //below
                    distanceValue = (int)(targBB.MinX - myCP.X);
                    rotationValue = CalculateRotationFrom(0);
                }
                else //I am above it
                {
                    distanceValue = (int)(myCP.X - targBB.MaxX);
                    rotationValue = CalculateRotationFrom(180);
                }
            }
            else
            {
                //I am not inline with it.
                double xTarg = myCP.X < targBB.MinX ? targBB.MinX : targBB.MaxX;
                double yTarg = myCP.Y < targBB.MinY ? targBB.MinY : targBB.MaxY;
                Point target = new Point(xTarg, yTarg);

                distanceValue = (int)ExtraMath.DistanceBetweenTwoPoints(target, myCP);

                double angleBetweenPoints = ExtraMath.AngleBetweenPoints(target, myCP);
                Angle abp = new Angle(angleBetweenPoints, true);
                rotationValue = CalculateRotationFrom((int)abp.Degrees);
            }

            distanceInput.SetValue(distanceValue);
            rotationInput.SetValue(rotationValue);

            myShape.Orientation.Degrees = rotationValue;
        }

        private int CalculateRotationFrom(int v)
        {
            double rotationdelta = v - myParent.Shape.Orientation.Degrees;
            double finalVal;
            if(rotationdelta < -180)
                finalVal = rotationdelta + 360;
            else if(rotationdelta > 180)
                finalVal = rotationdelta - 360;
            else
                finalVal = rotationdelta;

            return (int)finalVal;
        }

        public override SenseCluster CloneSense(WorldObject newParent)
        {
            return new GoalSenseCluster(newParent, this.Name, this.targetShape);
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            //There is no evolution here. It always targets in the same way.
            return new GoalSenseCluster(newParent, this.Name, this.targetShape);
        }
    }
}

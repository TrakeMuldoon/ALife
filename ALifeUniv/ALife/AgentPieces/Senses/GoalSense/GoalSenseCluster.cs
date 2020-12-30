using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                                      radius: 10,
                                      sweep: new Angle(2));
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

            //double clampX = Math.Clamp(myCP.X, targBB.MinX, targBB.MaxX);
            //double clampY = Math.Clamp(myCP.Y, targBB.MinY, targBB.MaxY);

            if(targBB.MinX <= myCP.X 
                && myCP.X <= targBB.MaxX)
            {
                if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
                {
                    //I am within
                    distanceInput.SetValue(0);
                    rotationInput.SetValue(0);
                    return;
                }
                if(myCP.Y < targBB.MinY)
                {
                    //below
                    distanceInput.SetValue((int)(targBB.MinY - myCP.Y));
                    int rotVal = CalculateRotationFrom(270);
                    rotationInput.SetValue(rotVal);
                    return;
                }
                else //I am above it
                {
                    distanceInput.SetValue((int)(myCP.Y - targBB.MaxY));
                    int rotVal = CalculateRotationFrom(90);
                    rotationInput.SetValue(rotVal);
                    return;
                }
                //I am above or below it
            }
            else if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
            {
                //I am to the left or the right
                if(myCP.X < targBB.MinX)
                {
                    //below
                    distanceInput.SetValue((int)(targBB.MinX - myCP.X));
                    int rotVal = CalculateRotationFrom(0);
                    rotationInput.SetValue(rotVal);
                    return;
                }
                else //I am above it
                {
                    distanceInput.SetValue((int)(myCP.X - targBB.MaxX));
                    int rotVal = CalculateRotationFrom(180);
                    rotationInput.SetValue(rotVal);
                    return;
                }
            }
            else
            {
                //I am not inline with it.
                double xTarg = myCP.X < targBB.MinX ? targBB.MinX : targBB.MaxX;
                double yTarg = myCP.Y < targBB.MinY ? targBB.MinY : targBB.MaxY;
                Point target = new Point(xTarg, yTarg);

                double length = ExtraMath.DistanceBetweenTwoPoints(target, myCP);
                double angleBetweenPoints = ExtraMath.AngleBetweenPoints(target, myCP);
                Angle abp = new Angle(angleBetweenPoints, true);

                distanceInput.SetValue((int)length);
                int rotVal = CalculateRotationFrom((int)abp.Degrees);
                rotationInput.SetValue(rotVal);
                return;
            }
        }

        private int CalculateRotationFrom(int v)
        {
            //me (45) targ (5) exp(-40) t-m = -40
            //me (355) targ (310) exp(-40) t-m = -40
            //me (5) targ(355) exp (-10) t-m = 350
            //me (300) targ(0) exp(60) t-m = -300
            //me (300) targ(355) exp(55) t-m = 55
            //me (0) targ (175) exp(175) t-m = 175
            //me (0) targ (185) exp(-175) t-m = 185

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
            throw new NotImplementedException();
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            throw new NotImplementedException();
        }
    }
}

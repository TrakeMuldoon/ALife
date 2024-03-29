﻿using ALife.Core.Geometry;
using ALife.Core.Geometry.Shapes;
using ALife.Core.Geometry.Shapes.ChildShapes;
using ALife.Core.Utility.Maths;
using ALife.Core.WorldObjects.Agents.Senses.GoalSense;
using System;

namespace ALife.Core.WorldObjects.Agents.Senses
{
    public class GoalSenseCluster : SenseCluster
    {
        public IShape TargetShape
        {
            get;
            private set;
        }
        private ChildSector myShape;
        private WorldObject myParent;

        private DistanceToObjectInput distanceInput;
        private RotationToObjectInput intRotationInput;
        private RotationToObjectDoubleInput doubRotationInput;

        public GoalSenseCluster(WorldObject parent, string name, IShape targetObject) : base(parent, name)
        {
            TargetShape = targetObject;
            myParent = parent;
            myShape = new ChildSector(parent: parent.Shape,
                                      orientationAroundParent: new Angle(0),
                                      distFromParentCentre: 0,
                                      relativeOrientationAngle: new Angle(0),
                                      radius: 6,
                                      sweep: new Angle(1));
            distanceInput = new DistanceToObjectInput(name + ".HowFar");
            intRotationInput = new RotationToObjectInput(name + ".RelativeAngle");
            doubRotationInput = new RotationToObjectDoubleInput(name + ".DoubleRelativeAngle");

            SubInputs.Add(distanceInput);
            SubInputs.Add(intRotationInput);
            SubInputs.Add(doubRotationInput);
        }

        public override IShape Shape
        {
            get { return myShape; }
        }

        public override void Detect()
        {
            switch(TargetShape)
            {
                case AARectangle aar: DetectAgainstAAR(aar); break;
                case Circle cir: DetectAgainstCircle(cir); break;
                default: throw new NotImplementedException("We have not implemented distance to other shapes yet");
            }
        }

        public void ChangeTarget(IShape newTarget)
        {
            TargetShape = newTarget;
        }

        /// <summary>
        /// Detect the orientation towards an AAR
        /// </summary>
        /// <param name="aar"></param>
        private void DetectAgainstAAR(AARectangle aar)
        {
            Point myCP = myShape.CentrePoint;
            BoundingBox targBB = aar.BoundingBox;

            int distanceValue;
            int rotationValue;


            //The Point 0,0 is TopLeft of the scenario

            //This detects if "myself" is inline with the "X" (Above, Below or Within)            
            if(targBB.MinX <= myCP.X
                && myCP.X <= targBB.MaxX)
            {
                if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
                {
                    //I am within the AAR
                    distanceValue = 0;
                    rotationValue = 0;
                }
                else if(myCP.Y < targBB.MinY)
                {
                    //below mathwise, above visually
                    distanceValue = (int)(targBB.MinY - myCP.Y);
                    rotationValue = CalculateRotationFrom(90);
                }
                else
                {
                    //above mathwise, below visually
                    distanceValue = (int)(myCP.Y - targBB.MaxY);
                    rotationValue = CalculateRotationFrom(270);
                }
            }
            //This detects if "myself" is inline with the "Y" (Left, Right) (Within is covered in the previous case)
            else if(targBB.MinY <= myCP.Y
                    && myCP.Y <= targBB.MaxY)
            {
                //I am to the left or the right
                if(myCP.X < targBB.MinX)
                {
                    //I am to the left
                    distanceValue = (int)(targBB.MinX - myCP.X);
                    rotationValue = CalculateRotationFrom(0);
                }
                else
                {
                    //I am to the right 
                    distanceValue = (int)(myCP.X - targBB.MaxX);
                    rotationValue = CalculateRotationFrom(180);
                }
            }
            else
            {
                //I am not inline with it. Therefore the closest Point will be one of the corners. 
                double xTarg = myCP.X < targBB.MinX ? targBB.MinX : targBB.MaxX;
                double yTarg = myCP.Y < targBB.MinY ? targBB.MinY : targBB.MaxY;
                Point target = new Point(xTarg, yTarg);

                distanceValue = (int)GeometryMath.DistanceBetweenTwoPoints(target, myCP);

                double angleBetweenPoints = GeometryMath.AngleBetweenPoints(target, myCP);
                Angle abp = new Angle(angleBetweenPoints, true);
                rotationValue = CalculateRotationFrom((int)abp.Degrees);
            }

            SetResults(distanceValue, rotationValue);
        }

        private void SetResults(int distanceValue, int rotationValue)
        {
            distanceInput.SetValue(distanceValue);
            intRotationInput.SetValue(rotationValue);
            double dubValue = (double)rotationValue / 180;
            doubRotationInput.SetValue(dubValue);

            myShape.Orientation.Degrees = rotationValue;
        }

        private void DetectAgainstCircle(Circle targetCircle)
        {
            Point myCP = myShape.CentrePoint;
            Point target = new Point(targetCircle.CentrePoint.X, targetCircle.CentrePoint.Y);

            int distanceValue = (int)GeometryMath.DistanceBetweenTwoPoints(target, myCP);
            distanceValue -= (int)targetCircle.Radius;

            double angleBetweenPoints = GeometryMath.AngleBetweenPoints(target, myCP);
            Angle abp = new Angle(angleBetweenPoints, true);
            int rotationValue = CalculateRotationFrom((int)abp.Degrees);

            SetResults(distanceValue, rotationValue);
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
            return new GoalSenseCluster(newParent, this.Name, this.TargetShape);
        }

        public override SenseCluster ReproduceSense(WorldObject newParent)
        {
            //There is no evolution here. It always targets in the same way.
            return new GoalSenseCluster(newParent, this.Name, this.TargetShape);
        }
    }
}

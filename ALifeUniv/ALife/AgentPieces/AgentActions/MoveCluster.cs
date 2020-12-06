using ALifeUni.ALife.UtilityClasses;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace ALifeUni.ALife
{
    public class MoveCluster : ActionCluster
    {
        private double Speed = Settings.AgentDefaultSpeed;

        public MoveCluster(Agent self) : base(self, "Move")
        {
            SubActions.Add("GoForward", new ActionPart("GoForward", Name));
            SubActions.Add("StopForward", new ActionPart("StopForward", Name));
            SubActions.Add("GoBackward", new ActionPart("GoBackward", Name));
            SubActions.Add("StopBackward", new ActionPart("StopBackward", Name));
            SubActions.Add("GoLeft", new ActionPart("GoLeft", Name));
            SubActions.Add("StopLeft", new ActionPart("StopLeft", Name));
            SubActions.Add("GoRight", new ActionPart("GoRight", Name));
            SubActions.Add("StopRight", new ActionPart("StopRight", Name));
        }

        public override ActionCluster CloneAction(Agent newParent)
        {
            return new MoveCluster(newParent);
        }

        protected override bool ValidatePreconditions()
        {
            //TODO: Draw this from Config
            return true;
        }
        protected override bool SubActionsEngaged()
        {
            foreach(ActionPart ap in SubActions.Values)
            {
                if(ap.Intensity != 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool AttemptEnact()
        {
            double fwd = GetDirectionValue("Forward");
            double back = GetDirectionValue("Backward");
            double left = GetDirectionValue("Left");
            double right = GetDirectionValue("Right");

            //figure out vector
            double forwardVector = fwd - back;
            double rightVector = right - left;
            bool moveResult = Move(forwardVector, rightVector);
            return moveResult;
        }

        private double GetDirectionValue(string direction)
        {
            string go = "Go" + direction;
            string stop = "Stop" + direction;

            double dirValue = SubActions[go].Intensity - SubActions[stop].Intensity;
            dirValue = Math.Clamp(dirValue, 0.0, 1.0);
            return dirValue;
        }

        double forwardDist = -999;
        double rightDist = -999;

        private bool Move(double forwardMagnitude, double rightMagnitude)
        {
            IShape theShape = self.Shape;
            Point origin = new Point(theShape.CentrePoint.X, theShape.CentrePoint.Y);

            //Move forward, then move right from that point
            forwardDist = Speed * forwardMagnitude;
            rightDist = Speed * rightMagnitude;

            Point tempPoint = ExtraMath.TranslateByVector(origin, theShape.Orientation.Radians, forwardDist);
            Point finalPoint = ExtraMath.TranslateByVector(tempPoint, theShape.Orientation.Radians + (Math.PI / 2), rightDist);

            double halfXLength = theShape.BoundingBox.XLength / 2;
            double halfYHeight = theShape.BoundingBox.YHeight / 2;
            finalPoint.X = Math.Clamp(finalPoint.X, halfXLength, Planet.World.WorldWidth - halfXLength);
            finalPoint.Y = Math.Clamp(finalPoint.Y, halfYHeight, Planet.World.WorldHeight - halfYHeight);

            theShape.CentrePoint = finalPoint;

            ICollisionMap<WorldObject> collider = Planet.World.CollisionLevels[self.CollisionLevel];
            List<WorldObject> collisions = collider.DetectCollisions(self);

            //If there are no collisions, we propogate the move.
            if(collisions.Count == 0)
            {
                collider.MoveObject(self);
                return true;
            }
            else
            {
                CollisionBehvaviour(collisions);
                return false;
            }
        }

        private void CollisionBehvaviour(List<WorldObject> collisions)
        {
            //TODO: Somehow abstract out "Collision behaviour"
            //Collision means death right now
            foreach(WorldObject wo in collisions)
            {
                //wo.Die();
            }
            self.Die();
        }


        protected override void FailureResults()
        {
            //TODO: Draw this from Config
        }

        protected override void SuccessResults()
        {
            //TODO: Draw this from Config
        }

        public override string LastTurnString()
        {
            if(ActivatedLastTurn)
            {
                return "Moved (" + forwardDist + "," + rightDist + ")";
            }
            else
            {
                return "No Move";
            }
        }
    }
}
